using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Havit.Data.Entity.Patterns.DataLoaders.Internal;
using Havit.Data.Entity.Patterns.Helpers;
using Havit.Data.Patterns.DataLoaders;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.Entity.Patterns.DataLoaders
{
	/// <summary>
	/// Explicity data loader.
	/// Načte hodnoty vlastnosti třídy, pokud ještě nejsou načteny.
	/// Podporováno je zřetězení (subjekt => subjekt.Adresa.Zeme.Svetadil) vč. varianty s kolekcemi, kdy je třeba použít AllItems (subjekt => subjekt.Adresy.AllItems().Zeme).
	/// </summary>
	public partial class DbDataLoader : IDataLoader, IDataLoaderAsync
	{
		private readonly IDbContext dbContext;
		private readonly IPropertyLambdaExpressionManager _lambdaExpressionManager;

		/// <summary>
		/// Konstructor.
		/// </summary>
		/// <param name="dbContext">DbContext, pomocí něhož budou objekty načítány.</param>
		/// <param name="lambdaExpressionManager">LambdaExpressionManager, pomocí něhož jsou získávány expression trees a kompilované expression trees pro lambda výrazy přístupu k vlastnostem objektů.</param>
		public DbDataLoader(IDbContext dbContext, IPropertyLambdaExpressionManager lambdaExpressionManager)
		{
			Contract.Requires(dbContext != null);

			this.dbContext = dbContext;
			this._lambdaExpressionManager = lambdaExpressionManager;
		}

		#region IDataLoader implementation (Load + LoadAll)

		/// <summary>
		/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
		/// </summary>
		/// <param name="entity">Objekt, jehož vlastnosti budou načteny.</param>
		/// <param name="propertyPaths">Vlastnostu, které mají být načteny.</param>
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
		/// <param name="propertyPaths">Vlastnostu, které mají být načteny.</param>
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
		/// <param name="propertyPaths">Vlastnostu, které mají být načteny.</param>
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
		/// <param name="propertyPaths">Vlastnostu, které mají být načteny.</param>
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
		private void LoadInternal<TEntity, TProperty>(IEnumerable<TEntity> entitiesToLoad, Expression<Func<TEntity, TProperty>> propertyPath)
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
				PropertyToLoad[] propertiesSequenceToLoad = GetPropertiesSequenceToLoad(propertyPath);

				Array entities = entitiesToLoadWithoutNulls;

				foreach (PropertyToLoad propertyToLoad in propertiesSequenceToLoad)
				{
					if (!propertyToLoad.IsCollection)
					{
						entities = (Array)typeof(DbDataLoader)
							.GetMethod(nameof(LoadReferencePropertyInternal), BindingFlags.Instance | BindingFlags.NonPublic)
							.MakeGenericMethod(
								propertyToLoad.SourceType,
								propertyToLoad.TargetType)
							.Invoke(this, new object[] { propertyToLoad.PropertyName, entities });
					}
					else
					{
						entities = (Array)typeof(DbDataLoader)
							.GetMethod(nameof(LoadCollectionPropertyInternal), BindingFlags.Instance | BindingFlags.NonPublic)
							.MakeGenericMethod(
								propertyToLoad.SourceType,
								propertyToLoad.TargetType,
								propertyToLoad.CollectionItemType)
							.Invoke(this, new object[] { propertyToLoad.PropertyName, entities });
					}

					if (entities.Length == 0)
					{
						break; // shortcut
					}
				}
			}
		}

		/// <summary>
		/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
		/// </summary>
		private async Task LoadInternalAsync<TEntity, TProperty>(IEnumerable<TEntity> entitiesToLoad, Expression<Func<TEntity, TProperty>> propertyPath)
			where TEntity : class
			where TProperty : class
		{
			// přeskočíme prázdné
			TEntity[] entitiesToLoadWithoutNulls = entitiesToLoad.Where(entity => entity != null).ToArray();

			if (entitiesToLoadWithoutNulls.Length > 0) // pokud máme, co načítat
			{
				// ověříme, že jsou všechny objekty sledované change trackerem (na který spoléháme)
				Contract.Assert<InvalidOperationException>(entitiesToLoadWithoutNulls.All(item => dbContext.GetEntityState(item) != EntityState.Detached), "DbDataLoader can be used only for objects tracked by a change tracker.");

				// vytáhnemep posloupnost vlastností, které budeme načítat
				PropertyToLoad[] propertiesSequenceToLoad = GetPropertiesSequenceToLoad(propertyPath);

				Array entities = entitiesToLoadWithoutNulls;
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
					entities = (Array)((dynamic)task).Result;

					if (entities.Length == 0)
					{
						break; // shortcut
					}
				}
			}
		}

		/// <summary>
		/// Vrátí sekvenci vlastostí v pořadí, v jakém jsou zapsány a jak budou načteny.
		/// </summary>
		private PropertyToLoad[] GetPropertiesSequenceToLoad<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertyPath)
					where TEntity : class
		{
			PropertiesSequenceExpressionVisitor visitor = new PropertiesSequenceExpressionVisitor();
			return visitor.GetPropertiesToLoad(propertyPath);
		}

		/// <summary>
		/// Zajistí načtení vlastnosti, která je referencí (není kolkecí). Voláno reflexí.
		/// </summary>
		private TProperty[] LoadReferencePropertyInternal<TEntity, TProperty>(string propertyName, TEntity[] entities)
			where TEntity : class
			where TProperty : class
		{
			var propertyLambdaExpression = _lambdaExpressionManager.GetPropertyLambdaExpression<TEntity, TProperty>(propertyName);

			List<int> ids = GetEntitiesIdsToLoadProperty(entities, propertyName, false);

			if (ids.Count > 0)
			{
				IQueryable loadQuery = GetLoadQuery(propertyLambdaExpression.LambdaExpression, ids, false);
				loadQuery.Load();
			}

			return entities.Select(item => propertyLambdaExpression.LambdaCompiled(item)).Where(item => item != null).ToArray();
		}

		/// <summary>
		/// Zajistí načtení vlastnosti, která je referencí (není kolkecí). Voláno reflexí.
		/// </summary>
		private async Task<TProperty[]> LoadReferencePropertyInternalAsync<TEntity, TProperty>(string propertyName, TEntity[] entities)
			where TEntity : class
			where TProperty : class
		{
			var propertyLambdaExpression = _lambdaExpressionManager.GetPropertyLambdaExpression<TEntity, TProperty>(propertyName);

			List<int> ids = GetEntitiesIdsToLoadProperty(entities, propertyName, false);

			if (ids.Count > 0)
			{
				IQueryable loadQuery = GetLoadQuery(propertyLambdaExpression.LambdaExpression, ids, false);
				await loadQuery.LoadAsync();
			}

			return entities.Select(item => propertyLambdaExpression.LambdaCompiled(item)).Where(item => item != null).ToArray();
		}

		/// <summary>
		/// Zajistí načtení vlastnosti, která je kolekcí. Voláno reflexí.
		/// </summary>
		private object[] LoadCollectionPropertyInternal<TEntity, TPropertyCollection, TPropertyItem>(string propertyName, TEntity[] entities)
					where TEntity : class
					where TPropertyCollection : class
					where TPropertyItem : class
		{
			var propertyLambdaExpression = _lambdaExpressionManager.GetPropertyLambdaExpression<TEntity, TPropertyCollection>(propertyName);

		    InitializeCollectionsForAddedEntities<TEntity, TPropertyCollection, TPropertyItem>(entities, propertyLambdaExpression.LambdaCompiled, propertyName);

			List<int> ids = GetEntitiesIdsToLoadProperty(entities, propertyName, true);

			if (ids.Count > 0)
			{
				IQueryable loadQuery = GetLoadQuery(propertyLambdaExpression.LambdaExpression, ids, true);
				loadQuery.Load();
			}

		    return entities.SelectMany(item => (IEnumerable<TPropertyItem>)propertyLambdaExpression.LambdaCompiled(item)).ToArray();
		}

        /// <summary>
        /// Zajistí načtení vlastnosti, která je kolekcí. Voláno reflexí.
        /// </summary>
        private async Task<TPropertyItem[]> LoadCollectionPropertyInternalAsync<TEntity, TPropertyCollection, TPropertyItem>(string propertyName, TEntity[] entities)
					where TEntity : class
					where TPropertyCollection : class
					where TPropertyItem : class
		{
			var propertyLambdaExpression = _lambdaExpressionManager.GetPropertyLambdaExpression<TEntity, TPropertyCollection>(propertyName);

		    InitializeCollectionsForAddedEntities<TEntity, TPropertyCollection, TPropertyItem>(entities, propertyLambdaExpression.LambdaCompiled, propertyName);

            List<int> ids = GetEntitiesIdsToLoadProperty(entities, propertyName, true);

			if (ids.Count > 0)
			{
				IQueryable loadQuery = GetLoadQuery(propertyLambdaExpression.LambdaExpression, ids, true);
				await loadQuery.LoadAsync();
			}

			TPropertyItem[] result = entities.SelectMany(item => (IEnumerable<TPropertyItem>)propertyLambdaExpression.LambdaCompiled(item)).ToArray();
			return result;
		}

	    /// <summary>
		/// Vrátí seznam Id objektů, jejichž vlastnost má být načtena.
		/// </summary>
		protected virtual List<int> GetEntitiesIdsToLoadProperty<TEntity>(TEntity[] entities, string propertyName, bool isPropertyCollection)
			where TEntity : class
	    {
	        IEnumerable<TEntity> entitiesNotInAddedState = entities.Where(item => dbContext.GetEntityState(item) != EntityState.Added);
		    IEnumerable<TEntity> entitiesToLoadQuery = entitiesNotInAddedState.Where(entity => !IsEntityPropertyLoaded(entity, propertyName, isPropertyCollection));			
			return entitiesToLoadQuery.Select(entity => EntityHelper.GetEntityId(entity)).Distinct().ToList();
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
		/// <param name="ids">Identifikátory objektů, které mají být ve where klauzuli.</param>
		private Expression<Func<TEntity, bool>> GetWhereExpression<TEntity>(List<int> ids)
			where TEntity : class
		{
			Contract.Requires(ids != null);
			Contract.Requires(ids.Count > 0);

			ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "item");

			// jediný záznam - testujeme na rovnost
			if (ids.Count == 1)
			{
				return (Expression<Func<TEntity, bool>>)Expression.Lambda(
					Expression.Equal(
						Expression.Property(parameter, typeof(TEntity), "Id"),
						Expression.Constant(ids[0])),
					parameter);
			}

			// více záznamů
			// pokud jde o řadu IDček (1, 2, 3, 4) bez přeskakování, pak použijeme porovnání >= a  <=.
			int[] sortedIds = ids.OrderBy(item => item).Distinct().ToArray();

			//pro pole: 1, 2, 3, 4
			// if 1 + 4 - 1 (4) == 4
			if ((sortedIds[0] + sortedIds.Length - 1) == sortedIds[sortedIds.Length - 1]) // testujeme, zda jde o posloupnost IDček
			{
				return (Expression<Func<TEntity, bool>>)Expression.Lambda(
					Expression.AndAlso(
						Expression.GreaterThanOrEqual(Expression.Property(parameter, typeof(TEntity), "Id"), Expression.Constant(sortedIds[0])),
						Expression.LessThanOrEqual(Expression.Property(parameter, typeof(TEntity), "Id"), Expression.Constant(sortedIds[sortedIds.Length - 1]))),
					parameter);
			}

			// v obecném případě hledáme přes IN (...)
			return (Expression<Func<TEntity, bool>>)Expression.Lambda(
				Expression.Call(
					Expression.Constant(ids),
					typeof(List<int>).GetMethod("Contains"),
					new List<Expression> { Expression.Property(parameter, typeof(TEntity), "Id") }),
				parameter);
		}

	    /// <summary>
		/// Vrátí dotaz načítající vlastnosti objektů s daným identifikátorem.
		/// </summary>
		/// <param name="propertyPath">Načítaná vlastnost.</param>
		/// <param name="ids">Identifikátory objektů, jejichž vlastnost má být načtena.</param>
		/// <param name="isPropertyCollection">True, pokud vlastost v propertyPath vyjadřuje kolekci.</param>
		private IQueryable GetLoadQuery<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertyPath, List<int> ids, bool isPropertyCollection)
			where TEntity : class
		{
			IQueryable loadQuery;

			//	jde o kolekci?
			if (isPropertyCollection)
			{
				loadQuery = dbContext.Set<TEntity>()
					.Where(GetWhereExpression<TEntity>(ids))
					.Include(propertyPath);
			}
			else
			{
				loadQuery = dbContext.Set<TEntity>()
					.Where(GetWhereExpression<TEntity>(ids))
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

	        addedNonLoadedEntities.ForEach(item => dbContext.SetEntityCollectionLoaded<TEntity>(item, propertyName, true));
	    }
	}
}
