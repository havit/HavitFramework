using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Havit.Data.Entity.Patterns.Helpers;
using Havit.Data.Patterns.DataLoaders;
using Havit.Data.Patterns.DataLoaders.Fluent;
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
		private readonly HashSet<object> alreadyLoadedCollectionInstances = new HashSet<object>();

		/// <summary>
		/// Konstructor.
		/// </summary>
		/// <param name="dbContext">DbContext, pomocí něhož budou objekty načítány.</param>
		public DbDataLoader(IDbContext dbContext)
		{
			Contract.Requires(dbContext != null);

			this.dbContext = dbContext;
		}

		#region IDataLoader implementation (Load + LoadAll)

		/// <summary>
		/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
		/// </summary>
		/// <param name="entity">Objekt, jehož vlastnosti budou načteny.</param>
		/// <param name="propertyPath">Vlastnost, která má být načtena.</param>
		public IDataLoaderFluent<TProperty> Load<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> propertyPath)
			where TEntity : class
			where TProperty : class
		{
			return LoadInternal(new TEntity[] { entity }, propertyPath);
		}

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
		/// <param name="propertyPath">Vlastnost, která má být načtena.</param>
		public IDataLoaderFluent<TProperty> LoadAll<TEntity, TProperty>(IEnumerable<TEntity> entities, Expression<Func<TEntity, TProperty>> propertyPath)
			where TEntity : class
			where TProperty : class
		{
			return LoadInternal(entities, propertyPath);
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
		/// <param name="propertyPath">Vlastnost, která má být načtena.</param>
		public Task<IDataLoaderFluentAsync<TProperty>> LoadAsync<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> propertyPath)
			where TEntity : class
			where TProperty : class
		{
			return LoadInternalAsync(new TEntity[] { entity }, propertyPath);
		}

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
		/// <param name="propertyPath">Vlastnost, která má být načtena.</param>
		public Task<IDataLoaderFluentAsync<TProperty>> LoadAllAsync<TEntity, TProperty>(IEnumerable<TEntity> entities, Expression<Func<TEntity, TProperty>> propertyPath)
			where TEntity : class
			where TProperty : class
		{
			return LoadInternalAsync(entities, propertyPath);
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
		private IDataLoaderFluent<TProperty> LoadInternal<TEntity, TProperty>(IEnumerable<TEntity> entitiesToLoad, Expression<Func<TEntity, TProperty>> propertyPath)
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

				foreach (PropertyToLoad propertyToLoad in propertiesSequenceToLoad)
				{
					if (!propertyToLoad.IsCollection)
					{
						entities = (Array)this.GetType()
							.GetMethod(nameof(LoadReferencePropertyInternal), BindingFlags.Instance | BindingFlags.NonPublic)
							.MakeGenericMethod(
								propertyToLoad.SourceType,
								propertyToLoad.TargetType)
							.Invoke(this, new object[] { propertyToLoad.PropertyName, entities });
					}
					else
					{
						entities = (Array)this.GetType()
							.GetMethod(nameof(LoadCollectionPropertyInternal), BindingFlags.Instance | BindingFlags.NonPublic)
							.MakeGenericMethod(
								propertyToLoad.SourceType,
								propertyToLoad.TargetType,
								propertyToLoad.CollectionItemType)
							.Invoke(this, new object[] { propertyToLoad.PropertyName, entities, propertyToLoad.CollectionUnwrapped });
					}

					if (entities.Length == 0)
					{
						break; // shortcut
					}
				}
				return new DataLoaderFluent<TProperty>(this, entities.Cast<TProperty>().ToArray());
			}
			else
			{
				return new DataLoaderFluent<TProperty>(this, new TProperty[] { });
			}
		}

		/// <summary>
		/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
		/// </summary>
		private async Task<IDataLoaderFluentAsync<TProperty>> LoadInternalAsync<TEntity, TProperty>(IEnumerable<TEntity> entitiesToLoad, Expression<Func<TEntity, TProperty>> propertyPath)
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
						task = (Task)this.GetType()
							.GetMethod(nameof(LoadReferencePropertyInternalAsync), BindingFlags.Instance | BindingFlags.NonPublic)
							.MakeGenericMethod(
								propertyToLoad.SourceType,
								propertyToLoad.TargetType)
							.Invoke(this, new object[] { propertyToLoad.PropertyName, entities });
					}
					else
					{
						task = (Task)this.GetType()
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

				return new DataLoaderFluentAsync<TProperty>(this, entities.Cast<TProperty>().ToArray());
			}
			else
			{
				return new DataLoaderFluentAsync<TProperty>(this, new TProperty[] { });
			}
		}

		/// <summary>
		/// Vrátí expression s lambda výrazem (TEntity item) => item.PropertyName (přičemž PropertyName je typu TProperty).
		/// </summary>
		private Expression<Func<TEntity, TProperty>> GetPropertyLambdaExpression<TEntity, TProperty>(string propertyName)
		{
			ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "item");
			return Expression.Lambda<Func<TEntity, TProperty>>(Expression.Property(parameter, propertyName), parameter);
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
			Expression<Func<TEntity, TProperty>> propertyPath = GetPropertyLambdaExpression<TEntity, TProperty>(propertyName);
			Func<TEntity, TProperty> propertyPathLambda = propertyPath.Compile();

			List<int> ids = GetEntitiesIdsToLoadProperty(entities, propertyPathLambda, false);

			if (ids.Count > 0)
			{
				IQueryable loadQuery = GetLoadQuery(propertyPath, ids, false);
				loadQuery.Load();
			}

			return entities.Select(item => propertyPathLambda(item)).ToArray();
		}

		/// <summary>
		/// Zajistí načtení vlastnosti, která je referencí (není kolkecí). Voláno reflexí.
		/// </summary>
		private async Task<TProperty[]> LoadReferencePropertyInternalAsync<TEntity, TProperty>(string propertyName, TEntity[] entities)
			where TEntity : class
			where TProperty : class
		{
			Expression<Func<TEntity, TProperty>> propertyPath = GetPropertyLambdaExpression<TEntity, TProperty>(propertyName);
			Func<TEntity, TProperty> propertyPathLambda = propertyPath.Compile();

			List<int> ids = GetEntitiesIdsToLoadProperty(entities, propertyPathLambda, false);

			if (ids.Count > 0)
			{
				IQueryable loadQuery = GetLoadQuery(propertyPath, ids, false);
				await loadQuery.LoadAsync();
			}

			return entities.Select(item => propertyPathLambda(item)).ToArray();
		}

		/// <summary>
		/// Zajistí načtení vlastnosti, která je kolekcí. Voláno reflexí.
		/// </summary>
		private object[] LoadCollectionPropertyInternal<TEntity, TPropertyCollection, TPropertyItem>(string propertyName, TEntity[] entities, bool unwrapCollection)
					where TEntity : class
					where TPropertyCollection : class
					where TPropertyItem : class
		{
			Expression<Func<TEntity, TPropertyCollection>> propertyPath = GetPropertyLambdaExpression<TEntity, TPropertyCollection>(propertyName);
			Func<TEntity, TPropertyCollection> propertyPathLambda = propertyPath.Compile();

			List<int> ids = GetEntitiesIdsToLoadProperty(entities, propertyPathLambda, true);

			if (ids.Count > 0)
			{
				IQueryable loadQuery = GetLoadQuery(propertyPath, ids, true);
				loadQuery.Load();
			}

			InsertLoadedCollectionInstancesToAlreadyLoadedCollectionInstances(entities, propertyPathLambda);
			if (unwrapCollection)
			{
				return entities.SelectMany(item => (IEnumerable<TPropertyItem>)propertyPathLambda(item)).ToArray();
			}
			else
			{
				return entities.Select(item => propertyPathLambda(item)).ToArray();
			}
		}

		/// <summary>
		/// Zajistí načtení vlastnosti, která je kolekcí. Voláno reflexí.
		/// </summary>
		private async Task<TPropertyItem[]> LoadCollectionPropertyInternalAsync<TEntity, TPropertyCollection, TPropertyItem>(string propertyName, TEntity[] entities)
					where TEntity : class
					where TPropertyCollection : class
					where TPropertyItem : class
		{
			Expression<Func<TEntity, TPropertyCollection>> propertyPath = GetPropertyLambdaExpression<TEntity, TPropertyCollection>(propertyName);
			Func<TEntity, TPropertyCollection> propertyPathLambda = propertyPath.Compile();

			List<int> ids = GetEntitiesIdsToLoadProperty(entities, propertyPathLambda, true);

			if (ids.Count > 0)
			{
				IQueryable loadQuery = GetLoadQuery(propertyPath, ids, true);
				await loadQuery.LoadAsync();
			}

			InsertLoadedCollectionInstancesToAlreadyLoadedCollectionInstances(entities, propertyPathLambda);

			TPropertyItem[] result = entities.SelectMany(item => (IEnumerable<TPropertyItem>)propertyPathLambda(item)).ToArray();
			return result;
		}

		/// <summary>
		/// Vrátí seznam Id objektů, jejichž vlastnost má být načtena.
		/// </summary>
		private List<int> GetEntitiesIdsToLoadProperty<TEntity, TProperty>(TEntity[] entities, Func<TEntity, TProperty> propertyPathLambda, bool isPropertyCollection)
			where TEntity : class
			where TProperty : class
		{
			IEnumerable<TEntity> entitiesToLoadQuery = isPropertyCollection
				? entities.Where(item => propertyPathLambda(item) == null || !alreadyLoadedCollectionInstances.Contains(propertyPathLambda(item)))
				: entities.Where(item => propertyPathLambda(item) == null);

			return entitiesToLoadQuery.Select(entity => EntityHelper.GetEntityId(entity)).Distinct().ToList();
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
		/// Vloží načtené instance kolekcí do AlreadyLoadedCollectionInstances.
		/// </summary>
		private void InsertLoadedCollectionInstancesToAlreadyLoadedCollectionInstances<TEntity, TProperty>(TEntity[] entities, Func<TEntity, TProperty> propertyPathLambda)
		{
			object[] collectionInstances = entities.Select(item => (object)propertyPathLambda(item)).ToArray();
			alreadyLoadedCollectionInstances.UnionWith(collectionInstances);
		}

	}
}
