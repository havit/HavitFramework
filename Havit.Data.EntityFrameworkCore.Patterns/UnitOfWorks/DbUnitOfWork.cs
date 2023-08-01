using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.Lookups;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.EntityValidation;
using Havit.Data.Patterns.UnitOfWorks;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;

/// <summary>
/// Unit of Work postavená nad <see cref="DbContext" />.
/// </summary>
public class DbUnitOfWork : IUnitOfWork
{
	private readonly IBeforeCommitProcessorsRunner beforeCommitProcessorsRunner;
	private readonly IEntityValidationRunner entityValidationRunner;
	private readonly ILookupDataInvalidationRunner lookupDataInvalidationRunner;
	private List<Action> afterCommits = null;

	private readonly HashSet<object> updateRegistrations = new HashSet<object>();

	/// <summary>
	/// DbContext, nad kterým stojí Unit of Work.
	/// </summary>
	protected IDbContext DbContext { get; private set; }

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
		Contract.Requires<ArgumentNullException>(dbContext != null);
		Contract.Requires<ArgumentNullException>(softDeleteManager != null);

		DbContext = dbContext;
		SoftDeleteManager = softDeleteManager;
		EntityCacheManager = entityCacheManager;
		EntityCacheDependencyManager = entityCacheDependencyManager;
		this.beforeCommitProcessorsRunner = beforeCommitProcessorsRunner;
		this.entityValidationRunner = entityValidationRunner;
		this.lookupDataInvalidationRunner = lookupDataInvalidationRunner;
	}

	/// <summary>
	/// Zajistí uložení změn registrovaných v Unit of Work.
	/// </summary>
	public void Commit()
	{
		BeforeCommit();
		beforeCommitProcessorsRunner.Run(GetAllKnownChanges());

		Changes allKnownChanges = GetAllKnownChanges(); // práme se na změny znovu, runnery mohli seznam objektů k uložení změnit
		entityValidationRunner.Validate(allKnownChanges);
		CacheInvalidationOperation cacheInvalidationOperation = PrepareCacheInvalidation(allKnownChanges);

		DbContext.SaveChanges();

		ClearRegistrationHashSets();
		cacheInvalidationOperation?.Invalidate();
		lookupDataInvalidationRunner.Invalidate(allKnownChanges);

		AfterCommit();
	}

	/// <summary>
	/// Asynchronně uloží změny registrované v Unit of Work.
	/// </summary>
	public async Task CommitAsync(CancellationToken cancellationToken = default)
	{
		BeforeCommit();
		beforeCommitProcessorsRunner.Run(GetAllKnownChanges());
		cancellationToken.ThrowIfCancellationRequested();

		Changes allKnownChanges = GetAllKnownChanges(); // ptáme se na změny znovu, runnery mohli seznam objektů k uložení změnit
		entityValidationRunner.Validate(allKnownChanges);
		CacheInvalidationOperation cacheInvalidationOperation = PrepareCacheInvalidation(allKnownChanges);

		await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		ClearRegistrationHashSets();
		cacheInvalidationOperation?.Invalidate();
		lookupDataInvalidationRunner.Invalidate(allKnownChanges);

		AfterCommit();
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
		updateRegistrations.Clear();
	}

	/// <summary>
	/// Spuštěno po  commitu.
	/// Zajišťuje volání registrovaných after commit akcí (viz RegisterAfterCommitAction).
	/// </summary>
	protected internal virtual void AfterCommit()
	{
		List<Action> registeredAfterCommitActiond = afterCommits;
		// Neprve vyčistíme afterCommits, pak je teprve spustíme.
		// Tím umožníme rekurzivní volání Commitu (resp. volání Commitu z AfterCommitAction), při opačném pořadí (nejdřív spustit, pak vyčistit) dojde k zacyklení.
		afterCommits = null;
		registeredAfterCommitActiond?.ForEach(item => item.Invoke());
	}

	/// <summary>
	/// Registruje akci k provedení po commitu. Akce je provedena metodou AfterCommit.
	/// Při opakovaném commitu již akce není volána.
	/// </summary>
	public void RegisterAfterCommitAction(Action action)
	{
		if (afterCommits == null)
		{
			afterCommits = new List<Action>();
		}
		afterCommits.Add(action);
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

		var modifiedEntities = entityEntries.Where(item => item.State == EntityState.Modified).Select(entry => entry.Entity).ToArray();
		// Entity, o kterých již víme, že jsou ve stavu Modified, odebereme z kolekce updateRegistrations, protože víme, že se nám v běžném kódu budou stále vracet z changetrackeru dle předchozího řádku.
		// Zároveň tak zajistíme, že updateRegistrations nemají žádný průnik s entityEntries (.Entry) a tak můžeme níže bezpečně použít Concat bez rizika vzniku duplicit.
		updateRegistrations.ExceptWith(modifiedEntities);

		var changesFromEntries = entityEntries.Select(entry => new EntityChange
		{
			ChangeType = (ChangeType)entry.State,
			ClrType = entry.Metadata.ClrType,
			EntityType = entry.Metadata,
			EntityEntry = entry,
			Entity = entry.Entity,
		});

		var changesFromUpdateRegistrations = updateRegistrations.Select(item => new EntityChange()
		{
			ChangeType = ChangeType.Update,
			ClrType = item.GetType(),
			EntityType = DbContext.Model.FindEntityType(item.GetType()),
			EntityEntry = DbContext.GetEntry(item, suppressDetectChanges: true),
			Entity = item
		});

		return new Changes(changesFromEntries.Concat(changesFromUpdateRegistrations));
	}

	/// <summary>
	/// Zajistí vložení objektu jako nového objektu (při uložení bude vložen).
	/// </summary>
	public void AddForInsert<TEntity>(TEntity entity)
		where TEntity : class
	{
		PerformAddForInsert(entity);
	}

	/// <summary>
	/// Zajistí vložení objektů jako nové objekty (při uložení budou vloženy).
	/// </summary>
	public void AddRangeForInsert<TEntity>(IEnumerable<TEntity> entities)
		where TEntity : class
	{
		PerformAddForInsert<TEntity>(entities.ToArray());
	}

	/// <summary>
	/// Zajistí vložení objektu jako změněného (při uložení bude změněn).
	/// </summary>
	public void AddForUpdate<TEntity>(TEntity entity)
		where TEntity : class
	{
		PerformAddForUpdate(entity);
	}

	/// <summary>
	/// Zajistí vložení objektů jako změněné objekty (při uložení budou změněny).
	/// </summary>
	public void AddRangeForUpdate<TEntity>(IEnumerable<TEntity> entities)
		where TEntity : class
	{
		PerformAddForUpdate<TEntity>(entities.ToArray());
	}

	/// <summary>
	/// Zajistí odstranění objektu (při uložení bude smazán).
	/// Objekty podporující mazání příznakem budou smazány příznakem.
	/// </summary>
	public void AddForDelete<TEntity>(TEntity entity)
		where TEntity : class
	{
		PerformAddForDelete(entity);
	}

	/// <summary>
	/// Zajistí odstranění objektů (při uložení budou smazány).
	/// Objekty podporující mazání příznakem budou smazány příznakem.
	/// </summary>
	public void AddRangeForDelete<TEntity>(IEnumerable<TEntity> entities)
		where TEntity : class
	{
		PerformAddForDelete<TEntity>(entities.ToArray());
	}

	/// <summary>
	/// Zajistí vložení objektů jako nové objekty (při uložení budou vloženy).
	/// </summary>
	protected virtual void PerformAddForInsert<TEntity>(params TEntity[] entities)
		where TEntity : class
	{
		if (entities.Length > 0)
		{
			DbContext.Set<TEntity>().AddRange(entities);
		}
	}

	/// <summary>
	/// Zajistí vložení objektů jako změněné objekty (při uložení budou změněny).
	/// </summary>
	protected virtual void PerformAddForUpdate<TEntity>(params TEntity[] entities)
		where TEntity : class
	{
		if (entities.Length > 0)
		{
			// registrace na DbSetu pro Update označí entity za modifikované a budou uložené celé bez ohledu na to, zda se na nich skutečně něco změnilo
			// proto implementujeme tak, že trackované entity neřešíme (a předpokládáme, že svou práci provede change tracker)
			// netrackované entity zaregistrujeme jako změněné

			// z výkonových důvodů se očekává, že volaná metoda GetEntityState nevolá change tracker

			DbContext.Set<TEntity>().UpdateRange(entities.Where(entity => DbContext.GetEntityState(entity) == EntityState.Detached).ToArray());
			updateRegistrations.UnionWith(entities);
		}
	}

	/// <summary>
	/// Zajistí odstranění objektů (při uložení budou smazány).
	/// Objekty podporující mazání příznakem budou smazány příznakem - bude jim nastaven příznak smazání a bude nad nimi zavoláno PerformAddForUpdate.
	/// </summary>
	protected virtual void PerformAddForDelete<TEntity>(params TEntity[] entities)
		where TEntity : class
	{
		if (entities.Length > 0)
		{
			if (SoftDeleteManager.IsSoftDeleteSupported<TEntity>())
			{
				foreach (TEntity entity in entities)
				{
					SoftDeleteManager.SetDeleted<TEntity>(entity);
				}
				PerformAddForUpdate(entities);
			}
			else
			{
				DbContext.Set<TEntity>().RemoveRange(entities);
			}
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
}
