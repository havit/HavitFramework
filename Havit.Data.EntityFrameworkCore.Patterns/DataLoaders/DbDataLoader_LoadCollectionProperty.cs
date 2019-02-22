using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.Patterns.DataLoaders;
using Havit.Data.Patterns.Infrastructure;
using Havit.Linq;
using Havit.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
			LoadCollectionPropertyInternal_GetFromCache<TEntity>(propertyName, entities, out var primaryKeysToLoad);
			if ((primaryKeysToLoad != null) && primaryKeysToLoad.Any()) // zůstalo nám, na co se ptát do databáze?
			{
				List<TPropertyItem> loadedProperties = LoadCollectionPropertyInternal_GetQuery<TEntity, TPropertyItem>(primaryKeysToLoad, propertyName).ToList();

				// TODO JK: Pozor, do cache potřebujeme dostat i prázdné kolekce!
				//LoadCollectionPropertyInternal_StoreToCache(...);
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
			LoadCollectionPropertyInternal_GetFromCache<TEntity>(propertyName, entities, out var primaryKeysToLoad);
			if ((primaryKeysToLoad != null) && primaryKeysToLoad.Any()) // zůstalo nám, na co se ptát do databáze?
			{
				List<TPropertyItem> loadedProperties = await LoadCollectionPropertyInternal_GetQuery<TEntity, TPropertyItem>(primaryKeysToLoad, propertyName).ToListAsync().ConfigureAwait(false);

				// TODO JK: Pozor, do cache potřebujeme dostat i prázdné kolekce!
				//LoadCollectionPropertyInternal_StoreToCache(...);
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
		private void LoadCollectionPropertyInternal_GetFromCache<TEntity>(string propertyName, TEntity[] entities, out List<object> primaryKeysToLoad)
			where TEntity : class
		{			
			List<TEntity> entitiesToLoadReference = entities.Where(entity => !IsEntityPropertyLoaded(entity, propertyName)).ToList();
			IEnumerable<TEntity> entitiesNotInAddedState = entities.Where(item => dbContext.GetEntityState(item) != EntityState.Added);
			List<TEntity> entitiesToLoadQuery = entitiesNotInAddedState.Where(entity => !IsEntityPropertyLoaded(entity, propertyName)).ToList();
			
			if (entitiesToLoadReference.Count == 0)
			{
				primaryKeysToLoad = null;
				return;
			}

			// TODO JK: Caching

			primaryKeysToLoad = entitiesToLoadReference.Select(entityToLoadReference => entityKeyAccessor.GetEntityKeyValues(entityToLoadReference).Single()).ToList();
		}

		/// <summary>
		/// Vrátí dotaz načítající vlastnosti objektů pro dané primární klíče.
		/// </summary>
		private IQueryable<TProperty> LoadCollectionPropertyInternal_GetQuery<TEntity, TProperty>(List<object> primaryKeysToLoad, string propertyName)
			where TEntity : class
			where TProperty : class
		{
			var foreignKeyProperty = dbContext.Model.FindEntityType(typeof(TEntity)).FindNavigation(propertyName).ForeignKey.Properties.Single();
	
			List<int> primaryKeysToLoadInt = primaryKeysToLoad.Cast<int>().ToList();
			return dbContext.Set<TProperty>()
					.AsQueryable()
					// workaround: Bez následujícího řádku může při vykonávání dotazu dojít System.InvalidOperationException: Objekt povolující hodnotu Null musí mít hodnotu.
					// Chráněno testy DbDataLoader_Load_Collection_SupportsNullableForeignKeysInMemory a DbDataLoader_Load_Collection_SupportsNullableForeignKeysInDatabase (pokud odebereme následující řádek, budou tyto testy failovat).
					.WhereIf(foreignKeyProperty.ClrType == typeof(int?), item => null != EF.Property<int?>(item, foreignKeyProperty.Name))
					.Where(primaryKeysToLoadInt.ContainsEffective<TProperty>(item => EF.Property<int>(item, foreignKeyProperty.Name)));
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
