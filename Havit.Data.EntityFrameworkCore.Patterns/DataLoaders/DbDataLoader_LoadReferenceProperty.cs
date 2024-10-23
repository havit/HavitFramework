using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Internal;
using Havit.Data.Patterns.DataLoaders;
using Havit.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;

public partial class DbDataLoader
{
	/// <summary>
	/// Zajistí načtení vlastnosti, která je referencí (není kolkecí). Voláno reflexí.
	/// </summary>
	private LoadPropertyInternalResult LoadReferencePropertyInternal<TEntity, TProperty>(string propertyName, IEnumerable<TEntity> distinctNotNullEntities)
		where TEntity : class
		where TProperty : class
	{
		ICollection<TEntity> entities = LoadPropertyInternal_EntitiesToCollectionOptimized(distinctNotNullEntities);

		LogDebug("Retrieving data for {0} entities from the cache.", args: entities.Count);
		LoadReferencePropertyInternal_GetFromCache<TEntity, TProperty>(propertyName, entities, out List<object> foreignKeysToLoad);

		if ((foreignKeysToLoad != null) && foreignKeysToLoad.Count > 0) // zůstalo nám, na co se ptát do databáze?
		{
			LogDebug("Trying to retrieve data for {0} entities from the database.", args: foreignKeysToLoad.Count);

			List<TProperty> loadedProperties;
			if ((foreignKeysToLoad.Count < ChunkSize) || dbContext.SupportsSqlServerOpenJson())
			{
				IQueryable<TProperty> query = LoadReferencePropertyInternal_GetQuery<TProperty>(foreignKeysToLoad);
				LogDebug("Starting reading from a database.");
				loadedProperties = query.ToList();
			}
			else
			{
				// viz komentář v LoadCollectionPropertyInternal
				int chunksCount = (int)Math.Ceiling((decimal)foreignKeysToLoad.Count / (decimal)ChunkSize);
				IEnumerable<IQueryable<TProperty>> chunkQueries = foreignKeysToLoad.Chunk(ChunkSize).Select(foreignKeysToLoadChunk => LoadReferencePropertyInternal_GetQuery<TProperty>(foreignKeysToLoadChunk.ToList()));
				LogDebug("Starting reading chunks from a database.");

				loadedProperties = new List<TProperty>(foreignKeysToLoad.Count);
				int chunkIndex = 0;
				foreach (var chunkQuery in chunkQueries)
				{
					List<TProperty> loadedPropertiesChunk = chunkQuery.TagWith($"Chunk {++chunkIndex}/{chunksCount}").ToList();
					loadedProperties.AddRange(loadedPropertiesChunk);
				}
			}
			LogDebug("Finished reading from a database.");

			LogDebug("Storing data for {0} entities to the cache.", args: loadedProperties.Count);
			LoadReferencePropertyInternal_StoreToCache(loadedProperties);
		}

		LogDebug("Returning.");
		return LoadReferencePropertyInternal_GetResult<TEntity, TProperty>(propertyName, entities);
	}

	/// <summary>
	/// Zajistí načtení vlastnosti, která je referencí (není kolkecí). Voláno reflexí.
	/// </summary>
	private async ValueTask<LoadPropertyInternalResult> LoadReferencePropertyInternalAsync<TEntity, TProperty>(string propertyName, IEnumerable<TEntity> distinctNotNullEntities, CancellationToken cancellationToken /* no default */)
		where TEntity : class
		where TProperty : class
	{
		ICollection<TEntity> entities = LoadPropertyInternal_EntitiesToCollectionOptimized(distinctNotNullEntities);

		LogDebug("Retrieving data for {0} entities from the cache.", args: entities.Count);
		LoadReferencePropertyInternal_GetFromCache<TEntity, TProperty>(propertyName, entities, out List<object> foreignKeysToLoad);

		if ((foreignKeysToLoad != null) && foreignKeysToLoad.Count > 0) // zůstalo nám, na co se ptát do databáze?
		{
			LogDebug("Trying to retrieve data for {0} entities from the database.", args: foreignKeysToLoad.Count);

			List<TProperty> loadedProperties;
			if ((foreignKeysToLoad.Count < ChunkSize) || dbContext.SupportsSqlServerOpenJson())
			{
				IQueryable<TProperty> query = LoadReferencePropertyInternal_GetQuery<TProperty>(foreignKeysToLoad);
				LogDebug("Starting reading from a database.");
				loadedProperties = await query.ToListAsync(cancellationToken).ConfigureAwait(false);
			}
			else
			{
				// viz komentář v LoadCollectionPropertyInternal
				// TODO EF Core 9: Odstranit ToList
				List<IQueryable<TProperty>> chunkQueries = foreignKeysToLoad.Chunk(ChunkSize).Select(foreignKeysToLoadChunk => LoadReferencePropertyInternal_GetQuery<TProperty>(foreignKeysToLoadChunk.ToList())).ToList();
				LogDebug("Starting reading chunks from a database.");

				loadedProperties = new List<TProperty>(foreignKeysToLoad.Count);
				for (int chunkIndex = 0; chunkIndex < chunkQueries.Count; chunkIndex++)
				{
					var chunkQuery = chunkQueries[chunkIndex];
					List<TProperty> loadedPropertiesChunk = await chunkQuery.TagWith($"Chunk {chunkIndex + 1}/{chunkQueries.Count}").ToListAsync(cancellationToken).ConfigureAwait(false);
					loadedProperties.AddRange(loadedPropertiesChunk);
				}
			}
			LogDebug("Finished reading from a database.");

			LogDebug("Storing data for {0} entities to the cache.", args: loadedProperties.Count);
			LoadReferencePropertyInternal_StoreToCache(loadedProperties);
		}

		LogDebug("Returning.");
		return LoadReferencePropertyInternal_GetResult<TEntity, TProperty>(propertyName, entities);
	}

	private void LoadReferencePropertyInternal_GetFromCache<TEntity, TProperty>(string propertyName, ICollection<TEntity> entities, out List<object> foreignKeysToLoad)
		where TEntity : class
		where TProperty : class
	{
		List<TEntity> entitiesToLoadReference = entities.Where(entity => !IsEntityPropertyLoaded(entity, propertyName)).ToList();

		if (entitiesToLoadReference.Count == 0)
		{
			foreignKeysToLoad = null;
			return;
		}

		// získáme cizí klíč reprezentující referenci (Navigation)
		IProperty foreignKeyForReference = dbContext.Model.FindEntityType(typeof(TEntity)).FindNavigation(propertyName).ForeignKey.Properties.Single();

		// získáme klíče objektů, které potřebujeme načíst (z "běžných vlastností" nebo z shadow properties)
		// ignorujeme nenastavené reference (null)
		IEnumerable<object> foreignKeyValues = entitiesToLoadReference.Select(entity => dbContext.GetEntry(entity, true).CurrentValues[foreignKeyForReference]).Where(value => value != null).Distinct();

		IDbSet<TProperty> dbSet = dbContext.Set<TProperty>();

		bool shouldFixup = false;
		foreignKeysToLoad = new List<object>(entitiesToLoadReference.Count);

		foreach (object foreignKeyValue in foreignKeyValues)
		{
			// Čistě teoreticky nemusela dosud proběhnout detekce změn (resp. fixup), proto se musíme podívat do identity map před tím,
			// než budeme řešit cache (cache by se mohla pokoušet o vytažení objektu, který je již v identity mapě a došlo by ke kolizi).
			// Spoléháme na provedení fixupu pomocí changetrackeru.

			// K této situaci dojde, pokud je objekt nejprve zaregistrován do changetrackeru (třeba přidáním do UnitOfWork/DbContextu)
			// a následně je mu nastavena hodnota cizího klíče (Id objektu), avšak nikoliv nastavení navigation property.
			TProperty trackedEntity = dbSet.FindTracked(foreignKeyValue);
			if (trackedEntity != null)
			{
				shouldFixup = true;
			}
			else if (entityCacheManager.TryGetEntity<TProperty>(foreignKeyValue, out TProperty cachedEntity))
			{
				// NOOP
			}
			else // není ani v identity mapě, ani v cache, hledáme v databázi
			{
				foreignKeysToLoad.Add(foreignKeyValue);
			}
		}

		// Detekovali jsme, že některá z vlastností nebyla načtena, ale hodnota cizího klíče má hodnotu, která již je v identity mapě.
		// Proto provedeme fixup. AFAIK, implementačně nemáme lepší možnosti, než zavolat DetectChanges.
		// Teoreticky bychom měli detekci změn volat v ještě menší míře - maximálně jednou na volání Loadu, avšak pro minimální výskyt problému necháváme v této podobě.
		// Known issue: Property bude i nadále považována za nenačtenou. Avšak díky metodě v IsEntityPropertyLoaded v DbDataLoaderWithLoadedPropertiesMemory nebude docházet k opakovanému zpracování této vlastnosti.
		if (shouldFixup)
		{
			LogDebug("Starting change tracker change detection.");
			// TODO EF Core 9: Pokud bychom měli i objekt(y), které vedou na tento cizí klíč, mohli bychom použít jen lokální detekci změn
			dbContext.ChangeTracker.DetectChanges();
		}
	}

	private IQueryable<TProperty> LoadReferencePropertyInternal_GetQuery<TProperty>(List<object> foreignKeysToLoad)
		where TProperty : class
	{
		// získáme název vlastnosti primárního klíče třídy načítané vlastnosti (obvykle "Id")
		// Performance: No big issue.
		string propertyPrimaryKey = dbContext.Model.FindEntityType(typeof(TProperty)).FindPrimaryKey().Properties.Single().Name;

		// získáme query pro načtení objektů

		// https://github.com/aspnet/EntityFrameworkCore/issues/14408
		// Jako workadound stačí místo v EF.Property<object> namísto object zvolit skutečný typ. Aktuálně používáme jen int, hardcoduji tedy int bez vynakládání většího úsilí na obecnější řešení.
		List<int> foreignKeysToQueryInt = foreignKeysToLoad.Cast<int>().ToList();
		return dbContext.Set<TProperty>()
			.AsQueryable(QueryTagBuilder.CreateTag(typeof(DbDataLoader), null))
			.Where(foreignKeysToQueryInt.ContainsEffective<TProperty>(item => EF.Property<int>(item, propertyPrimaryKey)));
	}

	private void LoadReferencePropertyInternal_StoreToCache<TProperty>(List<TProperty> loadedProperties)
		where TProperty : class
	{
		// uložíme do cache, pokud je cachovaná
		foreach (TProperty loadedEntity in loadedProperties)
		{
			entityCacheManager.StoreEntity(loadedEntity);
		}
	}

	private LoadPropertyInternalResult LoadReferencePropertyInternal_GetResult<TEntity, TProperty>(string propertyName, ICollection<TEntity> entities)
		where TEntity : class
		where TProperty : class
	{
		var propertyLambdaExpression = lambdaExpressionManager.GetPropertyLambdaExpression<TEntity, TProperty>(propertyName);

		// zde spoléháme na proběhnutí fixupu

		IEnumerable<TProperty> loadedEntities = entities.Select(item => propertyLambdaExpression.LambdaCompiled(item));
		return new LoadPropertyInternalResult
		{
			// Zde předáváme dvakrát IEnumerable<TProperty>, ale efektivně bude použit nejvýše jeden z nich.
			// Entities v dalším průchodu foreachem v LoadInternal[Async] (pokud nenásleduje další průchod, nebude kolekce nikdy zpracována)
			// FluentDataLoader v dalším ThanLoad (pokud nenásleduje ThenLoad[Async], nebude kolekce nikdy zpracována)
			Entities = loadedEntities,
			FluentDataLoader = new DbFluentDataLoader<TProperty>(this, loadedEntities)
		};
	}
}
