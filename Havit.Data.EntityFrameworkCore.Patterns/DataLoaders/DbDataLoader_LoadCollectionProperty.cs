using System.Reflection;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Internal;
using Havit.Data.Patterns.DataLoaders;
using Havit.Diagnostics.Contracts;
using Havit.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;

public partial class DbDataLoader
{
	/// <summary>
	/// Zajistí načtení vlastnosti, která je kolekcí. Voláno reflexí.
	/// </summary>
	private LoadPropertyInternalResult LoadCollectionPropertyInternal<TEntity, TPropertyCollection, TOriginalPropertyCollection, TPropertyItem>(string propertyName, string originalPropertyName, IEnumerable<TEntity> distinctNotNullEntities, string propertyPathString)
		where TEntity : class
		where TPropertyCollection : class
		where TOriginalPropertyCollection : class
		where TPropertyItem : class
	{
		ICollection<TEntity> entities = LoadPropertyInternal_EntitiesToCollectionOptimized(distinctNotNullEntities);

		if (entities.Count == 0)
		{
			_logger.LogDebug("No entity to retrieve data.");
			return LoadPropertyInternalResult.CreateEmpty<TPropertyItem>();
		}

		LoadCollectionPropertyInternal_GetFromCache<TEntity, TPropertyItem>(propertyName, entities, out List<TEntity> entitiesToLoadQuery);
		if ((entitiesToLoadQuery != null) && (entitiesToLoadQuery.Count > 0)) // zůstalo nám, na co se ptát do databáze?
		{
			_logger.LogDebug("Retrieving data for {Count} entities from the database...", args: entitiesToLoadQuery.Count);
			List<TPropertyItem> loadedProperties;
			if ((entitiesToLoadQuery.Count <= ChunkSize) || _dbContext.SupportsSqlServerOpenJson())
			{
				IQueryable<TPropertyItem> query = LoadCollectionPropertyInternal_GetQuery<TEntity, TPropertyItem>(entitiesToLoadQuery, propertyName, propertyPathString);
				loadedProperties = query.ToList();
			}
			else
			{
				// V případě velkého počtu identifikátorů ve WHERE xy IN (...) můžeme dostat chybu:
				// Internal error: An expression services limit has been reached.Please look for potentially complex expressions in your query, and try to simplify them.
				// https://learn.microsoft.com/en-us/sql/relational-databases/errors-events/mssqlserver-8632-database-engine-error?view=sql-server-ver16
				// (Vyskytlo se při přibližně 34000 hodnotách v Azure SQL Database, Elastic Pool 200 eDTU, každá databáze max. 100 DTU.)
				//
				// Snažíme se proto rozdělit identifikátory do skupin maximálně po ChunkSize prvcích.
				// Nevýhody:
				// - Pokud jsou identifikátory po sobě jdoucí, může se stát, že se podmínka WHERE XyId IN (...) zjednoduší na WHERE XyId >= 5001 and XyId <= 55000,  
				//   touto úpravou však dojde ke spuštění 10 dotazů, každý s ChunkSize prvky z daného rozsahu (vzhledem k pravděpodobnosti výskytu neřešíme
				//   více a považujeme toto za good-enough řešení).
				// - Víceré spuštění changetrackeru (ale když už použijeme takto enormní množství dat, changetracker nás nejspíš netrápí)
				// Zároveň se snažíme ani trochu nesnížit výkon pro běžný scénář s běžným počtem záznamů (nechceme přidat volání AddRange v dalších variantách této metody, atp.).

				int chunkIndex = 0;
				int chunksCount = (int)Math.Ceiling((decimal)entitiesToLoadQuery.Count / (decimal)ChunkSize);
				IEnumerable<IQueryable<TPropertyItem>> chunkQueries = entitiesToLoadQuery.Chunk(ChunkSize).Select(entitiesToLoadQueryChunk => LoadCollectionPropertyInternal_GetQuery<TEntity, TPropertyItem>(entitiesToLoadQueryChunk.ToList(), propertyName, propertyPathString));
				loadedProperties = new List<TPropertyItem>(entitiesToLoadQuery.Count);
				foreach (var chunkQuery in chunkQueries)
				{
					List<TPropertyItem> loadedPropertiesChunk = chunkQuery.TagWith($"Chunk {++chunkIndex}/{chunksCount}").ToList();
					loadedProperties.AddRange(loadedPropertiesChunk);
				}
			}
			_logger.LogDebug("Data for entities retrieved from the database.");
			LoadCollectionPropertyInternal_StoreCollectionsToCache<TEntity, TPropertyItem>(entitiesToLoadQuery, propertyName);
			LoadCollectionPropertyInternal_StoreEntitiesToCache<TPropertyItem>(loadedProperties);
		}

		LoadCollectionPropertyInternal_InitializeCollections<TEntity, TPropertyCollection, TPropertyItem>(entities, propertyName);
		LoadCollectionPropertyInternal_MarkAsLoaded(entities, propertyName); // potřebujeme označit všechny kolekce za načtené (načtené + založené prázdné + odbavené z cache)

		return LoadCollectionPropertyInternal_GetResult<TEntity, TOriginalPropertyCollection, TPropertyItem>(entities, originalPropertyName);
	}

	/// <summary>
	/// Zajistí načtení vlastnosti, která je kolekcí. Voláno reflexí.
	/// </summary>
	private async ValueTask<LoadPropertyInternalResult> LoadCollectionPropertyInternalAsync<TEntity, TPropertyCollection, TOriginalPropertyCollection, TPropertyItem>(string propertyName, string originalPropertyName, IEnumerable<TEntity> distinctNotNullEntities, string propertyPathString, CancellationToken cancellationToken /* no default */)
		where TEntity : class
		where TPropertyCollection : class
		where TOriginalPropertyCollection : class
		where TPropertyItem : class
	{
		ICollection<TEntity> entities = LoadPropertyInternal_EntitiesToCollectionOptimized(distinctNotNullEntities);

		if (entities.Count == 0)
		{
			_logger.LogDebug("No entity to retrieve data.");
			return LoadPropertyInternalResult.CreateEmpty<TPropertyItem>();
		}

		LoadCollectionPropertyInternal_GetFromCache<TEntity, TPropertyItem>(propertyName, entities, out List<TEntity> entitiesToLoadQuery);

		if ((entitiesToLoadQuery != null) && (entitiesToLoadQuery.Count > 0)) // zůstalo nám, na co se ptát do databáze?
		{
			_logger.LogDebug("Reading data for {Count} entities from the database...", args: entitiesToLoadQuery.Count);
			List<TPropertyItem> loadedProperties;
			if ((entitiesToLoadQuery.Count <= ChunkSize) || _dbContext.SupportsSqlServerOpenJson())
			{
				IQueryable<TPropertyItem> query = LoadCollectionPropertyInternal_GetQuery<TEntity, TPropertyItem>(entitiesToLoadQuery, propertyName, propertyPathString);
				loadedProperties = await query.ToListAsync(cancellationToken).ConfigureAwait(false);
			}
			else
			{
				// viz komentář v LoadCollectionPropertyInternal
				int chunkIndex = 0;
				int chunksCount = (int)Math.Ceiling((decimal)entitiesToLoadQuery.Count / (decimal)ChunkSize);
				IEnumerable<IQueryable<TPropertyItem>> chunkQueries = entitiesToLoadQuery.Chunk(ChunkSize).Select(entitiesToLoadQueryChunk => LoadCollectionPropertyInternal_GetQuery<TEntity, TPropertyItem>(entitiesToLoadQueryChunk.ToList(), propertyName, propertyPathString));
				loadedProperties = new List<TPropertyItem>(entitiesToLoadQuery.Count);
				foreach (var chunkQuery in chunkQueries)
				{
					List<TPropertyItem> loadedPropertiesChunk = await chunkQuery.TagWith($"Chunk {++chunkIndex}/{chunksCount}").ToListAsync(cancellationToken).ConfigureAwait(false);
					loadedProperties.AddRange(loadedPropertiesChunk);
				}
			}
			_logger.LogDebug("Data for entities read from the database.");

			LoadCollectionPropertyInternal_StoreCollectionsToCache<TEntity, TPropertyItem>(entitiesToLoadQuery, propertyName);
			LoadCollectionPropertyInternal_StoreEntitiesToCache<TPropertyItem>(loadedProperties);
		}

		LoadCollectionPropertyInternal_InitializeCollections<TEntity, TPropertyCollection, TPropertyItem>(entities, propertyName);
		LoadCollectionPropertyInternal_MarkAsLoaded(entities, propertyName); // potřebujeme označit všechny kolekce za načtené (načtené + založené prázdné + odbavené z cache)

		return LoadCollectionPropertyInternal_GetResult<TEntity, TOriginalPropertyCollection, TPropertyItem>(entities, originalPropertyName);
	}

	/// <summary>
	/// Zkouší načíst objekty z cache.
	/// Klíče objektů, které se nepodařilo načíst z cache, nastavuje out parametru.
	/// Aktuálně s cache nic nedělá, do out parametru vrací všechny entity.
	/// </summary>
	private void LoadCollectionPropertyInternal_GetFromCache<TEntity, TPropertyItem>(string propertyName, ICollection<TEntity> entities, out List<TEntity> entitiesToLoadQuery)
		where TEntity : class
		where TPropertyItem : class
	{
		// TODO EF Core: Má tento list a jeho duplikace o pár řádek níže smysl?

		List<TEntity> entitiesToLoad = entities.Where(entity => !IsEntityPropertyLoaded(entity, propertyName)).Where(item => _dbContext.GetEntityState(item) != EntityState.Added).ToList();

		if (entitiesToLoad.Count == 0)
		{
			_logger.LogDebug("No entity to retrieve data (previously loaded).");
			entitiesToLoadQuery = null;
			return;
		}

		_logger.LogDebug("Retrieving data for {Count} entities from the cache...", entities.Count);
		entitiesToLoadQuery = new List<TEntity>(entitiesToLoad.Count);
		int cacheHitCounter = 0;
		foreach (var entityToLoad in entitiesToLoad)
		{
			if (_entityCacheManager.TryGetNavigation<TEntity, TPropertyItem>(entityToLoad, propertyName))
			{
				cacheHitCounter += 1;
			}
			else
			{
				entitiesToLoadQuery.Add(entityToLoad);
			}
		}
		_logger.LogDebug("Retrieved data for {Count} entities from the cache.", cacheHitCounter);

	}

	/// <summary>
	/// Vrátí dotaz načítající vlastnosti objektů pro dané primární klíče.
	/// </summary>
	private IQueryable<TProperty> LoadCollectionPropertyInternal_GetQuery<TEntity, TProperty>(List<TEntity> entitiesToLoad, string propertyName, string propertyPathString)
		where TEntity : class
		where TProperty : class
	{
		// Performance: No big issue.
		var foreignKeyProperty = _dbContext.Model.FindEntityType(typeof(TEntity)).FindNavigation(propertyName).ForeignKey.Properties.Single();

		Contract.Assert<InvalidOperationException>(foreignKeyProperty.ClrType == typeof(int) || foreignKeyProperty.ClrType == typeof(int?));

		List<int> primaryKeysToLoad = entitiesToLoad.Select(entityToLoad => _entityKeyAccessor.GetEntityKeyValues(entityToLoad).Single()).Cast<int>().ToList();
		return _dbContext.Set<TProperty>()
			.AsQueryable($"{nameof(DbDataLoader)} ({typeof(TEntity).Name} {propertyPathString})")

			// V EF Core 2.x a 3.x bez následujícího řádku mohlo při vykonávání dotazu dojít k System.InvalidOperationException: Objekt povolující hodnotu Null musí mít hodnotu.
			// V EF Core 5.x opraveno, již není třeba.
			// Chování chráněno testy DbDataLoader_Load_Collection_SupportsNullableForeignKeysInMemory a DbDataLoader_Load_Collection_SupportsNullableForeignKeysInDatabase.
			//.WhereIf(foreignKeyProperty.ClrType == typeof(int?), item => null != EF.Property<int?>(item, foreignKeyProperty.Name))
			// Nyní musíme ověřit, zda pracujeme s int nebo Nullable<int>, přičemž pro Nullable<int> potřebujeme zvláštní péči:
			// Zkompilovat a spustit jde pro oba případy jen varianta s int, avšak v runtime způsobí client-side evaluation (podmínka se nedostane do dotazu, ale je vyhodnocena entity frameworkem), což z výkonových důvodů opravdu nechceme.
			// Proto doplníme variantu pro int?, která tento problém vyřeší.
			// Toto chování není chráněno žádným testem.
			.WhereIf(foreignKeyProperty.ClrType == typeof(int?), primaryKeysToLoad.ContainsEffective<TProperty>(item => (int)EF.Property<int?>(item, foreignKeyProperty.Name)))
			.WhereIf(foreignKeyProperty.ClrType == typeof(int), primaryKeysToLoad.ContainsEffective<TProperty>(item => EF.Property<int>(item, foreignKeyProperty.Name)));
	}

	private void LoadCollectionPropertyInternal_StoreCollectionsToCache<TEntity, TPropertyItem>(List<TEntity> loadedEntities, string propertyName)
		where TEntity : class
		where TPropertyItem : class
	{
		// TODO EF Core 9: if cachable

		_logger.LogDebug("Storing navigations to cache...");

		// pozor, property zde ještě může být null
		foreach (TEntity loadedEntity in loadedEntities)
		{
			_entityCacheManager.StoreNavigation<TEntity, TPropertyItem>(loadedEntity, propertyName);
		}

		_logger.LogDebug("Navigations stored to cache.");
	}

	private void LoadCollectionPropertyInternal_StoreEntitiesToCache<TProperty>(List<TProperty> loadedProperties)
		where TProperty : class
	{
		if (!_entityCacheManager.ShouldCacheEntityType<TProperty>())
		{
			return;
		}

		// TODO: Test na ManyToMany
		if (_entityKeyAccessor.GetEntityKeyPropertyNames(typeof(TProperty)).Length == 1)
		{
			_logger.LogDebug("Storing entities to cache...");
			// uložíme do cache, pokud je cachovaná
			foreach (TProperty loadedEntity in loadedProperties)
			{
				_entityCacheManager.StoreEntity(loadedEntity);
			}
			_logger.LogDebug("Entities stored to cache.");
		}
	}

	/// <summary>
	/// Tato metoda inicializuje kolekce (nastaví nové instance), pokud jsou null.
	/// </summary>
	private void LoadCollectionPropertyInternal_InitializeCollections<TEntity, TPropertyCollection, TPropertyItem>(ICollection<TEntity> entities, string propertyName)
		where TEntity : class
		where TPropertyCollection : class
		where TPropertyItem : class
	{
		_logger.LogDebug("Initializing collections...");

		var propertyLambda = _lambdaExpressionManager.GetPropertyLambdaExpression<TEntity, TPropertyCollection>(propertyName).LambdaCompiled;
		IEnumerable<TEntity> entitiesWithNullReference = entities.Where(item => propertyLambda(item) == null);

		var entitiesWithNullReferenceEnumerator = entitiesWithNullReference.GetEnumerator();
		if (entitiesWithNullReferenceEnumerator.MoveNext()) // == .Any() s jednou iterací
		{
			if (typeof(TPropertyCollection) == typeof(List<TPropertyItem>) || typeof(TPropertyCollection) == typeof(IList<TPropertyItem>))
			{
				MethodInfo setter = typeof(TEntity).GetProperty(propertyName).GetSetMethod();
				if (setter == null)
				{
					throw new InvalidOperationException($"DataLoader cannot set collection property {propertyName} on type {{typeof(TEntity).FullName}} while it does not have a public setter.");
				}

				var setterArgument = new object[1];
				do
				{
					setterArgument[0] = new List<TPropertyItem>(); // nastavujeme nové pole
					setter.Invoke(entitiesWithNullReferenceEnumerator.Current, setterArgument);
				} while (entitiesWithNullReferenceEnumerator.MoveNext());
			}
			else
			{
				throw new InvalidOperationException($"DataLoader cannot set collection property {propertyName} on type {typeof(TEntity).FullName} while it is not type of List<{typeof(TPropertyItem).Name}> or IList<{typeof(TPropertyItem).Name}.");
			}
		}

		_logger.LogDebug("Collections initialized.");
	}

	/// <summary>
	/// Označí entitám vlatnost propertyName jako načtenou.
	/// </summary>
	private void LoadCollectionPropertyInternal_MarkAsLoaded<TEntity>(ICollection<TEntity> entities, string propertyName)
		where TEntity : class
	{
		_logger.LogDebug("Marking properties as loaded...");

		foreach (var entity in entities)
		{
			_dbContext.MarkNavigationAsLoaded(entity, propertyName);
		}

		_logger.LogDebug("Properties marked as loaded.");
	}

	private LoadPropertyInternalResult LoadCollectionPropertyInternal_GetResult<TEntity, TOriginalPropertyCollection, TPropertyItem>(ICollection<TEntity> entities, string originalPropertyName)
		where TEntity : class
		where TOriginalPropertyCollection : class
		where TPropertyItem : class
	{
		var originalPropertyLambda = _lambdaExpressionManager.GetPropertyLambdaExpression<TEntity, IEnumerable<TPropertyItem>>(originalPropertyName).LambdaCompiled;

		IEnumerable<TPropertyItem> loadedEntities = entities.SelectMany(item => (IEnumerable<TPropertyItem>)originalPropertyLambda(item));
		return new LoadPropertyInternalResult
		{
			// Zde předáváme dvakrát IEnumerable<TPropertyItem>, ale efektivně bude použit nejvýše jeden z nich.
			// Entities v dalším průchodu foreachem v LoadInternal[Async] (pokud nenásleduje další průchod, nebude kolekce nikdy zpracována)
			// FluentDataLoader v dalším ThanLoad (pokud nenásleduje ThenLoad[Async], nebude kolekce nikdy zpracována)
			Entities = loadedEntities,
			FluentDataLoader = new FluentDataLoader<TOriginalPropertyCollection, TPropertyItem>(this, loadedEntities)
		};
	}
}
