using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Havit.Data.EntityFrameworkCore.Metadata;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;
using Havit.Data.Patterns.DataLoaders;
using Havit.Data.Patterns.Infrastructure;
using Havit.Diagnostics.Contracts;
using Havit.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataLoaders
{
	/// <summary>
	/// Explicity data loader.
	/// Načte hodnoty vlastnosti třídy, pokud ještě nejsou načteny.
	/// Podporováno je zřetězení (subjekt => subjekt.Adresa.Zeme.Svetadil) vč. varianty s kolekcemi, kdy je třeba použít AllItems (subjekt => subjekt.Adresy.AllItems().Zeme).
	/// </summary>
	public partial class DbDataLoader : IDataLoader, IDataLoaderAsync
	{
		private readonly IDbContext dbContext;
		private readonly IPropertyLoadSequenceResolver propertyLoadSequenceResolver;
		private readonly IPropertyLambdaExpressionManager lambdaExpressionManager;
		private readonly IEntityCacheManager entityCacheManager;

		/// <summary>
		/// Konstructor.
		/// </summary>
		/// <param name="dbContext">DbContext, pomocí něhož budou objekty načítány.</param>
		/// <param name="propertyLoadSequenceResolver">Služba, která poskytne vlastnosti, které mají být načteny, a jejich pořadí.</param>
		/// <param name="lambdaExpressionManager">LambdaExpressionManager, pomocí něhož jsou získávány expression trees a kompilované expression trees pro lambda výrazy přístupu k vlastnostem objektů.</param>
		/// <param name="entityCacheManager">Zajišťuje získávání a ukládání entit z/do cache.</param>
		public DbDataLoader(IDbContext dbContext, IPropertyLoadSequenceResolver propertyLoadSequenceResolver, IPropertyLambdaExpressionManager lambdaExpressionManager, IEntityCacheManager entityCacheManager)
		{
			Contract.Requires(dbContext != null);

			this.dbContext = dbContext;
			this.propertyLoadSequenceResolver = propertyLoadSequenceResolver;
			this.lambdaExpressionManager = lambdaExpressionManager;
			this.entityCacheManager = entityCacheManager;
		}

		#region IDataLoader implementation (Load + LoadAll)

		/// <summary>
		/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
		/// </summary>
		/// <param name="entity">Objekt, jehož vlastnosti budou načteny.</param>
		/// <param name="propertyPath">Vlastnost, která má být načtená.</param>
		public IFluentDataLoader<TProperty> Load<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> propertyPath)
			where TEntity : class
			where TProperty : class
		{
			return LoadInternal(new TEntity[] { entity }, propertyPath);
		}

		/// <summary>
		/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
		/// </summary>
		/// <param name="entity">Objekt, jehož vlastnosti budou načteny.</param>
		/// <param name="propertyPaths">Vlastnosti, které mají být načteny.</param>
		public void Load<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] propertyPaths)
			where TEntity : class
		{
			Contract.Requires(propertyPaths != null);

			foreach (Expression<Func<TEntity, object>> propertyPath in propertyPaths)
			{
				LoadInternal(new TEntity[] { entity }, propertyPath);
			}
		}

		/// <summary>
		/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
		/// </summary>
		/// <param name="entities">Objekty, jejíž vlastnosti budou načteny.</param>
		/// <param name="propertyPath">Vlastnost, která má být načtena.</param>
		public IFluentDataLoader<TProperty> LoadAll<TEntity, TProperty>(IEnumerable<TEntity> entities, Expression<Func<TEntity, TProperty>> propertyPath)
			where TEntity : class
			where TProperty : class
		{
			return LoadInternal(entities, propertyPath);
		}

		/// <summary>
		/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
		/// </summary>
		/// <param name="entities">Objekty, jejíž vlastnosti budou načteny.</param>
		/// <param name="propertyPaths">Vlastnosti, které mají být načteny.</param>
		public void LoadAll<TEntity>(IEnumerable<TEntity> entities, params Expression<Func<TEntity, object>>[] propertyPaths)
			where TEntity : class
		{
			Contract.Requires(entities != null);
			Contract.Requires(propertyPaths != null);

			foreach (Expression<Func<TEntity, object>> propertyPath in propertyPaths)
			{
				LoadInternal(entities, propertyPath);
			}
		}

		#endregion

		#region IDataLoaderAsync implementation (LoadAsync + LoadAllAsync)

		/// <summary>
		/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
		/// </summary>
		/// <param name="entity">Objekt, jehož vlastnosti budou načteny.</param>
		/// <param name="propertyPath">Vlastnost, která má být načtena.</param>
		public async Task<IFluentDataLoaderAsync<TProperty>> LoadAsync<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> propertyPath)
			where TEntity : class
			where TProperty : class
		{
			Contract.Requires(propertyPath != null);

			return await LoadInternalAsync(new TEntity[] { entity }, propertyPath);
		}

		/// <summary>
		/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
		/// </summary>
		/// <param name="entity">Objekt, jehož vlastnosti budou načteny.</param>
		/// <param name="propertyPaths">Vlastnosti, které mají být načteny.</param>
		public async Task LoadAsync<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] propertyPaths)
			where TEntity : class
		{
			Contract.Requires(propertyPaths != null);

			foreach (Expression<Func<TEntity, object>> propertyPath in propertyPaths)
			{
				await LoadInternalAsync(new TEntity[] { entity }, propertyPath);
			}
		}

		/// <summary>
		/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
		/// </summary>
		/// <param name="entities">Objekty, jejíž vlastnosti budou načteny.</param>
		/// <param name="propertyPath">Vlastnost, který má být načtena.</param>
		public async Task<IFluentDataLoaderAsync<TProperty>> LoadAllAsync<TEntity, TProperty>(IEnumerable<TEntity> entities, Expression<Func<TEntity, TProperty>> propertyPath)
			where TEntity : class
			where TProperty : class
		{
			Contract.Requires(entities != null);
			Contract.Requires(propertyPath != null);
			return await LoadInternalAsync(entities, propertyPath);
		}

		/// <summary>
		/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
		/// </summary>
		/// <param name="entities">Objekty, jejíž vlastnosti budou načteny.</param>
		/// <param name="propertyPaths">Vlastnosti, které mají být načteny.</param>
		public async Task LoadAllAsync<TEntity>(IEnumerable<TEntity> entities, params Expression<Func<TEntity, object>>[] propertyPaths)
			where TEntity : class
		{
			Contract.Requires(propertyPaths != null);

			foreach (Expression<Func<TEntity, object>> propertyPath in propertyPaths)
			{
				await LoadInternalAsync(entities, propertyPath);
			}
		}
		#endregion

		/// <summary>
		/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
		/// </summary>
		private IFluentDataLoader<TProperty> LoadInternal<TEntity, TProperty>(IEnumerable<TEntity> entitiesToLoad, Expression<Func<TEntity, TProperty>> propertyPath)
			where TEntity : class
			where TProperty : class
		{
			// přeskočíme prázdné
			TEntity[] entitiesToLoadWithoutNulls = entitiesToLoad.Where(entity => entity != null).ToArray();

			if (entitiesToLoadWithoutNulls.Length > 0) // pokud máme, co načítat
			{
				// ověříme, že jsou všechny objekty sledované change trackerem (na který spoléháme)
				Contract.Assert<InvalidOperationException>(entitiesToLoadWithoutNulls.All(item => dbContext.GetEntityState(item) != EntityState.Detached), "DbDataLoader can be used only for objects tracked by a change tracker.");

				// vytáhneme posloupnost vlastností, které budeme načítat
				PropertyToLoad[] propertiesSequenceToLoad = propertyLoadSequenceResolver.GetPropertiesToLoad(propertyPath);

				Array entities = entitiesToLoadWithoutNulls;
				object fluentDataLoader = null;

				foreach (PropertyToLoad propertyToLoad in propertiesSequenceToLoad)
				{
					LoadPropertyInternalResult loadPropertyInternalResult;

					if (!propertyToLoad.IsCollection)
					{
						loadPropertyInternalResult = (LoadPropertyInternalResult)typeof(DbDataLoader)
							.GetMethod(nameof(LoadReferencePropertyInternal), BindingFlags.Instance | BindingFlags.NonPublic)
							.MakeGenericMethod(
								propertyToLoad.SourceType,
								propertyToLoad.TargetType)
							.Invoke(this, new object[] { propertyToLoad.PropertyName, entities });
					}
					else
					{
						loadPropertyInternalResult = (LoadPropertyInternalResult)typeof(DbDataLoader)
							.GetMethod(nameof(LoadCollectionPropertyInternal), BindingFlags.Instance | BindingFlags.NonPublic)
							.MakeGenericMethod(
								propertyToLoad.SourceType,
								propertyToLoad.TargetType,
								propertyToLoad.CollectionItemType)
							.Invoke(this, new object[] { propertyToLoad.PropertyName, entities });

					}

					entities = loadPropertyInternalResult.Entities;
					fluentDataLoader = loadPropertyInternalResult.FluentDataLoader;

					if (entities.Length == 0)
					{
						// shortcut
						return new DbFluentDataLoader<TProperty>(this, new TProperty[0]);
					}
				}

				return (IFluentDataLoader<TProperty>)fluentDataLoader;
			}
			else
			{
				return new DbFluentDataLoader<TProperty>(this, new TProperty[0]);
			}
		}

		/// <summary>
		/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
		/// </summary>
		private async Task<IFluentDataLoaderAsync<TProperty>> LoadInternalAsync<TEntity, TProperty>(IEnumerable<TEntity> entitiesToLoad, Expression<Func<TEntity, TProperty>> propertyPath)
			where TEntity : class
			where TProperty : class
		{
			// přeskočíme prázdné
			TEntity[] entitiesToLoadWithoutNulls = entitiesToLoad.Where(entity => entity != null).ToArray();

			if (entitiesToLoadWithoutNulls.Length > 0) // pokud máme, co načítat
			{
				// ověříme, že jsou všechny objekty sledované change trackerem (na který spoléháme)
				Contract.Assert<InvalidOperationException>(entitiesToLoadWithoutNulls.All(item => dbContext.GetEntityState(item) != EntityState.Detached), "DbDataLoader can be used only for objects tracked by a change tracker.");

				// vytáhneme posloupnost vlastností, které budeme načítat
				PropertyToLoad[] propertiesSequenceToLoad = propertyLoadSequenceResolver.GetPropertiesToLoad(propertyPath);

				Array entities = entitiesToLoadWithoutNulls;
				object fluentDataLoader = null;

				Task task;
				foreach (PropertyToLoad propertyToLoad in propertiesSequenceToLoad)
				{
					if (!propertyToLoad.IsCollection)
					{
						task = (Task)typeof(DbDataLoader)
							.GetMethod(nameof(LoadReferencePropertyInternalAsync), BindingFlags.Instance | BindingFlags.NonPublic)
							.MakeGenericMethod(
								propertyToLoad.SourceType,
								propertyToLoad.TargetType)
							.Invoke(this, new object[] { propertyToLoad.PropertyName, entities });
					}
					else
					{
						task = (Task)typeof(DbDataLoader)
							.GetMethod(nameof(LoadCollectionPropertyInternalAsync), BindingFlags.Instance | BindingFlags.NonPublic)
							.MakeGenericMethod(
								propertyToLoad.SourceType,
								propertyToLoad.TargetType,
								propertyToLoad.CollectionItemType)
							.Invoke(this, new object[] { propertyToLoad.PropertyName, entities });
					}
					await task;
					LoadPropertyInternalResult loadPropertyInternalResult = (LoadPropertyInternalResult)((dynamic)task).Result;

					entities = loadPropertyInternalResult.Entities;
					fluentDataLoader = loadPropertyInternalResult.FluentDataLoader;

					if (entities.Length == 0)
					{
						// shortcut
						return new DbFluentDataLoader<TProperty>(this, new TProperty[0]);
					}
				}

				return (IFluentDataLoaderAsync<TProperty>)fluentDataLoader;
			}
			else
			{
				return new DbFluentDataLoader<TProperty>(this, new TProperty[0]);
			}
		}

		/// <summary>
		/// Zajistí načtení vlastnosti, která je referencí (není kolkecí). Voláno reflexí.
		/// </summary>
		private LoadPropertyInternalResult LoadReferencePropertyInternal<TEntity, TProperty>(string propertyName, TEntity[] entities)
			where TEntity : class
			where TProperty : class
		{
			LoadReferencePropertyInternal_GetFromCache<TEntity, TProperty>(propertyName, entities, out List<object> keysToQuery);

			if ((keysToQuery != null) && keysToQuery.Any()) // zůstalo nám, na co se ptát do databáze?
			{
				List<TProperty> loadedProperties = LoadReferencePropertyInternal_GetQuery<TProperty>(keysToQuery).ToList();
				LoadReferencePropertyInternal_StoreToCache(loadedProperties);
			}

			return LoadReferencePropertyInternal_GetResult<TEntity, TProperty>(propertyName, entities);
			
		}

		/// <summary>
		/// Zajistí načtení vlastnosti, která je referencí (není kolkecí). Voláno reflexí.
		/// </summary>
		private async Task<LoadPropertyInternalResult> LoadReferencePropertyInternalAsync<TEntity, TProperty>(string propertyName, TEntity[] entities)
			where TEntity : class
			where TProperty : class
		{
			LoadReferencePropertyInternal_GetFromCache<TEntity, TProperty>(propertyName, entities, out List<object> keysToQuery);

			if ((keysToQuery != null) && keysToQuery.Any()) // zůstalo nám, na co se ptát do databáze?
			{
				List<TProperty> loadedProperties = await LoadReferencePropertyInternal_GetQuery<TProperty>(keysToQuery).ToListAsync();
				LoadReferencePropertyInternal_StoreToCache(loadedProperties);
			}

			return LoadReferencePropertyInternal_GetResult<TEntity, TProperty>(propertyName, entities);
		}

		private void LoadReferencePropertyInternal_GetFromCache<TEntity, TProperty>(string propertyName, TEntity[] entities, out List<object> keysToLoad)
			where TEntity : class
			where TProperty : class
		{
			IEnumerable<TEntity> entitiesNotInAddedState = entities.Where(item => dbContext.GetEntityState(item) != EntityState.Added);
			List<TEntity> entitiesToLoadReference = entitiesNotInAddedState.Where(entity => !IsEntityPropertyLoaded(entity, propertyName, false)).ToList();

			if (entitiesToLoadReference.Count > 0)
			{
				// získáme cizí klíč reprezentující referenci (Navigation)
				IProperty foreignKeyForReference = dbContext.Model.FindEntityType(typeof(TEntity)).FindNavigation(propertyName).ForeignKey.Properties.Single();

				// získáme klíče objektů, které potřebujeme načíst (z "běžných vlastností" nebo z shadow properties)
				// ignorujeme nenastavené reference (null)
				object[] foreignKeyValues = entitiesToLoadReference.Select(entity => dbContext.GetEntry(entity, true).CurrentValues[foreignKeyForReference]).Where(value => value != null).ToArray();

				IDbSet<TProperty> dbSet = dbContext.Set<TProperty>();

				List<TProperty> loadedReferences = new List<TProperty>(entitiesToLoadReference.Count);
				keysToLoad = new List<object>(entitiesToLoadReference.Count);

				foreach (object foreignKeyValue in foreignKeyValues)
				{
					// Čistě teoreticky nemusela dosud proběhnout detekce změn (resp. fixup), proto se musíme podívat do identity map před tím,
					// než budeme řešit cache (cache by se mohla pokoušet o vytažení objektu, který je již v identity mapě a došlo by ke kolizi).
					// Spoléháme na provedení fixupu pomocí changetrackeru.
					// Možnost tohoto scénáře se však nepodařilo po potvrdit, k této situaci nikdy nedojde.
					//TProperty trackedEntity = dbSet.FindTracked(foreignKeyValue);
					//if (trackedEntity != null)
					//{
					//	loadedReferences.Add(trackedEntity);
					//}
					//else
					if (entityCacheManager.TryGetEntity<TProperty>(foreignKeyValue, out TProperty cachedEntity))
					{
						loadedReferences.Add(cachedEntity);
					}
					else // není ani v identity mapě, ani v cache, hledáme v databázi
					{
						keysToLoad.Add(foreignKeyValue);
					}
				}
			}
			else
			{
				keysToLoad = null;
			}
		}

		private IQueryable<TProperty> LoadReferencePropertyInternal_GetQuery<TProperty>(List<object> keysToQuery)
			where TProperty : class
		{
			// získáme název vlastnosti primárního klíče třídy načítané vlastnosti (obvykle "Id")
			string propertyPrimaryKey = dbContext.Model.FindEntityType(typeof(TProperty)).FindPrimaryKey().Properties.Single().Name;

			// získáme query pro načtení objektů
			return dbContext.Set<TProperty>().AsQueryable().Where(p => keysToQuery.Contains(EF.Property<object>(p, propertyPrimaryKey)));
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

		private LoadPropertyInternalResult LoadReferencePropertyInternal_GetResult<TEntity, TProperty>(string propertyName, TEntity[] entities)
			where TEntity : class
			where TProperty : class
		{
			var propertyLambdaExpression = lambdaExpressionManager.GetPropertyLambdaExpression<TEntity, TProperty>(propertyName);
			// zde spoléháme na proběhnutí fixupu
			var loadedEntities = entities.Select(item => propertyLambdaExpression.LambdaCompiled(item)).Where(item => item != null).ToArray();
			return new LoadPropertyInternalResult
			{
				Entities = loadedEntities,
				FluentDataLoader = new DbFluentDataLoader<TProperty>(this, loadedEntities)
			};
		}

		/// <summary>
		/// Zajistí načtení vlastnosti, která je kolekcí. Voláno reflexí.
		/// </summary>
		private LoadPropertyInternalResult LoadCollectionPropertyInternal<TEntity, TPropertyCollection, TPropertyItem>(string propertyName, TEntity[] entities)
					where TEntity : class
					where TPropertyCollection : class
					where TPropertyItem : class
		{
			var propertyLambdaExpression = lambdaExpressionManager.GetPropertyLambdaExpression<TEntity, TPropertyCollection>(propertyName);

		    InitializeCollectionsForAddedEntities<TEntity, TPropertyCollection, TPropertyItem>(entities, propertyLambdaExpression.LambdaCompiled, propertyName);

			List<EntityPrimaryKeyWithValues> primaryKeyWithValues = GetEntitiesKeysToLoadProperty(entities, propertyName, true);

			if (primaryKeyWithValues != null)
			{
				IQueryable<TEntity> loadQuery = (IQueryable<TEntity>)GetLoadQuery(propertyLambdaExpression.LambdaExpression, primaryKeyWithValues, true);
				loadQuery.Load();
			}

			return new LoadPropertyInternalResult
			{
				Entities = entities.SelectMany(item => (IEnumerable<TPropertyItem>)propertyLambdaExpression.LambdaCompiled(item)).ToArray(),
				FluentDataLoader = new DbFluentDataLoader<TPropertyCollection>(this, entities.Select(item => propertyLambdaExpression.LambdaCompiled(item)).ToArray())
			};
		}

        /// <summary>
        /// Zajistí načtení vlastnosti, která je kolekcí. Voláno reflexí.
        /// </summary>
        private async Task<LoadPropertyInternalResult> LoadCollectionPropertyInternalAsync<TEntity, TPropertyCollection, TPropertyItem>(string propertyName, TEntity[] entities)
					where TEntity : class
					where TPropertyCollection : class
					where TPropertyItem : class
		{
			var propertyLambdaExpression = lambdaExpressionManager.GetPropertyLambdaExpression<TEntity, TPropertyCollection>(propertyName);

		    InitializeCollectionsForAddedEntities<TEntity, TPropertyCollection, TPropertyItem>(entities, propertyLambdaExpression.LambdaCompiled, propertyName);

            List<EntityPrimaryKeyWithValues> primaryKeyWithValues = GetEntitiesKeysToLoadProperty(entities, propertyName, true);

			if (primaryKeyWithValues != null)
			{
				IQueryable<TEntity> loadQuery = (IQueryable<TEntity>)GetLoadQuery(propertyLambdaExpression.LambdaExpression, primaryKeyWithValues, true);
				await loadQuery.LoadAsync();
			}

			return new LoadPropertyInternalResult
			{
				Entities = entities.SelectMany(item => (IEnumerable<TPropertyItem>)propertyLambdaExpression.LambdaCompiled(item)).ToArray(),
				FluentDataLoader = new DbFluentDataLoader<TPropertyCollection>(this, entities.Select(item => propertyLambdaExpression.LambdaCompiled(item)).ToArray())
			};
		}

		/// <summary>
		/// Vrátí seznam názvů primárních klíčů a jejich hodnot objektů, jejichž vlastnost má být načtena.
		/// Pokud není potřeba žádné objekty načítat, vrací null.
		/// </summary>
		protected virtual List<EntityPrimaryKeyWithValues> GetEntitiesKeysToLoadProperty<TEntity>(TEntity[] entities, string propertyName, bool isPropertyCollection)
			where TEntity : class
		{
			IEnumerable<TEntity> entitiesNotInAddedState = entities.Where(item => dbContext.GetEntityState(item) != EntityState.Added);
			List<TEntity> entitiesToLoadQuery = entitiesNotInAddedState.Where(entity => !IsEntityPropertyLoaded(entity, propertyName, isPropertyCollection)).ToList();

			if (!entitiesToLoadQuery.Any())
			{
				return null;
			}
			else
			{
				IKey primaryKey = dbContext.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey();

				return primaryKey.Properties.Select(primaryKeyProperty =>
						new EntityPrimaryKeyWithValues
						{
							PrimaryKeyName = primaryKeyProperty.Name,
							Values = entitiesToLoadQuery.Select(entity => (int)primaryKeyProperty.PropertyInfo.GetValue(entity)).ToList()
						})
					.ToList();
			}
		}


		/// <summary>
		/// Vrací true, pokud je vlastnost objektu již načtena.
		/// Řídí se pomocí IDbContext.IsEntityCollectionLoaded, DbContext.IsEntityReferenceLoaded.
		/// </summary>
		protected virtual bool IsEntityPropertyLoaded<TEntity>(TEntity entity, string propertyName, bool isPropertyCollection)
			where TEntity : class
		{
			return isPropertyCollection
				? dbContext.IsEntityCollectionLoaded(entity, propertyName)
				: dbContext.IsEntityReferenceLoaded(entity, propertyName);
		}

		/// <summary>
		/// Vrátí WHERE podmínku omezující množinu záznamů dle Id.
		/// </summary>
		/// <param name="primaryKeyWithValues">Identifikátory objektů, které mají být ve where klauzuli.</param>
		private Expression<Func<TEntity, bool>> GetWhereExpression<TEntity>(List<EntityPrimaryKeyWithValues> primaryKeyWithValues)
			where TEntity : class
		{
			Contract.Requires(primaryKeyWithValues != null);
			Contract.Requires(primaryKeyWithValues.Count > 0);

			ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "item");
			return ExpressionExt.AndAlso<TEntity>(primaryKeyWithValues.Select(primaryKeyWithValuesItem =>
				{
					// jediný záznam - testujeme na rovnost
					if (primaryKeyWithValuesItem.Values.Count == 1)
					{
						return (Expression<Func<TEntity, bool>>)Expression.Lambda(
							Expression.Equal(
								Expression.Property(parameter, typeof(TEntity), primaryKeyWithValuesItem.PrimaryKeyName),
								Expression.Constant(primaryKeyWithValuesItem.Values[0])),
							parameter);
					}

					// více záznamů
					// pokud jde o řadu IDček (1, 2, 3, 4) bez přeskakování, pak použijeme porovnání >= a  <=.
					int[] sortedIds = primaryKeyWithValuesItem.Values.OrderBy(item => item).Distinct().ToArray();

					//pro pole: 1, 2, 3, 4
					// if 1 + 4 - 1 (4) == 4
					if ((sortedIds[0] + sortedIds.Length - 1) == sortedIds[sortedIds.Length - 1]) // testujeme, zda jde o posloupnost IDček
					{
						return (Expression<Func<TEntity, bool>>)Expression.Lambda(
							Expression.AndAlso(
								Expression.GreaterThanOrEqual(Expression.Property(parameter, typeof(TEntity), primaryKeyWithValuesItem.PrimaryKeyName), Expression.Constant(sortedIds[0])),
								Expression.LessThanOrEqual(Expression.Property(parameter, typeof(TEntity), primaryKeyWithValuesItem.PrimaryKeyName), Expression.Constant(sortedIds[sortedIds.Length - 1]))),
							parameter);
					}

					// v obecném případě hledáme přes IN (...)
					return (Expression<Func<TEntity, bool>>)Expression.Lambda(
						Expression.Call(
							Expression.Constant(primaryKeyWithValuesItem.Values),
							typeof(List<int>).GetMethod("Contains"),
							new List<Expression> { Expression.Property(parameter, typeof(TEntity), primaryKeyWithValuesItem.PrimaryKeyName) }),
						parameter);
				})
				.ToArray()
			);
		}

		/// <summary>
		/// Vrátí dotaz načítající vlastnosti objektů s daným identifikátorem.
		/// </summary>
		/// <param name="propertyPath">Načítaná vlastnost.</param>
		/// <param name="keyValues">Identifikátory objektů, jejichž vlastnost má být načtena.</param>
		/// <param name="isPropertyCollection">True, pokud vlastost v propertyPath vyjadřuje kolekci.</param>
		private IQueryable GetLoadQuery<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertyPath, List<EntityPrimaryKeyWithValues> keyValues, bool isPropertyCollection)
			where TEntity : class
		{
			IQueryable loadQuery;

			//	jde o kolekci?
			if (isPropertyCollection)
			{
				loadQuery = dbContext.Set<TEntity>()
					.AsQueryable()
					.Where(GetWhereExpression<TEntity>(keyValues))
					.Include(propertyPath);
			}
			else
			{
				loadQuery = dbContext.Set<TEntity>()
					.AsQueryable()
					.Where(GetWhereExpression<TEntity>(keyValues))
					.Select(propertyPath);
			}
			return loadQuery;
		}

        /// <summary>
        /// Entity, které jsou ve stavu Added nemá cenu dotazovat do databáze, protože tam ještě nejsou.
        /// Tato metoda objektům ve stavu added inicializuje kolekce a nastavuje je jako Loaded.
        /// </summary>
	    private void InitializeCollectionsForAddedEntities<TEntity, TPropertyCollection, TPropertyItem>(TEntity[] entities, Func<TEntity, TPropertyCollection> propertyPathLambda, string propertyName)
	        where TEntity : class
	        where TPropertyCollection : class
	        where TPropertyItem : class
	    {
	        List<TEntity> addedNonLoadedEntities = entities.Where(item => (dbContext.GetEntityState<TEntity>(item) == EntityState.Added) && !dbContext.IsEntityCollectionLoaded<TEntity>(item, propertyName)).ToList();
	        List<TEntity> addedNonLoadedEntitiesWithNullEntity = addedNonLoadedEntities.Where(item => propertyPathLambda(item) == null).ToList();

	        if (addedNonLoadedEntitiesWithNullEntity.Count > 0)
	        {
	            if (typeof(TPropertyCollection) == typeof(List<TPropertyItem>) || typeof(TPropertyCollection) == typeof(IList<TPropertyItem>))
	            {
	                MethodInfo setter = typeof(TEntity).GetProperty(propertyName).GetSetMethod();
	                if (setter == null)
	                {
	                    throw new InvalidOperationException($"DataLoader on new objects (EntityState.Added) of type {typeof(TEntity).FullName} cannot set collection property {propertyName} while it does not have a public setter.");
	                }
	                foreach (var item in addedNonLoadedEntitiesWithNullEntity)
	                {
	                    setter.Invoke(item, new object[] { new List<TPropertyItem>() });
	                }
	            }
	            else
	            {
	                throw new InvalidOperationException($"DataLoader on new objects (EntityState.Added) of type {typeof(TEntity).FullName} cannot set collection property {propertyName} while it is not type of List<{typeof(TPropertyItem).Name}> or IList<{typeof(TPropertyItem).Name}.");
	            }
	        }

			// TODO JK: Dořešit.
	        // addedNonLoadedEntities.ForEach(item => dbContext.SetEntityCollectionLoaded<TEntity>(item, propertyName, true));
	    }		
	}
}
