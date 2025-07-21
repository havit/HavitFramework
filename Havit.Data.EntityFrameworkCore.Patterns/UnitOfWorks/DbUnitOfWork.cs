using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.Lookups;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.EntityValidation;
using Havit.Data.Patterns.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;

/// <summary>
/// Unit of Work postavená nad <see cref="DbContext" />.
/// </summary>
public class DbUnitOfWork : IUnitOfWork
{
	private readonly IBeforeCommitProcessorsRunner _beforeCommitProcessorsRunner;
	private readonly IEntityValidationRunner _entityValidationRunner;
	private readonly ILookupDataInvalidationRunner _lookupDataInvalidationRunner;

	// internal: unit testy ověřují stav
	internal List<Action> _afterCommitActions = null;
	internal List<Func<CancellationToken, Task>> _asyncAfterCommitsActions = null;
	internal readonly HashSet<object> _updateRegistrations = new HashSet<object>();

	/// <summary>
	/// DbContext, nad kterým stojí Unit of Work.
	/// </summary>
	protected internal IDbContext DbContext { get; private set; }

	/// <summary>
	/// SoftDeleteManager používaný v tomto Unit Of Worku.
	/// </summary>
	protected ISoftDeleteManager SoftDeleteManager { get; private set; }

	/// <summary>
	/// EntityCacheManager používaný v tomto Unit of Worku.
	/// </summary>
	protected IEntityCacheManager EntityCacheManager { get; private set; }

	/// <summary>
	/// EntityCacheDependencyManager používaný v tomto Unit of Worku.
	/// </summary>
	protected IEntityCacheDependencyManager EntityCacheDependencyManager { get; private set; }

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public DbUnitOfWork(IDbContext dbContext, ISoftDeleteManager softDeleteManager, IEntityCacheManager entityCacheManager, IEntityCacheDependencyManager entityCacheDependencyManager, IBeforeCommitProcessorsRunner beforeCommitProcessorsRunner, IEntityValidationRunner entityValidationRunner, ILookupDataInvalidationRunner lookupDataInvalidationRunner)
	{
		DbContext = dbContext;
		SoftDeleteManager = softDeleteManager;
		EntityCacheManager = entityCacheManager;
		EntityCacheDependencyManager = entityCacheDependencyManager;
		_beforeCommitProcessorsRunner = beforeCommitProcessorsRunner;
		_entityValidationRunner = entityValidationRunner;
		_lookupDataInvalidationRunner = lookupDataInvalidationRunner;
	}

	/// <summary>
	/// Zajistí uložení změn registrovaných v Unit of Work.
	/// </summary>
	public void Commit()
	{
		BeforeCommit();

		Changes allKnownChanges = GetAllKnownChanges(); // volá detekci změn na change trackeru

		// spuštění before commit processorů
		ChangeTrackerImpact beforeCommitProcessorImpact = _beforeCommitProcessorsRunner.Run(allKnownChanges);

		if (beforeCommitProcessorImpact == ChangeTrackerImpact.StateChanged)
		{
			// pokud nějaký BeforeCommitProcessor způsobil, že se změnil stav change trackeru (zejména zatrackována nová entity),
			// aktualizujeme seznam známých změn.

			allKnownChanges = GetAllKnownChanges(); // volá detekci změn na change trackeru
		}

		// validace entit před uložením
		_entityValidationRunner.Validate(allKnownChanges);

		// příprava invalidace cache
		CacheInvalidationOperation cacheInvalidationOperation = PrepareCacheInvalidation(allKnownChanges);

		DbContext.SaveChanges(suppressDetectChanges: true); // change tracker byl zavolán a nepotřebujeme jej volat znovu
		Commit_AfterSaveChanges(allKnownChanges, cacheInvalidationOperation);

		AfterCommit();
	}

	/// <summary>
	/// Asynchronně uloží změny registrované v Unit of Work.
	/// </summary>
	public async Task CommitAsync(CancellationToken cancellationToken = default)
	{
		BeforeCommit();

		Changes allKnownChanges = GetAllKnownChanges(); // volá detekci změn na change trackeru

		// spuštění before commit processorů
		ChangeTrackerImpact beforeCommitProcessorImpact = await _beforeCommitProcessorsRunner.RunAsync(allKnownChanges, cancellationToken).ConfigureAwait(false);

		if (beforeCommitProcessorImpact == ChangeTrackerImpact.StateChanged)
		{
			// pokud nějaký BeforeCommitProcessor způsobil, že se změnil stav change trackeru (zejména zatrackována nová entity),
			// aktualizujeme seznam známých změn.

			allKnownChanges = GetAllKnownChanges(); // volá detekci změn na change trackeru
		}

		// validace entit před uložením
		_entityValidationRunner.Validate(allKnownChanges);

		// příprava invalidace cache
		CacheInvalidationOperation cacheInvalidationOperation = PrepareCacheInvalidation(allKnownChanges);

		await DbContext.SaveChangesAsync(suppressDetectChanges: true, cancellationToken: cancellationToken).ConfigureAwait(false); // change tracker byl zavolán a nepotřebujeme jej volat znovu
		Commit_AfterSaveChanges(allKnownChanges, cacheInvalidationOperation);

		await AfterCommitAsync(cancellationToken).ConfigureAwait(false);
	}

	private void Commit_AfterSaveChanges(Changes allKnownChanges, CacheInvalidationOperation cacheInvalidationOperation)
	{
		ClearRegistrationHashSets();
		cacheInvalidationOperation?.Invalidate();
		_lookupDataInvalidationRunner.Invalidate(allKnownChanges);
	}

	/// <summary>
	/// Spuštěno před commitem.
	/// </summary>
	protected internal virtual void BeforeCommit()
	{
		// NOOP
	}

	private void ClearRegistrationHashSets()
	{
		_updateRegistrations.Clear();
	}

	/// <summary>
	/// Spuštěno po  commitu.
	/// Zajišťuje volání registrovaných after commit akcí (viz RegisterAfterCommitAction).
	/// V případě registrace asynchronní after commit akce vyhodí výjimku.
	/// </summary>
	protected internal virtual void AfterCommit()
	{
		if (_asyncAfterCommitsActions != null)
		{
			throw new InvalidOperationException($"Cannot use asynchronous after commit actions for {nameof(Commit)} method method. Call {nameof(CommitAsync)} method or use only synchronous after commit actions.");
		}

		List<Action> registeredAfterCommitActions = _afterCommitActions;
		// Neprve vyčistíme afterCommits, pak je teprve spustíme.
		// Tím umožníme rekurzivní volání Commitu (resp. volání Commitu z AfterCommitAction), při opačném pořadí (nejdřív spustit, pak vyčistit) dojde k zacyklení.
		_afterCommitActions = null;

		if (registeredAfterCommitActions != null)
		{
			foreach (var registeredAfterCommitAction in registeredAfterCommitActions)
			{
				registeredAfterCommitAction.Invoke();
			}
		}
	}

	/// <summary>
	/// Spuštěno po commitu.
	/// Zajišťuje volání registrovaných after commit akcí (viz RegisterAfterCommitAction).
	/// Podporuje synchronní i asynchronní after commit akce.
	/// </summary>
	protected internal virtual async Task AfterCommitAsync(CancellationToken cancellationToken)
	{
		List<Action> registeredAfterCommitActions = _afterCommitActions;
		List<Func<CancellationToken, Task>> registeredAsyncAfterCommitActions = _asyncAfterCommitsActions;
		// Neprve vyčistíme afterCommits a asyncAfterCommits, pak je teprve spustíme.
		// Tím umožníme rekurzivní volání Commitu (resp. volání Commitu z AfterCommitAction), při opačném pořadí (nejdřív spustit, pak vyčistit) dojde k zacyklení.
		_afterCommitActions = null;
		_asyncAfterCommitsActions = null;

		if (registeredAfterCommitActions != null)
		{
			foreach (var registeredAfterCommitAction in registeredAfterCommitActions)
			{
				registeredAfterCommitAction.Invoke();
			}
		}

		if (registeredAsyncAfterCommitActions != null)
		{
			foreach (var registeredAsyncAfterCommitAction in registeredAsyncAfterCommitActions)
			{
				await registeredAsyncAfterCommitAction(cancellationToken).ConfigureAwait(false);
			}
		}
	}

	/// <summary>
	/// Registruje akci k provedení po commitu. Akce je provedena metodou AfterCommit.
	/// Při opakovaném commitu již akce není volána.
	/// </summary>
	public void RegisterAfterCommitAction(Action action)
	{
		ArgumentNullException.ThrowIfNull(action);
		if (_afterCommitActions == null)
		{
			_afterCommitActions = new List<Action>(1);
		}
		_afterCommitActions.Add(action);
	}

	/// <summary>
	/// Registruje asynchronní akci k provedení po commitu. Akce je provedena metodou AfterCommit.
	/// Při opakovaném commitu již akce není volána.
	/// </summary>
	public void RegisterAfterCommitAction(Func<CancellationToken, Task> asyncAction)
	{
		ArgumentNullException.ThrowIfNull(asyncAction);
		if (_asyncAfterCommitsActions == null)
		{
			_asyncAfterCommitsActions = new List<Func<CancellationToken, Task>>(1);
		}
		_asyncAfterCommitsActions.Add(asyncAction);
	}

	/// <summary>
	/// Vrací všechny známé změny ke commitu, vychází z registrací objektů a používá také DbContext a jeho changetracker.
	/// </summary>
	protected internal Changes GetAllKnownChanges()
	{
		// Dříve jsme používali insertRegistrations a deleteRegistrations. Nyní je již nepoužíváme a spoléháme se plně na change tracker.
		// Umožníme tak klientskému programátorovi změnit stav entity, např. pro kompenzaci konfliktu při ukládání entity (DbConcurencyUpdateException).
		// Avšak stále používáme updateRegistrations, protože PerformAddForUpdate z výkonových důvodů nemění stav trackovaných entit (viz komentář v PerformAddForUpdate).

		// Pro získání updates v následujícím kódu je podstatné, že proběhl changetracker.
		// Konkrétně, že reálně změněné entity jsou ve stavu Modified a nikoliv Unchanged, jak je nechává PerformAddForUpdate.

		EntityEntry[] entityEntries = DbContext.GetEntries(suppressDetectChanges: false).Where(item => (item.State == EntityState.Added) || (item.State == EntityState.Modified) || (item.State == EntityState.Deleted)).ToArray();

		// Abychom umožnili kompenzaci konfliktu na entitě ve stavu Modified, potřebujeme také aby entita zmizela z kolekce updateRegistrations.
		// Tím se pro ni přestanou volat BeforeCommitProcessory.

		// Entity, o kterých již víme, že jsou ve stavu Modified, odebereme z kolekce updateRegistrations, protože víme, že se nám v běžném kódu budou stále vracet z changetrackeru dle předchozího řádku.
		// Zároveň tak zajistíme, že updateRegistrations nemají žádný průnik s entityEntries (.Entry) a tak můžeme níže bezpečně použít Concat bez rizika vzniku duplicit.
		_updateRegistrations.ExceptWith(entityEntries.Where(item => item.State == EntityState.Modified).Select(entry => entry.Entity));

		var changesFromEntries = entityEntries.Select(entry => new EntityChange
		{
			ChangeType = (ChangeType)entry.State,
			ClrType = entry.Metadata.ClrType,
			EntityType = entry.Metadata,
			EntityEntry = entry,
			Entity = entry.Entity,
		});

		var changesFromUpdateRegistrations = _updateRegistrations.Select(item => new EntityChange()
		{
			ChangeType = ChangeType.Update,
			ClrType = item.GetType(),
			EntityType = DbContext.Model.FindEntityType(item.GetType()),
			EntityEntry = DbContext.GetEntry(item, suppressDetectChanges: true),
			Entity = item
		});

		return new Changes(changesFromEntries.Concat(changesFromUpdateRegistrations).Cast<Change>().ToList());
	}

	/// <summary>
	/// Zajistí vložení objektu jako nového objektu (při uložení bude vložen).
	/// </summary>
	public void AddForInsert<TEntity>(TEntity entity)
		where TEntity : class
	{
		ArgumentNullException.ThrowIfNull(entity);
		PerformAddForInsert(entity);
	}

	/// <summary>
	/// Zajistí vložení objektu jako nového objektu (při uložení bude vložen).
	/// Určeno pro použití s entitami využívajícími asynchronní operace, typicky jen s použití HiLo strategie generování identifikátorů entit.
	/// </summary>
	public async ValueTask AddForInsertAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
		where TEntity : class
	{
		ArgumentNullException.ThrowIfNull(entity);
		await PerformAddForInsertAsync(entity, cancellationToken).ConfigureAwait(false);
	}

	/// <summary>
	/// Zajistí vložení objektů jako nové objekty (při uložení budou vloženy).
	/// </summary>
	public void AddRangeForInsert<TEntity>(IEnumerable<TEntity> entities)
		where TEntity : class
	{
		ArgumentNullException.ThrowIfNull(entities);
		PerformAddRangeForInsert(entities);
	}

	/// <summary>
	/// Zajistí vložení objektů jako nové objekty (při uložení budou vloženy).
	/// Určeno pro použití s entitami využívajícími asynchronní operace, typicky jen s použití HiLo strategie generování identifikátorů entit.
	/// </summary>
	public async ValueTask AddRangeForInsertAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
		where TEntity : class
	{
		ArgumentNullException.ThrowIfNull(entities);
		await PerformAddRangeForInsertAsync(entities, cancellationToken).ConfigureAwait(false);
	}

	/// <summary>
	/// Zajistí vložení objektu jako změněného (při uložení bude změněn).
	/// </summary>
	public void AddForUpdate<TEntity>(TEntity entity)
		where TEntity : class
	{
		ArgumentNullException.ThrowIfNull(entity);
		PerformAddForUpdate(entity);
	}

	/// <summary>
	/// Zajistí vložení objektů jako změněné objekty (při uložení budou změněny).
	/// </summary>
	public void AddRangeForUpdate<TEntity>(IEnumerable<TEntity> entities)
		where TEntity : class
	{
		ArgumentNullException.ThrowIfNull(entities);
		PerformAddRangeForUpdate(entities);
	}

	/// <summary>
	/// Zajistí odstranění objektu (při uložení bude smazán).
	/// Objekty podporující mazání příznakem budou smazány příznakem.
	/// </summary>
	public void AddForDelete<TEntity>(TEntity entity)
		where TEntity : class
	{
		ArgumentNullException.ThrowIfNull(entity);
		PerformAddForDelete(entity);
	}

	/// <summary>
	/// Zajistí odstranění objektů (při uložení budou smazány).
	/// Objekty podporující mazání příznakem budou smazány příznakem.
	/// </summary>
	public void AddRangeForDelete<TEntity>(IEnumerable<TEntity> entities)
		where TEntity : class
	{
		ArgumentNullException.ThrowIfNull(entities);
		PerformAddRangeForDelete(entities);
	}

	/// <summary>
	/// Zajistí vložení objektu jako nového objektu (při uložení bude vložen).
	/// </summary>
	protected virtual void PerformAddForInsert<TEntity>(TEntity entity)
		where TEntity : class
	{
		DbContext.Set<TEntity>().Add(entity);
	}

	/// <summary>
	/// Zajistí vložení objektu jako nového objektu (při uložení bude vložen).
	/// </summary>
	protected virtual async ValueTask PerformAddForInsertAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
		where TEntity : class
	{
		await DbContext.Set<TEntity>().AddAsync(entity, cancellationToken).ConfigureAwait(false);
	}

	/// <summary>
	/// Zajistí vložení objektů jako nové objekty (při uložení budou vloženy).
	/// </summary>
	protected virtual void PerformAddRangeForInsert<TEntity>(IEnumerable<TEntity> entities)
		where TEntity : class
	{
		DbContext.Set<TEntity>().AddRange(entities);
	}

	/// <summary>
	/// Zajistí vložení objektů jako nové objekty (při uložení budou vloženy).
	/// </summary>
	protected virtual async ValueTask PerformAddRangeForInsertAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
		where TEntity : class
	{
		await DbContext.Set<TEntity>().AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
	}

	/// <summary>
	/// Zajistí vložení objektu jako změněného objektu (při uložen bude změněn).
	/// </summary>
	protected virtual void PerformAddForUpdate<TEntity>(TEntity entity)
		where TEntity : class
	{
		// z výkonových důvodů se očekává, že volaná metoda GetEntityState nevolá change tracker
		if (DbContext.GetEntityState(entity) == EntityState.Detached)
		{
			DbContext.Set<TEntity>().Update(entity);
		}
		else
		{
			_updateRegistrations.Add(entity);
		}
	}

	/// <summary>
	/// Zajistí vložení objektů jako změněné objekty (při uložení budou změněny).
	/// </summary>
	protected virtual void PerformAddRangeForUpdate<TEntity>(IEnumerable<TEntity> entities)
		where TEntity : class
	{
		// Registrace na DbSetu pro Update označí entity za modifikované a budou uložené celé bez ohledu na to, zda se na nich skutečně něco změnilo.
		// Proto implementujeme tak, že netrackované entity zaregistrujeme jako změněné.
		// Trackované entity z pohledu DbSetu neřešíme (a předpokládáme, že svou práci provede change tracker),
		// pouze si je zapíšeme mezi updateRegistrations (aby se na něj při uložení volal BeforeCommitProcessor).
		// Vzhledem k tomu, že vše chceme realizovat v rámci jednoho průchodu entit, zapisujeme si entity do updateRegistrations
		// v rámci iterování entit (v rámci volání Where).

		// Z výkonových důvodů se očekává, že volaná metoda GetEntityState nevolá change tracker.
		DbContext.Set<TEntity>().UpdateRange(entities
			.Where(entity =>
			{
				bool isDetached = DbContext.GetEntityState(entity) == EntityState.Detached;
				if (!isDetached)
				{
					_updateRegistrations.Add(entity);
				}
				return isDetached;
			}));
	}

	/// <summary>
	/// Zajistí odstranění objektu (při uložení bude smazán).
	/// Objekty podporující mazání příznakem budou smazány příznakem - bude jim nastaven příznak smazání a bude nad nimi zavoláno PerformAddForUpdate.
	/// </summary>
	protected virtual void PerformAddForDelete<TEntity>(TEntity entity)
		where TEntity : class
	{
		if (SoftDeleteManager.IsSoftDeleteSupported<TEntity>())
		{
			SoftDeleteManager.SetDeleted<TEntity>(entity);
			PerformAddForUpdate(entity);
		}
		else
		{
			DbContext.Set<TEntity>().Remove(entity);
		}
	}

	/// <summary>
	/// Zajistí odstranění objektů (při uložení budou smazány).
	/// Objekty podporující mazání příznakem budou smazány příznakem - bude jim nastaven příznak smazání a bude nad nimi zavoláno PerformAddForUpdate.
	/// </summary>
	protected virtual void PerformAddRangeForDelete<TEntity>(IEnumerable<TEntity> entities)
		where TEntity : class
	{
		if (SoftDeleteManager.IsSoftDeleteSupported<TEntity>())
		{
			// Pokud jde o list, můžeme levně projít seznam a následně zavolat PerformAddForUpdate, nevadí nám dva průchody.
			if (entities is IList<TEntity> entitiesList)
			{
				for (int i = 0; i < entitiesList.Count; i++)
				{
					SoftDeleteManager.SetDeleted<TEntity>(entitiesList[i]);
				}
				PerformAddRangeForUpdate(entitiesList);
			}
			else
			{
				// Chceme 
				// a) zavolat nad entitou SoftDeleteManager.SetDeleted
				// b) včechny entity předat PerformAddForUpdate
				// a to v jednom průchodu.				
				// Abychom zajistili jediný průchod, nad entitami voláme SoftDeleteManager.SetDeleted v rámci jejich iterování.
				PerformAddRangeForUpdate(entities.Select(entity =>
				{
					SoftDeleteManager.SetDeleted<TEntity>(entity);
					return entity;
				}));
			}
		}
		else
		{
			DbContext.Set<TEntity>().RemoveRange(entities);
		}
	}

	/// <summary>
	/// Oznámí k invalidaci cache všechny změněné objekty.
	/// </summary>
	protected virtual CacheInvalidationOperation PrepareCacheInvalidation(Changes allKnownChanges)
	{
		var prepareToInvalidate1 = EntityCacheManager.PrepareCacheInvalidation(allKnownChanges);
		var prepareToInvalidate2 = EntityCacheDependencyManager.PrepareCacheInvalidation(allKnownChanges);

		return ((prepareToInvalidate1 != null) || (prepareToInvalidate2 != null))
			? new CacheInvalidationOperation(() =>
				{
					prepareToInvalidate1?.Invalidate();
					prepareToInvalidate2?.Invalidate();
				})
			: null;
	}

	/// <summary>
	/// Vyčistí registrace objektů, after commit actions, atp. (vč. podkladového DbContextu a jeho changetrackeru).
	/// </summary>	
	public void Clear()
	{
		ClearRegistrationHashSets();
		_afterCommitActions = null;
		_asyncAfterCommitsActions = null;
		DbContext.ChangeTracker.Clear();
	}
}
