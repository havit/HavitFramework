﻿using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Internal;
using Havit.Data.Patterns.DataLoaders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;

public partial class DbDataLoader
{
	/// <summary>
	/// Zajistí načtení vlastnosti, která je referencí (není kolkecí). Voláno reflexí.
	/// </summary>
	private LoadPropertyInternalResult LoadReferencePropertyInternal<TEntity, TProperty>(string propertyName, IEnumerable<TEntity> distinctNotNullEntities, string propertyPathString)
		where TEntity : class
		where TProperty : class
	{
		ICollection<TEntity> entities = LoadPropertyInternal_EntitiesToCollectionOptimized(distinctNotNullEntities);

		if (entities.Count == 0)
		{
			_logger.LogDebug("No entity to retrieve data.");
			return LoadPropertyInternalResult.CreateEmpty<TProperty>();
		}

		var isTPropertyCachable = _entityCacheManager.ShouldCacheEntityType<TProperty>();
		LoadReferencePropertyInternal_GetFromCache<TEntity, TProperty>(propertyName, entities, out List<object> foreignKeysToLoad, isTPropertyCachable);

		if ((foreignKeysToLoad != null) && foreignKeysToLoad.Count > 0) // zůstalo nám, na co se ptát do databáze?
		{
			_logger.LogDebug("Reading data for {Count} entities from the database...", foreignKeysToLoad.Count);

			List<TProperty> loadedProperties;
			if ((foreignKeysToLoad.Count < ChunkSize) || _dbContext.SupportsSqlServerOpenJson())
			{
				IQueryable<TProperty> query = LoadReferencePropertyInternal_GetQuery<TEntity, TProperty>(foreignKeysToLoad, propertyPathString);
				loadedProperties = query.ToList();
			}
			else
			{
				// viz komentář v LoadCollectionPropertyInternal
				int chunkIndex = 0;
				int chunksCount = (int)Math.Ceiling((decimal)foreignKeysToLoad.Count / (decimal)ChunkSize);
				IEnumerable<IQueryable<TProperty>> chunkQueries = foreignKeysToLoad.Chunk(ChunkSize).Select(foreignKeysToLoadChunk => LoadReferencePropertyInternal_GetQuery<TEntity, TProperty>(foreignKeysToLoadChunk.ToList(), propertyPathString));

				loadedProperties = new List<TProperty>(foreignKeysToLoad.Count);
				foreach (var chunkQuery in chunkQueries)
				{
					List<TProperty> loadedPropertiesChunk = chunkQuery.TagWith($"Chunk {++chunkIndex}/{chunksCount}").ToList();
					loadedProperties.AddRange(loadedPropertiesChunk);
				}
			}

			_logger.LogDebug("Data for entities read from the database.");

			if (isTPropertyCachable)
			{
				LoadReferencePropertyInternal_StoreToCache(loadedProperties);
			}
		}

		return LoadReferencePropertyInternal_GetResult<TEntity, TProperty>(propertyName, entities);
	}

	/// <summary>
	/// Zajistí načtení vlastnosti, která je referencí (není kolkecí). Voláno reflexí.
	/// </summary>
	private async ValueTask<LoadPropertyInternalResult> LoadReferencePropertyInternalAsync<TEntity, TProperty>(string propertyName, IEnumerable<TEntity> distinctNotNullEntities, string propertyPathString, CancellationToken cancellationToken /* no default */)
		where TEntity : class
		where TProperty : class
	{
		ICollection<TEntity> entities = LoadPropertyInternal_EntitiesToCollectionOptimized(distinctNotNullEntities);

		if (entities.Count == 0)
		{
			_logger.LogDebug("No entity to retrieve data.");
			return LoadPropertyInternalResult.CreateEmpty<TProperty>();
		}

		var isTPropertyCachable = _entityCacheManager.ShouldCacheEntityType<TProperty>();
		LoadReferencePropertyInternal_GetFromCache<TEntity, TProperty>(propertyName, entities, out List<object> foreignKeysToLoad, isTPropertyCachable);

		if ((foreignKeysToLoad != null) && foreignKeysToLoad.Count > 0) // zůstalo nám, na co se ptát do databáze?
		{
			_logger.LogDebug("Reading data for {Count} entities from the database...", foreignKeysToLoad.Count);

			List<TProperty> loadedProperties;
			if ((foreignKeysToLoad.Count < ChunkSize) || _dbContext.SupportsSqlServerOpenJson())
			{
				IQueryable<TProperty> query = LoadReferencePropertyInternal_GetQuery<TEntity, TProperty>(foreignKeysToLoad, propertyPathString);
				loadedProperties = await query.ToListAsync(cancellationToken).ConfigureAwait(false);
			}
			else
			{
				// viz komentář v LoadCollectionPropertyInternal
				int chunkIndex = 0;
				int chunksCount = (int)Math.Ceiling((decimal)foreignKeysToLoad.Count / (decimal)ChunkSize);
				IEnumerable<IQueryable<TProperty>> chunkQueries = foreignKeysToLoad.Chunk(ChunkSize).Select(foreignKeysToLoadChunk => LoadReferencePropertyInternal_GetQuery<TEntity, TProperty>(foreignKeysToLoadChunk.ToList(), propertyPathString));

				loadedProperties = new List<TProperty>(foreignKeysToLoad.Count);
				foreach (var chunkQuery in chunkQueries)
				{
					List<TProperty> loadedPropertiesChunk = await chunkQuery.TagWith($"Chunk {++chunkIndex}/{chunksCount}").ToListAsync(cancellationToken).ConfigureAwait(false);
					loadedProperties.AddRange(loadedPropertiesChunk);
				}
			}
			_logger.LogDebug("Data for entities read from the database.");

			if (isTPropertyCachable)
			{
				LoadReferencePropertyInternal_StoreToCache(loadedProperties);
			}
		}

		return LoadReferencePropertyInternal_GetResult<TEntity, TProperty>(propertyName, entities);
	}

	private void LoadReferencePropertyInternal_GetFromCache<TEntity, TProperty>(string propertyName, ICollection<TEntity> entities, out List<object> foreignKeysToLoad, bool isTPropertyCachable)
		where TEntity : class
		where TProperty : class
	{
		List<TEntity> entitiesToLoadReference = entities.Where(entity => !IsEntityPropertyLoaded(entity, propertyName)).ToList();

		if (entitiesToLoadReference.Count == 0)
		{
			_logger.LogDebug("No entity to retrieve data (previously loaded).");
			foreignKeysToLoad = null;
			return;
		}

		_logger.LogDebug("Retrieving data for {Count} entities from the identity map and the cache...", entities.Count);

		// získáme cizí klíč reprezentující referenci (Navigation)
		IProperty foreignKeyForReference = _dbContext.Model.FindEntityType(typeof(TEntity)).FindNavigation(propertyName).ForeignKey.Properties.Single();

		// získáme klíče objektů, které potřebujeme načíst (z "běžných vlastností" nebo z shadow properties)
		// ignorujeme nenastavené reference (null)
		IEnumerable<object> foreignKeyValues = entitiesToLoadReference.Select(entity => _dbContext.GetEntry(entity, true).CurrentValues[foreignKeyForReference]).Where(value => value != null).Distinct();

		IDbSet<TProperty> dbSet = _dbContext.Set<TProperty>();

		int cacheHitCounter = 0;
		bool shouldFixup = false;
		foreignKeysToLoad = _entityCacheManager.ShouldCacheEntityType<TProperty>() ? new List<object>() : new List<object>(entitiesToLoadReference.Count);

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
				cacheHitCounter += 1;
				shouldFixup = true;
			}
			else if (isTPropertyCachable && _entityCacheManager.TryGetEntity<TProperty>(foreignKeyValue, out TProperty cachedEntity))
			{
				cacheHitCounter += 1;
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
			_logger.LogDebug("It is required to to detect changes by ChangeTracker.");
			// Pokud bychom měli i objekt(y), které vedou na tento cizí klíč, mohli bychom použít jen lokální detekci změn.
			_dbContext.ChangeTracker.DetectChanges();
		}

		_logger.LogDebug("Retrieved data for {Count} entities from the identity map and the cache.", cacheHitCounter);

	}

	private IQueryable<TProperty> LoadReferencePropertyInternal_GetQuery<TEntity, TProperty>(List<object> foreignKeysToLoad, string propertyPathString)
		where TProperty : class
	{
		// získáme název vlastnosti primárního klíče třídy načítané vlastnosti (obvykle "Id")
		// Performance: No big issue.
		string propertyPrimaryKey = _dbContext.Model.FindEntityType(typeof(TProperty)).FindPrimaryKey().Properties.Single().Name;

		// získáme query pro načtení objektů

		// https://github.com/aspnet/EntityFrameworkCore/issues/14408
		// Jako workadound stačí místo v EF.Property<object> namísto object zvolit skutečný typ. Aktuálně používáme jen int, hardcoduji tedy int bez vynakládání většího úsilí na obecnější řešení.
		List<int> foreignKeysToQueryInt = foreignKeysToLoad.Cast<int>().ToList();
		return _dbContext.Set<TProperty>()
			.AsQueryable($"{nameof(DbDataLoader)} ({typeof(TEntity).Name} {propertyPathString})")
			.Where(foreignKeysToQueryInt.ContainsEffective<TProperty>(item => EF.Property<int>(item, propertyPrimaryKey)));
	}

	private void LoadReferencePropertyInternal_StoreToCache<TProperty>(List<TProperty> loadedProperties)
		where TProperty : class
	{
		_logger.LogDebug("Storing entities to cache...");
		// uložíme do cache, pokud je cachovaná
		foreach (TProperty loadedEntity in loadedProperties)
		{
			_entityCacheManager.StoreEntity(loadedEntity);
		}
		_logger.LogDebug("Entities stored to cache.");
	}

	private LoadPropertyInternalResult LoadReferencePropertyInternal_GetResult<TEntity, TProperty>(string propertyName, ICollection<TEntity> entities)
		where TEntity : class
		where TProperty : class
	{
		var propertyLambdaExpression = _lambdaExpressionManager.GetPropertyLambdaExpression<TEntity, TProperty>(propertyName);

		// zde spoléháme na proběhnutí fixupu

		IEnumerable<TProperty> loadedDistinctNotNullEntities = entities.Select(item => propertyLambdaExpression.LambdaCompiled(item))
			.Where(item => item != null)
			.Distinct();

		return new LoadPropertyInternalResult
		{
			// Zde předáváme dvakrát IEnumerable<TProperty>, ale efektivně bude použit nejvýše jeden z nich.
			// Entities v dalším průchodu foreachem v LoadInternal[Async] (pokud nenásleduje další průchod, nebude kolekce nikdy zpracována)
			// FluentDataLoader v dalším ThanLoad (pokud nenásleduje ThenLoad[Async], nebude kolekce nikdy zpracována)
			Entities = loadedDistinctNotNullEntities,
			FluentDataLoader = new FluentDataLoader<TProperty>(this, loadedDistinctNotNullEntities)
		};
	}
}
