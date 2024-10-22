using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;
using Havit.Data.Patterns.Exceptions;
using Havit.Data.Patterns.Infrastructure;
using Havit.Data.Patterns.Repositories;
using Havit.Linq;
using Havit.Linq.Expressions;
using Havit.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System.Linq.Expressions;

namespace Havit.Data.EntityFrameworkCore.Patterns.Lookups;

/// <summary>
/// Bázová třída pro vyhledávání entit dle klíče (jednoduchého i složeného).
/// Použití:
/// 1) Podědit od této třídy a implementovat abstrakční vlastností
/// 2) Eventuelně nakonfigurovat chování overridováním virtuálních vlastností.
/// 3) Implementovat nějaký vlastní interface, imlementace bude volat GetEntityByLookupKey (ev. GetEntityKeyByLookupKey).
/// </summary>
/// <typeparam name="TLookupKey">Typ klíče.</typeparam>
/// <typeparam name="TEntity">Entita, kterou hledáme.</typeparam>
/// <remarks>
/// Určeno k implementaci občasně měněných entit, ev. entit které se mění hromadně (naráz).
/// Není garantována stoprocentní spolehlivost u entit, které se mění často (myšleno zejména paralelně) v různých transakcích - invalidace a aktualizace může proběhnout v jiném pořadí, než v jakém doběhly commity.
/// </remarks>
public abstract class LookupServiceBase<TLookupKey, TEntity> : ILookupDataInvalidationService
	where TEntity : class
{
	private static readonly CriticalSection<Type> s_criticalSection = new CriticalSection<Type>();

	private readonly IEntityLookupDataStorage lookupStorage;
	private readonly IRepository<TEntity> repository; // TODO: QueryTags nedokonalé, bude se hlásit query tag dle DbRepository.
	private readonly IDbContext dbContext;
	private readonly IEntityKeyAccessor entityKeyAccessor;
	private readonly ISoftDeleteManager softDeleteManager;
	private readonly IDistributedLookupDataInvalidationService distributedLookupDataInvalidationService;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	protected LookupServiceBase(IEntityLookupDataStorage lookupStorage, IRepository<TEntity> repository, IDbContext dbContext, IEntityKeyAccessor entityKeyAccessor, ISoftDeleteManager softDeleteManager)
		: this(lookupStorage, repository, dbContext, entityKeyAccessor, softDeleteManager, null)
	{
	}

	/// <summary>
	/// Konstruktor.
	/// </summary>
	protected LookupServiceBase(IEntityLookupDataStorage lookupStorage, IRepository<TEntity> repository, IDbContext dbContext, IEntityKeyAccessor entityKeyAccessor, ISoftDeleteManager softDeleteManager, IDistributedLookupDataInvalidationService distributedLookupDataInvalidationService)
	{
		this.lookupStorage = lookupStorage;
		this.repository = repository;
		this.dbContext = dbContext;
		this.entityKeyAccessor = entityKeyAccessor;
		this.softDeleteManager = softDeleteManager;
		this.distributedLookupDataInvalidationService = distributedLookupDataInvalidationService ?? new NullDistributedLookupDataInvalidationService();
	}

	/// <summary>
	/// Indikuje, zda jsou použity i smazané záznamy (Výchozí hodnota: false).
	/// </summary>
	protected virtual bool IncludeDeleted => false;

	/// <summary>
	/// Indikuje, zda je v případě nenalezení vyhozena výjimka ObjectNotFound (pokud je true), či zda je vrácena null hodnota (pokud je false). (Vychozí hodnota: true).
	/// </summary>
	protected virtual bool ThrowExceptionWhenNotFound => true;

	/// <summary>
	/// Poskytuje klíč, podle kterého jsou záznamy dohledávány.
	/// </summary>
	protected abstract Expression<Func<TEntity, TLookupKey>> LookupKeyExpression { get; }

	/// <summary>
	/// Poskytuje volitelný filtr, který se má na záznamy uplatnit (např. jen neprázdný párovací klíč, atp.).
	/// </summary>
	protected virtual Expression<Func<TEntity, bool>> Filter => null;

	/// <summary>
	/// Nápověda pro efektivnější fungování služby.
	/// </summary>
	protected abstract LookupServiceOptimizationHints OptimizationHints { get; }

	/// <summary>
	/// Vrátí entitu na základě lookup klíče.
	/// Není-li nalezena, řídí se chování dle ThrowExceptionWhenNotFound.
	/// Entita je na základě klíče vrácena z repository, což umožní použít cache.
	/// </summary>
	protected TEntity GetEntityByLookupKey(TLookupKey lookupKey)
	{
		return TryGetEntityKeyByLookupKey(lookupKey, out int entityKey)
			? repository.GetObject(entityKey)
			: null;
	}

	/// <summary>
	/// Vrátí entitu na základě lookup klíče.
	/// Není-li nalezena, řídí se chování dle ThrowExceptionWhenNotFound.
	/// Entita je na základě klíče vrácena z repository, což umožní použít cache.
	/// </summary>
	protected async ValueTask<TEntity> GetEntityByLookupKeyAsync(TLookupKey lookupKey, CancellationToken cancellationToken = default)
	{
		var result = await TryGetEntityKeyByLookupKeyAsync(lookupKey, cancellationToken).ConfigureAwait(false);
		return result.Success
			? await repository.GetObjectAsync(result.EntityKey, cancellationToken).ConfigureAwait(false)
			: null;
	}

	/// <summary>
	/// Vrátí entity na základě lookup klíčů.
	/// Není-li nalezena, řídí se chování dle ThrowExceptionWhenNotFound, pokud nevyhazuje výjimku a entita není nalezena, prostě není ve výsledku metody.
	/// Entity jsou na základě klíče vráceny z repository, což umožní použít cache.
	/// </summary>
	protected List<TEntity> GetEntitiesByLookupKeys(TLookupKey[] lookupKeys)
	{
		var entityKeys = lookupKeys.Select(lookupKey =>
			{
				bool success = TryGetEntityKeyByLookupKey(lookupKey, out int entityKey);
				return new { Success = success, EntityKey = entityKey };
			})
			.Where(result => result.Success)
			.Select(result => result.EntityKey)
			.ToArray();

		return repository.GetObjects(entityKeys);
	}

	/// <summary>
	/// Vrátí entity na základě lookup klíčů.
	/// Není-li nalezena, řídí se chování dle ThrowExceptionWhenNotFound, pokud nevyhazuje výjimku a entita není nalezena, prostě není ve výsledku metody.
	/// Entity jsou na základě klíče vráceny z repository, což umožní použít cache.
	/// </summary>
	protected async ValueTask<List<TEntity>> GetEntitiesByLookupKeysAsync(TLookupKey[] lookupKeys, CancellationToken cancellationToken = default)
	{
		List<int> entityKeys = new List<int>(lookupKeys.Length);
		foreach (var lookupKey in lookupKeys)
		{
			var result = await TryGetEntityKeyByLookupKeyAsync(lookupKey, cancellationToken).ConfigureAwait(false);
			if (result.Success)
			{
				entityKeys.Add(result.EntityKey);
			}
		}

		return await repository.GetObjectsAsync(entityKeys.ToArray(), cancellationToken).ConfigureAwait(false);
	}

	/// <summary>
	/// Vyhledá klíč entity entitu na základě lookup klíče.
	/// Není-li nalezena, řídí se chování dle ThrowExceptionWhenNotFound.
	/// Určeno pro možnost získat si více klíčů entit a následné hromadné načtení.
	/// </summary>
	private bool TryGetEntityKeyByLookupKey(TLookupKey lookupKey, out int entityKey)
	{
		string storageKey = GetStorageKey();
		EntityLookupData<TEntity, int, TLookupKey> entityLookupData = lookupStorage.GetEntityLookupData<TEntity, int, TLookupKey>(storageKey);
		if (entityLookupData == null)
		{
			s_criticalSection.ExecuteAction(this.GetType(), () =>
			{
				entityLookupData = CreateEntityLookupData();
			});
			lookupStorage.StoreEntityLookupData(storageKey, entityLookupData);
		}

		lock (entityLookupData)
		{
			if (entityLookupData.EntityKeyByLookupKeyDictionary.TryGetValue(lookupKey, out entityKey))
			{
				return true;
			}
			else
			{
				if (ThrowExceptionWhenNotFound)
				{
					throw new ObjectNotFoundException($"Object with key '{lookupKey}' not found. To return null instead of throwing exception, please override property {nameof(ThrowExceptionWhenNotFound)} and return false.");
				}
				return false;
			}
		}
	}

	/// <summary>
	/// Vyhledá klíč entity entitu na základě lookup klíče.
	/// Není-li nalezena, řídí se chování dle ThrowExceptionWhenNotFound.
	/// Určeno pro možnost získat si více klíčů entit a následné hromadné načtení.
	/// </summary>
	private async ValueTask<TryGetEntityKeyByLookupKeyResult> TryGetEntityKeyByLookupKeyAsync(TLookupKey lookupKey, CancellationToken cancellationToken = default)
	{
		string storageKey = GetStorageKey();
		EntityLookupData<TEntity, int, TLookupKey> entityLookupData = lookupStorage.GetEntityLookupData<TEntity, int, TLookupKey>(storageKey);
		if (entityLookupData == null)
		{
			await s_criticalSection.ExecuteActionAsync(this.GetType(), async () =>
			{
				entityLookupData = await CreateEntityLookupDataAsync().ConfigureAwait(false);
			}, cancellationToken).ConfigureAwait(false);
			lookupStorage.StoreEntityLookupData(storageKey, entityLookupData);
		}

		lock (entityLookupData)
		{
			if (entityLookupData.EntityKeyByLookupKeyDictionary.TryGetValue(lookupKey, out int entityKey))
			{
				return new TryGetEntityKeyByLookupKeyResult { Success = true, EntityKey = entityKey };
			}
			else
			{
				if (ThrowExceptionWhenNotFound)
				{
					throw new ObjectNotFoundException($"Object with key '{lookupKey}' not found. To return null instead of throwing exception, please override property {nameof(ThrowExceptionWhenNotFound)} and return false.");
				}
				return new TryGetEntityKeyByLookupKeyResult { Success = false, EntityKey = default };
			}
		}
	}

	/// <summary>
	/// Vyčistá data používaná pro vyhledávání.
	/// Použití pro
	/// a) možnost invalidovat data v případě změny mimo UnitOfWork
	/// b) možnost invalidovat data pro uvolnění paměti, pokud již párování nemá smysl udržovat v paměti.
	/// </summary>
	protected void ClearLookupData()
	{
		// Pozor, jako side-effekt dojde k nové kompilaci LookupKeyExpression a Filteru.
		distributedLookupDataInvalidationService.Invalidate(GetStorageKey());
		lookupStorage.RemoveEntityLookupData(GetStorageKey());
	}

	/// <summary>
	/// Klíč, pod jakým jsou lookup data uložena ve storage.
	/// </summary>
	protected virtual string GetStorageKey()
	{
		return this.GetType().FullName;
	}

	/// <summary>
	/// Factory pro EntityLookupData.
	/// </summary>
	internal EntityLookupData<TEntity, int, TLookupKey> CreateEntityLookupData()
	{
		IQueryable<EntityLookupPair<int, TLookupKey>> entityLookupDataQuery = GetEntityLookupDataQuery();

		string tag = QueryTagBuilder.CreateTag(this.GetType(), nameof(CreateEntityLookupData));
		List<EntityLookupPair<int, TLookupKey>> pairs = entityLookupDataQuery
			.TagWith(tag)
			.ToList();

		return CreateEntityLookupData(pairs);
	}

	/// <summary>
	/// Factory pro EntityLookupData.
	/// </summary>
	/// <remarks>
	/// Virtuální je pro možnost ji vyměnit pro účely unit testů - v testech na mockovaných datech nelze použít ToListAsync.</remarks>
	internal virtual async Task<EntityLookupData<TEntity, int, TLookupKey>> CreateEntityLookupDataAsync(CancellationToken cancellationToken = default)
	{
		IQueryable<EntityLookupPair<int, TLookupKey>> entityLookupDataQuery = GetEntityLookupDataQuery();

		string tag = QueryTagBuilder.CreateTag(this.GetType(), nameof(CreateEntityLookupDataAsync));
		List<EntityLookupPair<int, TLookupKey>> pairs = await entityLookupDataQuery
			.ToListAsync(cancellationToken)
			.ConfigureAwait(false);

		return CreateEntityLookupData(pairs);
	}

	/// <summary>
	/// Vyrobí query vracející lookup data z entity
	/// </summary>
	/// <returns></returns>
	private IQueryable<EntityLookupPair<int, TLookupKey>> GetEntityLookupDataQuery()
	{
		string entityKeyPropertyName = entityKeyAccessor.GetEntityKeyPropertyNames(typeof(TEntity)).Single();
		Expression<Func<TEntity, TLookupKey>> lookupKeyExpression = LookupKeyExpression;

		ParameterExpression expressionParameter = Expression.Parameter(typeof(TEntity), "item");
		Expression<Func<TEntity, EntityLookupPair<int, TLookupKey>>> lambdaExpression = (Expression<Func<TEntity, EntityLookupPair<int, TLookupKey>>>)Expression.Lambda(
			Expression.MemberInit(Expression.New(typeof(EntityLookupPair<int, TLookupKey>)),
			Expression.Bind(typeof(EntityLookupPair<int, TLookupKey>).GetProperty(nameof(EntityLookupPair<int, TLookupKey>.EntityKey)), Expression.MakeMemberAccess(expressionParameter, typeof(TEntity).GetProperty(entityKeyPropertyName))),
			Expression.Bind(typeof(EntityLookupPair<int, TLookupKey>).GetProperty(nameof(EntityLookupPair<int, TLookupKey>.LookupKey)), ExpressionExt.ReplaceParameter(lookupKeyExpression.Body, lookupKeyExpression.Parameters[0], expressionParameter))),
			expressionParameter);

		IQueryable<EntityLookupPair<int, TLookupKey>> pairsQuery = (IncludeDeleted ? dbContext.Set<TEntity>().AsQueryable(null) : dbContext.Set<TEntity>().AsQueryable(null).WhereNotDeleted(softDeleteManager))
			.WhereIf(Filter != null, Filter)
			.Select(lambdaExpression);

		return pairsQuery;
	}

	private EntityLookupData<TEntity, int, TLookupKey> CreateEntityLookupData(List<EntityLookupPair<int, TLookupKey>> pairs)
	{
		Dictionary<TLookupKey, int> entityKeyByLookupKeyDictionary;
		try
		{
			entityKeyByLookupKeyDictionary = pairs.ToDictionary(pair => pair.LookupKey, pair => pair.EntityKey);
		}
		catch (ArgumentException argumentException)
		{
			throw new InvalidOperationException("Source data contains duplicity.", argumentException);
		}

		Dictionary<int, TLookupKey> lookupKeyByEntityKeyDictionary = !OptimizationHints.HasFlag(LookupServiceOptimizationHints.EntityIsReadOnly)
			? pairs.ToDictionary(pair => pair.EntityKey, pair => pair.LookupKey)
			: null;

		return new EntityLookupData<TEntity, int, TLookupKey>(
			entityKeyByLookupKeyDictionary,
			lookupKeyByEntityKeyDictionary,
			LookupKeyExpression.Compile(),
			Filter?.Compile());
	}


	/// <summary>
	/// Provede invalidaci lookup dat bez ohledu na provedené změny.
	/// </summary>
	void ILookupDataInvalidationService.Invalidate()
	{
		ClearLookupData();
	}

	/// <summary>
	/// Aktualizuje data po uložení v UnitOfWork.
	/// </summary>
	/// <remarks>
	/// Metodu nechceme by default veřejnou.
	/// </remarks>
	void ILookupDataInvalidationService.InvalidateAfterCommit(Changes changes)
	{
		Invalidate(changes);
	}

	/// <summary>
	/// Aktualizuje data po uložení v UnitOfWork.
	/// </summary>
	protected virtual void Invalidate(Changes changes)
	{
		// entita je read-only, neprovádíme žádnou invalidaci
		if (OptimizationHints.HasFlag(LookupServiceOptimizationHints.EntityIsReadOnly))
		{
			return;
		}

		// nedošlo k žádné změně sledované entity, neprovádíme žádnou invalidaci
		IEnumerable<Change> entityChanges = changes.GetChangesByClrType()[typeof(TEntity)];
		if (!entityChanges.Any())
		{
			return;
		}

		// musí předcházet opuštění v případě neexistence lookup dat
		distributedLookupDataInvalidationService.Invalidate(GetStorageKey());

		EntityLookupData<TEntity, int, TLookupKey> entityLookupData = lookupStorage.GetEntityLookupData<TEntity, int, TLookupKey>(GetStorageKey());
		if (entityLookupData == null)
		{
			// nemáme sestaven lookupTable, není co invalidovat (avšak distrubuovanou invalidaci nutno řešit, proto předchází tomuto bloku kódu).
			return;
		}

		var updatedAndDeletedEntities = entityChanges
			.Where(change => (change.ChangeType == ChangeType.Update) || (change.ChangeType == ChangeType.Delete))
			.Select(change => (TEntity)change.Entity)
			.ToList();

		foreach (var entity in updatedAndDeletedEntities)
		{
			// mohlo dojít ke změně na entitě (klíče, filtru)
			// podíváme se, zda máme entitu v evidenci a pokud ano, invalidujeme ji
			int entityKey = (int)entityKeyAccessor.GetEntityKeyValues(entity).Single();
			lock (entityLookupData)
			{
				if (entityLookupData.LookupKeyByEntityKeyDictionary.TryGetValue(entityKey, out TLookupKey lookupKey))
				{
					entityLookupData.EntityKeyByLookupKeyDictionary.Remove(lookupKey);
					entityLookupData.LookupKeyByEntityKeyDictionary.Remove(entityKey);
				}
			}
		}

		bool softDeleteSupported = softDeleteManager.IsSoftDeleteSupported<TEntity>();
		Func<TEntity, bool> softDeleteCompiledLambda = softDeleteSupported ? softDeleteManager.GetNotDeletedCompiledLambda<TEntity>() : null;
		Func<TEntity, bool> filterCompiledLambda = entityLookupData.FilterCompiledLambda;

		var insertedAndUpdatedEntities = entityChanges
		.Where(change => (change.ChangeType == ChangeType.Update) || (change.ChangeType == ChangeType.Insert))
		.Select(change => (TEntity)change.Entity)
		.ToList();

		foreach (var entity in insertedAndUpdatedEntities)
		{
			// Když někdo založí příznakem smazanou entitu, pak ji použijeme tehdy, pokud používáme i deleted záznamy
			if (!IncludeDeleted && softDeleteSupported && !softDeleteCompiledLambda(entity))
			{
				// smazanou entitu nechceme evidovat
				continue;
			}

			if (filterCompiledLambda != null && !filterCompiledLambda(entity))
			{
				// entita neodpovídá filtru
				continue;
			}

			int entityKey = (int)entityKeyAccessor.GetEntityKeyValues(entity).Single();
			TLookupKey lookupKey = entityLookupData.LookupKeyCompiledLambda(entity);
			lock (entityLookupData)
			{
				entityLookupData.EntityKeyByLookupKeyDictionary.Add(lookupKey, entityKey);
				entityLookupData.LookupKeyByEntityKeyDictionary.Add(entityKey, lookupKey);
			}
		}
	}

	/// <summary>
	/// Vrátí název vlastnosti, která je reprezentována daným výrazem.
	/// </summary>
	/// <remarks>
	/// Duplicitní kód, avšak nechci jej veřejně sdílet jako extension metodu nebo tak).
	/// </remarks>
	internal string GetPropertyName(Expression item)
	{
		if (item is MemberExpression)
		{
			MemberExpression memberExpression = (MemberExpression)item;
			if (memberExpression.Expression is ParameterExpression)
			{
				return memberExpression.Member.Name;
			}
		}
		throw new NotSupportedException(item.ToString());
	}

	internal class TryGetEntityKeyByLookupKeyResult
	{
		public required bool Success { get; init; }
		public required int EntityKey { get; init; }
	}
}
