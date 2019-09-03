using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.PropertyLambdaExpressions.Internal;
using Havit.Data.Patterns.DataLoaders;
using Havit.Data.Patterns.Infrastructure;
using Havit.Diagnostics.Contracts;
using Havit.Linq;
using Havit.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataLoaders
{
	public partial class DbDataLoader
	{
		/// <summary>
		/// Zajistí načtení vlastnosti, která je kolekcí. Voláno reflexí.
		/// </summary>
		private LoadPropertyInternalResult LoadCollectionPropertyInternal<TEntity, TPropertyCollection, TPropertyItem>(string propertyName, string originalPropertyName, TEntity[] entities)
			where TEntity : class
			where TPropertyCollection : class
			where TPropertyItem : class
		{
			LoadCollectionPropertyInternal_GetFromCache<TEntity, TPropertyItem>(propertyName, entities, out var entitiesToLoadQuery);
			if ((entitiesToLoadQuery != null) && entitiesToLoadQuery.Any()) // zůstalo nám, na co se ptát do databáze?
			{
                List<TPropertyItem> loadedProperties = LoadCollectionPropertyInternal_GetQuery<TEntity, TPropertyItem>(entitiesToLoadQuery, propertyName).ToList();
				LoadCollectionPropertyInternal_StoreCollectionsToCache<TEntity, TPropertyItem>(entitiesToLoadQuery, propertyName);
                LoadCollectionPropertyInternal_StoreEntitiesToCache<TPropertyItem>(loadedProperties);
            }

			LoadCollectionPropertyInternal_InitializeCollections<TEntity, TPropertyCollection, TPropertyItem>(entities, propertyName);
			LoadCollectionPropertyInternal_MarkAsLoaded(entities, propertyName); // potřebujeme označit všechny kolekce za načtené (načtené + založené prázdné + odbavené z cache)

			return LoadCollectionPropertyInternal_GetResult<TEntity, TPropertyCollection, TPropertyItem>(entities, originalPropertyName);
		}

		/// <summary>
		/// Zajistí načtení vlastnosti, která je kolekcí. Voláno reflexí.
		/// </summary>
		private async Task<LoadPropertyInternalResult> LoadCollectionPropertyInternalAsync<TEntity, TPropertyCollection, TPropertyItem>(string propertyName, string originalPropertyName, TEntity[] entities)
			where TEntity : class
			where TPropertyCollection : class
			where TPropertyItem : class
		{
			LoadCollectionPropertyInternal_GetFromCache<TEntity, TPropertyItem>(propertyName, entities, out var entitiesToLoadQuery);
			if ((entitiesToLoadQuery != null) && entitiesToLoadQuery.Any()) // zůstalo nám, na co se ptát do databáze?
			{
                List<TPropertyItem> loadedProperties = await LoadCollectionPropertyInternal_GetQuery<TEntity, TPropertyItem>(entitiesToLoadQuery, propertyName).ToListAsync().ConfigureAwait(false);
                LoadCollectionPropertyInternal_StoreCollectionsToCache<TEntity, TPropertyItem>(entitiesToLoadQuery, propertyName);
                LoadCollectionPropertyInternal_StoreEntitiesToCache<TPropertyItem>(loadedProperties);
            }

			LoadCollectionPropertyInternal_InitializeCollections<TEntity, TPropertyCollection, TPropertyItem>(entities, propertyName);
			LoadCollectionPropertyInternal_MarkAsLoaded(entities, propertyName); // potřebujeme označit všechny kolekce za načtené (načtené + založené prázdné + odbavené z cache)
			
			return LoadCollectionPropertyInternal_GetResult<TEntity, TPropertyCollection, TPropertyItem>(entities, originalPropertyName);
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
				if (entityCacheManager.TryGetCollection<TEntity, TPropertyItem>(entityToLoad, propertyName))
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
				.AsQueryable()
				// workaround: Bez následujícího řádku může při vykonávání dotazu dojít System.InvalidOperationException: Objekt povolující hodnotu Null musí mít hodnotu.
				// Chráněno testy DbDataLoader_Load_Collection_SupportsNullableForeignKeysInMemory a DbDataLoader_Load_Collection_SupportsNullableForeignKeysInDatabase (pokud odebereme následující řádek, budou tyto testy failovat).
				.WhereIf(foreignKeyProperty.ClrType == typeof(int?), item => null != EF.Property<int?>(item, foreignKeyProperty.Name))
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
				entityCacheManager.StoreCollection<TEntity, TPropertyItem>(loadedEntity, propertyName);                                
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

		private LoadPropertyInternalResult LoadCollectionPropertyInternal_GetResult<TEntity, TPropertyCollection, TPropertyItem>(TEntity[] entities, string originalPropertyName)
			where TEntity : class
			where TPropertyCollection : class
			where TPropertyItem : class
		{
			var originalPropertyLambda = lambdaExpressionManager.GetPropertyLambdaExpression<TEntity, IEnumerable<TPropertyItem>>(originalPropertyName).LambdaCompiled;

			return new LoadPropertyInternalResult
			{
				Entities = entities.SelectMany(item => (IEnumerable<TPropertyItem>)originalPropertyLambda(item)).ToArray(),
				FluentDataLoader = new DbFluentDataLoader<TPropertyCollection, TPropertyItem>(this, entities.SelectMany(item => originalPropertyLambda(item)).ToArray())
			};
		}
	}
}
