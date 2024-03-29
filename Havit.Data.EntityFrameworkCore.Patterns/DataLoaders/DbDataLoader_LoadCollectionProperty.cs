﻿using System.Reflection;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Internal;
using Havit.Data.Patterns.DataLoaders;
using Havit.Diagnostics.Contracts;
using Havit.Linq;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;

public partial class DbDataLoader
{
	/// <summary>
	/// Zajistí načtení vlastnosti, která je kolekcí. Voláno reflexí.
	/// </summary>
	private LoadPropertyInternalResult LoadCollectionPropertyInternal<TEntity, TPropertyCollection, TOriginalPropertyCollection, TPropertyItem>(string propertyName, string originalPropertyName, TEntity[] entities)
		where TEntity : class
		where TPropertyCollection : class
		where TOriginalPropertyCollection : class
		where TPropertyItem : class
	{
		LogDebug("Retrieving data for {0} entities from the cache.", args: entities.Length);
		LoadCollectionPropertyInternal_GetFromCache<TEntity, TPropertyItem>(propertyName, entities, out var entitiesToLoadQuery);
		if ((entitiesToLoadQuery != null) && entitiesToLoadQuery.Any()) // zůstalo nám, na co se ptát do databáze?
		{
			LogDebug("Trying to retrieve data for {0} entities from the database.", args: entitiesToLoadQuery.Count);
			List<TPropertyItem> loadedProperties;
			if ((entitiesToLoadQuery.Count <= ChunkSize) || dbContext.SupportsSqlServerOpenJson())
			{
				IQueryable<TPropertyItem> query = LoadCollectionPropertyInternal_GetQuery<TEntity, TPropertyItem>(entitiesToLoadQuery, propertyName);
				LogDebug("Starting reading from a database.");
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
				List<IQueryable<TPropertyItem>> chunkQueries = entitiesToLoadQuery.Chunkify(ChunkSize).Select(entitiesToLoadQueryChunk => LoadCollectionPropertyInternal_GetQuery<TEntity, TPropertyItem>(entitiesToLoadQueryChunk.ToList(), propertyName)).ToList(); /* ToList: Jen seznam dotazů, nikoliv spuštění dotazu */
				LogDebug("Starting reading chunks from a database.");
				loadedProperties = new List<TPropertyItem>();
				for (int chunkIndex = 0; chunkIndex < chunkQueries.Count; chunkIndex++)
				{
					var chunkQuery = chunkQueries[chunkIndex];
					List<TPropertyItem> loadedPropertiesChunk = chunkQuery.TagWith($"Chunk {chunkIndex + 1}/{chunkQueries.Count}").ToList();
					loadedProperties.AddRange(loadedPropertiesChunk);
				}
			}
			LogDebug("Finished reading from a database.");
			LogDebug("Storing data for {0} entities to the cache.", args: entitiesToLoadQuery.Count);
			LoadCollectionPropertyInternal_StoreCollectionsToCache<TEntity, TPropertyItem>(entitiesToLoadQuery, propertyName);
			LoadCollectionPropertyInternal_StoreEntitiesToCache<TPropertyItem>(loadedProperties);
		}

		LogDebug("Initializing collections.");
		LoadCollectionPropertyInternal_InitializeCollections<TEntity, TPropertyCollection, TPropertyItem>(entities, propertyName);
		LogDebug("Marking properties as loaded.");
		LoadCollectionPropertyInternal_MarkAsLoaded(entities, propertyName); // potřebujeme označit všechny kolekce za načtené (načtené + založené prázdné + odbavené z cache)

		LogDebug("Returning.");
		return LoadCollectionPropertyInternal_GetResult<TEntity, TOriginalPropertyCollection, TPropertyItem>(entities, originalPropertyName);
	}

	/// <summary>
	/// Zajistí načtení vlastnosti, která je kolekcí. Voláno reflexí.
	/// </summary>
	private async ValueTask<LoadPropertyInternalResult> LoadCollectionPropertyInternalAsync<TEntity, TPropertyCollection, TOriginalPropertyCollection, TPropertyItem>(string propertyName, string originalPropertyName, TEntity[] entities, CancellationToken cancellationToken /* no default */)
		where TEntity : class
		where TPropertyCollection : class
		where TOriginalPropertyCollection : class
		where TPropertyItem : class
	{
		LogDebug("Retrieving data for {0} entities from the cache.", args: entities.Length);
		LoadCollectionPropertyInternal_GetFromCache<TEntity, TPropertyItem>(propertyName, entities, out var entitiesToLoadQuery);

		if ((entitiesToLoadQuery != null) && entitiesToLoadQuery.Any()) // zůstalo nám, na co se ptát do databáze?
		{
			LogDebug("Trying to retrieve data for {0} entities from the database.", args: entitiesToLoadQuery.Count);
			List<TPropertyItem> loadedProperties;
			if ((entitiesToLoadQuery.Count <= ChunkSize) || dbContext.SupportsSqlServerOpenJson())
			{
				IQueryable<TPropertyItem> query = LoadCollectionPropertyInternal_GetQuery<TEntity, TPropertyItem>(entitiesToLoadQuery, propertyName);
				LogDebug("Starting reading from a database.");
				loadedProperties = await query.ToListAsync(cancellationToken).ConfigureAwait(false);
			}
			else
			{
				// viz komentář v LoadCollectionPropertyInternal
				List<IQueryable<TPropertyItem>> chunkQueries = entitiesToLoadQuery.Chunkify(ChunkSize).Select(entitiesToLoadQueryChunk => LoadCollectionPropertyInternal_GetQuery<TEntity, TPropertyItem>(entitiesToLoadQueryChunk.ToList(), propertyName)).ToList(); /* ToList: Jen seznam dotazů, nikoliv spuštění dotazu */
				LogDebug("Starting reading chunks from a database.");
				loadedProperties = new List<TPropertyItem>();
				for (int chunkIndex = 0; chunkIndex < chunkQueries.Count; chunkIndex++)
				{
					var chunkQuery = chunkQueries[chunkIndex];
					List<TPropertyItem> loadedPropertiesChunk = await chunkQuery.TagWith($"Chunk {chunkIndex + 1}/{chunkQueries.Count}").ToListAsync(cancellationToken).ConfigureAwait(false);
					loadedProperties.AddRange(loadedPropertiesChunk);
				}
			}
			LogDebug("Finished reading from a database.");

			LogDebug("Storing data for {0} entities to the cache.", args: entitiesToLoadQuery.Count);
			LoadCollectionPropertyInternal_StoreCollectionsToCache<TEntity, TPropertyItem>(entitiesToLoadQuery, propertyName);
			LoadCollectionPropertyInternal_StoreEntitiesToCache<TPropertyItem>(loadedProperties);
		}

		LogDebug("Initializing collections.");
		LoadCollectionPropertyInternal_InitializeCollections<TEntity, TPropertyCollection, TPropertyItem>(entities, propertyName);
		LogDebug("Marking properties as loaded.");
		LoadCollectionPropertyInternal_MarkAsLoaded(entities, propertyName); // potřebujeme označit všechny kolekce za načtené (načtené + založené prázdné + odbavené z cache)

		LogDebug("Returning.");
		return LoadCollectionPropertyInternal_GetResult<TEntity, TOriginalPropertyCollection, TPropertyItem>(entities, originalPropertyName);
	}

	/// <summary>
	/// Zkouší načíst objekty z cache.
	/// Klíče objektů, které se nepodařilo načíst z cache, nastavuje out parametru.
	/// Aktuálně s cache nic nedělá, do out parametru vrací všechny entity.
	/// </summary>
	private void LoadCollectionPropertyInternal_GetFromCache<TEntity, TPropertyItem>(string propertyName, TEntity[] entities, out List<TEntity> entitiesToLoadQuery)
		where TEntity : class
		where TPropertyItem : class
	{
		List<TEntity> entitiesToLoad = entities.Where(entity => !IsEntityPropertyLoaded(entity, propertyName)).Where(item => dbContext.GetEntityState(item) != EntityState.Added).ToList();

		if (entitiesToLoad.Count == 0)
		{
			entitiesToLoadQuery = null;
			return;
		}

		entitiesToLoadQuery = new List<TEntity>(entitiesToLoad.Count);
		foreach (var entityToLoad in entitiesToLoad)
		{
			if (entityCacheManager.TryGetNavigation<TEntity, TPropertyItem>(entityToLoad, propertyName))
			{
				// NOOP
			}
			else
			{
				entitiesToLoadQuery.Add(entityToLoad);
			}
		}
	}

	/// <summary>
	/// Vrátí dotaz načítající vlastnosti objektů pro dané primární klíče.
	/// </summary>
	private IQueryable<TProperty> LoadCollectionPropertyInternal_GetQuery<TEntity, TProperty>(List<TEntity> entitiesToLoad, string propertyName)
		where TEntity : class
		where TProperty : class
	{
		// Performance: No big issue.
		var foreignKeyProperty = dbContext.Model.FindEntityType(typeof(TEntity)).FindNavigation(propertyName).ForeignKey.Properties.Single();

		Contract.Assert(foreignKeyProperty.ClrType == typeof(int) || foreignKeyProperty.ClrType == typeof(int?));

		List<int> primaryKeysToLoad = entitiesToLoad.Select(entityToLoad => entityKeyAccessor.GetEntityKeyValues(entityToLoad).Single()).Cast<int>().ToList();
		return dbContext.Set<TProperty>()
			.AsQueryable(QueryTagBuilder.CreateTag(typeof(DbDataLoader), null))

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
		// pozor, property zde ještě může být null
		foreach (TEntity loadedEntity in loadedEntities)
		{
			entityCacheManager.StoreNavigation<TEntity, TPropertyItem>(loadedEntity, propertyName);
		}
	}

	private void LoadCollectionPropertyInternal_StoreEntitiesToCache<TProperty>(List<TProperty> loadedProperties)
		where TProperty : class
	{
		// TODO: Test na ManyToMany
		if (entityKeyAccessor.GetEntityKeyPropertyNames(typeof(TProperty)).Length == 1)
		{
			// uložíme do cache, pokud je cachovaná
			foreach (TProperty loadedEntity in loadedProperties)
			{
				entityCacheManager.StoreEntity(loadedEntity);
			}
		}
	}

	/// <summary>
	/// Tato metoda inicializuje kolekce (nastaví nové instance), pokud jsou null.
	/// </summary>
	private void LoadCollectionPropertyInternal_InitializeCollections<TEntity, TPropertyCollection, TPropertyItem>(TEntity[] entities, string propertyName)
		where TEntity : class
		where TPropertyCollection : class
		where TPropertyItem : class
	{
		var propertyLambda = lambdaExpressionManager.GetPropertyLambdaExpression<TEntity, TPropertyCollection>(propertyName).LambdaCompiled;
		List<TEntity> entitiesWithNullReference = entities.Where(item => propertyLambda(item) == null).ToList();

		if (entitiesWithNullReference.Count > 0)
		{
			if (typeof(TPropertyCollection) == typeof(List<TPropertyItem>) || typeof(TPropertyCollection) == typeof(IList<TPropertyItem>))
			{
				MethodInfo setter = typeof(TEntity).GetProperty(propertyName).GetSetMethod();
				if (setter == null)
				{
					throw new InvalidOperationException($"DataLoader cannot set collection property {propertyName} on type {{typeof(TEntity).FullName}} while it does not have a public setter.");
				}
				foreach (var item in entitiesWithNullReference)
				{
					setter.Invoke(item, new object[] { new List<TPropertyItem>() });
				}
			}
			else
			{
				throw new InvalidOperationException($"DataLoader cannot set collection property {propertyName} on type {typeof(TEntity).FullName} while it is not type of List<{typeof(TPropertyItem).Name}> or IList<{typeof(TPropertyItem).Name}.");
			}
		}
	}

	/// <summary>
	/// Označí entitám vlatnost propertyName jako načtenou.
	/// </summary>
	private void LoadCollectionPropertyInternal_MarkAsLoaded<TEntity>(TEntity[] entities, string propertyName)
		where TEntity : class
	{
		foreach (var entity in entities)
		{
			dbContext.MarkNavigationAsLoaded(entity, propertyName);
		}
	}

	private LoadPropertyInternalResult LoadCollectionPropertyInternal_GetResult<TEntity, TOriginalPropertyCollection, TPropertyItem>(TEntity[] entities, string originalPropertyName)
		where TEntity : class
		where TOriginalPropertyCollection : class
		where TPropertyItem : class
	{
		var originalPropertyLambda = lambdaExpressionManager.GetPropertyLambdaExpression<TEntity, IEnumerable<TPropertyItem>>(originalPropertyName).LambdaCompiled;

		return new LoadPropertyInternalResult
		{
			Entities = entities.SelectMany(item => (IEnumerable<TPropertyItem>)originalPropertyLambda(item)).ToArray(),
			FluentDataLoader = new DbFluentDataLoader<TOriginalPropertyCollection, TPropertyItem>(this, entities.SelectMany(item => originalPropertyLambda(item)).ToArray())
		};
	}
}
