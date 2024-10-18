using System.Linq.Expressions;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.Patterns.DataLoaders;
using Havit.Data.Patterns.Infrastructure;
using Havit.Data.Patterns.Repositories;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Patterns.Repositories;

/// <summary>
/// Repository objektů typu TEntity.
/// </summary>
public abstract class DbRepository<TEntity> : IRepository<TEntity>
	 where TEntity : class
{
	internal const int GetObjectsChunkSize = 5_000;

	private readonly IDbContext dbContext;
	private readonly IEntityKeyAccessor<TEntity, int> entityKeyAccessor;
	private readonly IRepositoryQueryProvider repositoryQueryProvider;

	/// <summary>
	/// DataLoader pro případné využití v implementaci potomků.
	/// </summary>
	/// <remarks>
	/// Bohužel používáme generovaný constructor v partial třídě, takže není úplně jednoduché si v potomkovi dataLoader odklonit pro vlastní použití.
	/// </remarks>
	protected IDataLoader DataLoader => dataLoader;
	private readonly IDataLoader dataLoader;

	/// <summary>
	/// DbSet, nad kterým je DbRepository postaven.
	/// </summary>
	protected IDbSet<TEntity> DbSet => dbSetLazy.Value;

	/// <summary>
	/// Implementačně jako Lazy, aby kontruktor nevyzvedával DbSet. To umožňuje psát unit testy s mockem dbContextu bez setupu metody Set (dbContext nemusí nic umět).
	/// </summary>
	private readonly Lazy<IDbSet<TEntity>> dbSetLazy;

	/// <summary>
	/// Vrací data z datového zdroje jako IQueryable.
	/// Pokud zdroj obsahuje záznamy smazané příznakem, jsou odfiltrovány (nejsou v datech).
	/// </summary>
	protected IQueryable<TEntity> Data => DbSet.AsQueryable(QueryTagBuilder.CreateTag(this.GetType(), nameof(Data))).WhereNotDeleted(SoftDeleteManager);

	/// <summary>
	/// Vrací data z datového zdroje jako IQueryable.
	/// Pokud zdroj obsahuje záznamy smazané příznakem, jsou součástí dat.
	/// </summary>
	protected IQueryable<TEntity> DataIncludingDeleted => DbSet.AsQueryable(QueryTagBuilder.CreateTag(this.GetType(), nameof(DataIncludingDeleted)));

	/// <summary>
	/// SoftDeleteManager používaný repository.
	/// </summary>
	protected ISoftDeleteManager SoftDeleteManager { get; }

	/// <summary>
	/// EntityCacheManager používaný repository.
	/// </summary>
	protected IEntityCacheManager EntityCacheManager { get; }

	/// <summary>
	/// Konstruktor.
	/// </summary>
	protected DbRepository(IDbContext dbContext, IEntityKeyAccessor<TEntity, int> entityKeyAccessor, IDataLoader dataLoader, ISoftDeleteManager softDeleteManager, IEntityCacheManager entityCacheManager, IRepositoryQueryProvider repositoryQueryProvider)
	{
		this.dbContext = dbContext;
		this.entityKeyAccessor = entityKeyAccessor;
		this.dataLoader = dataLoader;
		this.SoftDeleteManager = softDeleteManager;
		this.EntityCacheManager = entityCacheManager;
		this.repositoryQueryProvider = repositoryQueryProvider;
		this.dbSetLazy = new Lazy<IDbSet<TEntity>>(() => dbContext.Set<TEntity>(), LazyThreadSafetyMode.None);
	}

	/// <summary>
	/// Vrací objekt dle Id.
	/// Objekt má načtené vlastnosti definované v metodě GetLoadReferences. 
	/// </summary>
	/// <exception cref="Havit.Data.Patterns.Exceptions.ObjectNotFoundException">Objekt s daným Id nebyl nalezen.</exception>
	public TEntity GetObject(int id)
	{
		Contract.Requires<ArgumentException>(id != default(int));

		// hledáme v identity mapě
		TEntity result = DbSet.FindTracked(id);

		// není v identity mapě, hledáme v cache
		if (result == null)
		{
			EntityCacheManager.TryGetEntity<TEntity>(id, out result);
		}

		// není ani v identity mapě, ani v cache, hledáme v databázi
		if (result == null)
		{
			Func<DbContext, int, TEntity> query = repositoryQueryProvider.GetGetObjectCompiledQuery<TEntity>(this.GetType(), entityKeyAccessor);
			result = query((DbContext)dbContext, id);

			if (result != null)
			{
				// načtený objekt uložíme do cache
				EntityCacheManager.StoreEntity(result);
			}
		}

		if (result == null)
		{
			ThrowObjectNotFoundException(id);
		}

		LoadReferences([result]);
		return result;
	}

	/// <summary>
	/// Vrací objekt dle Id.
	/// Objekt má načtené vlastnosti definované v metodě GetLoadReferences. 
	/// </summary>
	/// <exception cref="Havit.Data.Patterns.Exceptions.ObjectNotFoundException">Objekt s daným Id nebyl nalezen.</exception>
	public async Task<TEntity> GetObjectAsync(int id, CancellationToken cancellationToken = default)
	{
		Contract.Requires<ArgumentException>(id != default(int));

		TEntity result = DbSet.FindTracked(id);

		// není v identity mapě, hledáme v cache
		if (result == null)
		{
			EntityCacheManager.TryGetEntity<TEntity>(id, out result);
		}

		// není ani v identity mapě, ani v cache, hledáme v databázi
		if (result == null)
		{
			Func<DbContext, int, CancellationToken, Task<TEntity>> query = repositoryQueryProvider.GetGetObjectAsyncCompiledQuery<TEntity>(this.GetType(), entityKeyAccessor);
			result = await query((DbContext)dbContext, id, cancellationToken).ConfigureAwait(false);

			if (result != null)
			{
				// načtený objekt uložíme do cache
				EntityCacheManager.StoreEntity(result);
			}
		}

		if (result == null)
		{
			ThrowObjectNotFoundException(id);
		}

		await LoadReferencesAsync([result], cancellationToken).ConfigureAwait(false);
		return result;
	}

	/// <summary>
	/// Vrací instance objektů dle Id.
	/// Vrací instance objektů dle Id.
	/// Objekty mají načtené vlastnosti definované v metodě GetLoadReferences. 
	/// </summary>
	/// <exception cref="Havit.Data.Patterns.Exceptions.ObjectNotFoundException">Alespoň jeden objekt nebyl nalezen.</exception>
	public List<TEntity> GetObjects(params int[] ids)
	{
		Contract.Requires<ArgumentNullException>(ids != null, nameof(ids));

		HashSet<TEntity> loadedEntities = new HashSet<TEntity>();
		HashSet<int> idsToLoad = new HashSet<int>();

		foreach (int id in ids)
		{
			Contract.Assert<ArgumentException>(id != default(int));

			TEntity trackedEntity = DbSet.FindTracked(id);
			if (trackedEntity != null)
			{
				loadedEntities.Add(trackedEntity);
			}
			// není v identity mapě, hledáme v cache
			else if (EntityCacheManager.TryGetEntity<TEntity>(id, out TEntity cachedEntity))
			{
				loadedEntities.Add(cachedEntity);
			}
			else // není ani v identity mapě, ani v cache, hledáme v databázi
			{
				idsToLoad.Add(id);
			}
		}

		List<TEntity> result = new List<TEntity>(ids.Length);
		result.AddRange(loadedEntities);

		if (idsToLoad.Count > 0)
		{
			Func<DbContext, int[], IEnumerable<TEntity>> query = repositoryQueryProvider.GetGetObjectsCompiledQuery(this.GetType(), entityKeyAccessor);

			List<TEntity> loadedObjects;
			if ((idsToLoad.Count <= GetObjectsChunkSize) || dbContext.SupportsSqlServerOpenJson())
			{
				loadedObjects = query((DbContext)dbContext, idsToLoad.ToArray()).ToList();
			}
			else
			{
				loadedObjects = new List<TEntity>(idsToLoad.Count);
				foreach (int[] idsToLoadChunk in idsToLoad.Chunk(GetObjectsChunkSize))
				{
					loadedObjects.AddRange(query((DbContext)dbContext, idsToLoadChunk).ToList());
				}
			}

			if (idsToLoad.Count != loadedObjects.Count)
			{
				int[] missingObjectIds = idsToLoad.Except(loadedObjects.Select(entityKeyAccessor.GetEntityKeyValue)).ToArray();
				ThrowObjectNotFoundException(missingObjectIds);
			}

			// načtené objekty uložíme do cache
			foreach (TEntity loadedObject in loadedObjects)
			{
				EntityCacheManager.StoreEntity(loadedObject);
			}

			result.AddRange(loadedObjects);
		}

		LoadReferences(result);
		return result;
	}

	/// <summary>
	/// Vrací instance objektů dle Id.
	/// Objekty mají načtené vlastnosti definované v metodě GetLoadReferences. 
	/// </summary>
	/// <exception cref="Havit.Data.Patterns.Exceptions.ObjectNotFoundException">Alespoň jeden objekt nebyl nalezen.</exception>
	public async Task<List<TEntity>> GetObjectsAsync(int[] ids, CancellationToken cancellationToken = default)
	{
		Contract.Requires<ArgumentException>(ids != null, nameof(ids));

		HashSet<TEntity> loadedEntities = new HashSet<TEntity>();
		HashSet<int> idsToLoad = new HashSet<int>();

		foreach (int id in ids)
		{
			Contract.Assert<ArgumentException>(id != default(int));

			TEntity trackedEntity = DbSet.FindTracked(id);
			if (trackedEntity != null)
			{
				loadedEntities.Add(trackedEntity);
			}
			// není v identity mapě, hledáme v cache
			else if (EntityCacheManager.TryGetEntity<TEntity>(id, out TEntity cachedEntity))
			{
				loadedEntities.Add(cachedEntity);
			}
			else // není ani v identity mapě, ani v cache, hledáme v databázi
			{
				idsToLoad.Add(id);
			}
		}

		List<TEntity> result = new List<TEntity>(ids.Length);
		result.AddRange(loadedEntities);

		if (idsToLoad.Count > 0)
		{
			Func<DbContext, int[], IAsyncEnumerable<TEntity>> query = repositoryQueryProvider.GetGetObjectsAsyncCompiledQuery<TEntity>(this.GetType(), entityKeyAccessor);
			List<TEntity> loadedObjects;
			if ((idsToLoad.Count <= GetObjectsChunkSize) || dbContext.SupportsSqlServerOpenJson())
			{
				loadedObjects = await query((DbContext)dbContext, idsToLoad.ToArray()).ToListAsync(cancellationToken).ConfigureAwait(false);
			}
			else
			{
				loadedObjects = new List<TEntity>(idsToLoad.Count);
				foreach (int[] idsToLoadChunk in idsToLoad.Chunk(GetObjectsChunkSize))
				{
					loadedObjects.AddRange(await query((DbContext)dbContext, idsToLoadChunk).ToListAsync(cancellationToken).ConfigureAwait(false));
				}
			}

			if (idsToLoad.Count != loadedObjects.Count)
			{
				int[] missingObjectIds = idsToLoad.Except(loadedObjects.Select(entityKeyAccessor.GetEntityKeyValue)).ToArray();
				ThrowObjectNotFoundException(missingObjectIds);
			}

			// načtené objekty uložíme do cache
			foreach (TEntity loadedObject in loadedObjects)
			{
				EntityCacheManager.StoreEntity(loadedObject);
			}

			result.AddRange(loadedObjects);
		}

		await LoadReferencesAsync(result, cancellationToken).ConfigureAwait(false);
		return result;
	}

	/// <summary>
	/// Vrací seznam všech (příznakem nesmazaných) objektů typu TEntity.
	/// Dotaz na seznam objektů provede jednou, při opakovaném volání vrací data z paměti.
	/// Objekty mají načtené vlastnosti definované v metodě GetLoadReferences. 
	/// </summary>
	public List<TEntity> GetAll()
	{
		if (_all == null)
		{
			TEntity[] allData;

			// máme v cache klíče, která chceme načítat?
			if (EntityCacheManager.TryGetAllKeys<TEntity>(out object keys))
			{
				// pokud ano, načteme je přes GetObjects (což umožní využití cache pro samotné entity)
				allData = GetObjects((int[])keys).ToArray();
			}
			else
			{
				// pokud ne, načtene data a uložíme data a klíče do cache
				Func<DbContext, IEnumerable<TEntity>> query = repositoryQueryProvider.GetGetAllCompiledQuery<TEntity>(this.GetType(), SoftDeleteManager);
				allData = query((DbContext)dbContext).ToArray();

				EntityCacheManager.StoreAllKeys<TEntity>(allData.Select(entity => entityKeyAccessor.GetEntityKeyValue(entity)).ToArray());
				foreach (var entity in allData) // performance: Pokud již objekty jsou v cache je jejich ukládání do cache zbytečné. Pro většinový scénář však nemáme ani klíče ani entity v cache, proto je jejich uložení do cache na místě).
				{
					EntityCacheManager.StoreEntity<TEntity>(entity);
				}
			}

			LoadReferences(allData);

			_all = allData;
			dbContext.RegisterAfterSaveChangesAction(() =>
			{
				_all = null;
			});
		}
		return new List<TEntity>(_all);
	}

	/// <summary>
	/// Vrací seznam všech (příznakem nesmazaných) objektů typu TEntity.
	/// Dotaz na seznam objektů provede jednou, při opakovaném volání vrací data z paměti.
	/// Objekty mají načtené vlastnosti definované v metodě GetLoadReferences. 
	/// </summary>
	public async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		if (_all == null)
		{
			TEntity[] allData;

			// máme v cache klíče, která chceme načítat?
			if (EntityCacheManager.TryGetAllKeys<TEntity>(out object keys))
			{
				// pokud ano, načteme je přes GetObjects (což umožní využití cache pro samotné entity)
				allData = (await GetObjectsAsync((int[])keys, cancellationToken).ConfigureAwait(false)).ToArray();
			}
			else
			{
				// pokud ne, načtene data a uložíme klíče do cache
				Func<DbContext, IAsyncEnumerable<TEntity>> query = repositoryQueryProvider.GetGetAllAsyncCompiledQuery<TEntity>(this.GetType(), SoftDeleteManager);
				allData = await query((DbContext)dbContext).ToArrayAsync(cancellationToken).ConfigureAwait(false);
				EntityCacheManager.StoreAllKeys<TEntity>(allData.Select(entity => entityKeyAccessor.GetEntityKeyValue(entity)).ToArray());
			}
			await LoadReferencesAsync(allData, cancellationToken).ConfigureAwait(false);

			_all = allData;
			dbContext.RegisterAfterSaveChangesAction(() =>
			{
				_all = null;
			});
		}
		return new List<TEntity>(_all);
	}

	private TEntity[] _all;

	/// <summary>
	/// Zajistí načtení vlastností definovaných v meodě GetLoadReferences.
	/// </summary>
	/// <remarks>
	/// Metodu lze overridovat, pokud chceme doplnit podrobnější implementaci dočítání (přes IDataLoader), např. nepodporované dočítání prvků v kolekcích.
	/// Nezapomeňte však overridovat synchronní i asynchronní verzi! Jsou to nezávislé implementace...
	/// </remarks>
	protected virtual void LoadReferences(IEnumerable<TEntity> entities)
	{
		Contract.Requires<ArgumentNullException>(entities != null, nameof(entities));

		var loadReferences = GetLoadReferences();
		// Výrazně nejčastější scénář je, že nemáme žádné references (vrací se enumerable.empty) a porovnání referencí je nejrychlejší.
		if (loadReferences == Enumerable.Empty<Expression<Func<TEntity, object>>>())
		{
			return;
		}

		dataLoader.LoadAll(entities, loadReferences.ToArray());
	}

	/// <summary>
	/// Zajistí načtení vlastností definovaných v meodě GetLoadReferences.
	/// </summary>
	/// <remarks>
	/// Metodu lze overridovat, pokud chceme doplnit podrobnější implementaci dočítání (přes IDataLoader), např. nepodporované dočítání prvků v kolekcích.
	/// Nezapomeňte však overridovat synchronní i asynchronní verzi! Jsou to nezávislé implementace...
	/// </remarks>
	protected virtual async Task LoadReferencesAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
	{
		Contract.Requires<ArgumentNullException>(entities != null, nameof(entities));

		var loadReferences = GetLoadReferences();
		// Výrazně nejčastější scénář je, že nemáme žádné references (vrací se enumerable.empty) a porovnání referencí je nejrychlejší.
		if (loadReferences == Enumerable.Empty<Expression<Func<TEntity, object>>>())
		{
			return;
		}

		await dataLoader.LoadAllAsync(entities, loadReferences.ToArray(), cancellationToken).ConfigureAwait(false);
	}

	/// <summary>
	/// Vrací expressions určující, které vlastnosti budou s objektem načteny.
	/// Načítání prování DbDataLoader.
	/// </summary>
	protected virtual IEnumerable<Expression<Func<TEntity, object>>> GetLoadReferences()
	{
		return Enumerable.Empty<Expression<Func<TEntity, object>>>();
	}

	private void ThrowObjectNotFoundException(params int[] missingIds)
	{
		Contract.Requires<ArgumentNullException>(missingIds != null, nameof(missingIds));
		Contract.Requires<ArgumentException>(missingIds.Length > 0);

		string exceptionText = (missingIds.Length == 1)
			? String.Format("Object {0} with key {1} not found.", typeof(TEntity).Name, missingIds[0])
			: String.Format("Objects {0} with keys {1} not found.", typeof(TEntity).Name, String.Join(", ", missingIds.Select(item => item.ToString())));

		throw new Havit.Data.Patterns.Exceptions.ObjectNotFoundException(exceptionText);
	}

}