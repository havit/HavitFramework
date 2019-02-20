using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.Patterns.DataLoaders;
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
		private LoadPropertyInternalResult LoadCollectionPropertyInternal<TEntity, TPropertyCollection, TPropertyItem>(string propertyName, TEntity[] entities)
			where TEntity : class
			where TPropertyCollection : class
			where TPropertyItem : class
		{
			var propertyLambdaExpression = lambdaExpressionManager.GetPropertyLambdaExpression<TEntity, TPropertyCollection>(propertyName);

			LoadCollectionPropertyInternal_InitializeCollectionsForAddedEntities<TEntity, TPropertyCollection, TPropertyItem>(entities, propertyLambdaExpression.LambdaCompiled, propertyName);

			List<EntityPrimaryKeyWithValues> primaryKeyWithValues = LoadCollectionPropertyInternal_GetEntitiesKeysToLoadProperty(entities, propertyName, true);

			if (primaryKeyWithValues != null)
			{
				IQueryable<TEntity> loadQuery = (IQueryable<TEntity>)LoadCollectionPropertyInternal_GetLoadQuery(propertyLambdaExpression.LambdaExpression, primaryKeyWithValues);
				loadQuery.Load();
			}

			return LoadCollectionPropertyInternal_GetResult<TEntity, TPropertyCollection, TPropertyItem>(entities, propertyLambdaExpression);
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

			LoadCollectionPropertyInternal_InitializeCollectionsForAddedEntities<TEntity, TPropertyCollection, TPropertyItem>(entities, propertyLambdaExpression.LambdaCompiled, propertyName);

			List<EntityPrimaryKeyWithValues> primaryKeyWithValues = LoadCollectionPropertyInternal_GetEntitiesKeysToLoadProperty(entities, propertyName, true);

			if (primaryKeyWithValues != null)
			{
				IQueryable<TEntity> loadQuery = (IQueryable<TEntity>)LoadCollectionPropertyInternal_GetLoadQuery(propertyLambdaExpression.LambdaExpression, primaryKeyWithValues);
				await loadQuery.LoadAsync().ConfigureAwait(false);
			}

			return LoadCollectionPropertyInternal_GetResult<TEntity, TPropertyCollection, TPropertyItem>(entities, propertyLambdaExpression);
		}

		/// <summary>
		/// Entity, které jsou ve stavu Added nemá cenu dotazovat do databáze, protože tam ještě nejsou.
		/// Tato metoda objektům ve stavu added inicializuje kolekce a nastavuje je jako Loaded.
		/// </summary>
		private void LoadCollectionPropertyInternal_InitializeCollectionsForAddedEntities<TEntity, TPropertyCollection, TPropertyItem>(TEntity[] entities, Func<TEntity, TPropertyCollection> propertyPathLambda, string propertyName)
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

		/// <summary>
		/// Vrátí seznam názvů primárních klíčů a jejich hodnot objektů, jejichž vlastnost má být načtena.
		/// Pokud není potřeba žádné objekty načítat, vrací null.
		/// </summary>
		protected virtual List<EntityPrimaryKeyWithValues> LoadCollectionPropertyInternal_GetEntitiesKeysToLoadProperty<TEntity>(TEntity[] entities, string propertyName, bool isPropertyCollection)
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
		/// Vrátí dotaz načítající vlastnosti objektů s daným identifikátorem.
		/// </summary>
		/// <param name="propertyPath">Načítaná vlastnost.</param>
		/// <param name="keyValues">Identifikátory objektů, jejichž vlastnost má být načtena.</param>
		private IQueryable LoadCollectionPropertyInternal_GetLoadQuery<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertyPath, List<EntityPrimaryKeyWithValues> keyValues)
			where TEntity : class
		{
			return dbContext.Set<TEntity>()
					.AsQueryable()
					.Where(LoadCollectionPropertyInternal_GetWhereExpression<TEntity>(keyValues))
					.Include(propertyPath);
		}

		/// <summary>
		/// Vrátí WHERE podmínku omezující množinu záznamů dle Id.
		/// </summary>
		/// <param name="primaryKeyWithValues">Identifikátory objektů, které mají být ve where klauzuli.</param>
		private Expression<Func<TEntity, bool>> LoadCollectionPropertyInternal_GetWhereExpression<TEntity>(List<EntityPrimaryKeyWithValues> primaryKeyWithValues)
			where TEntity : class
		{
			Contract.Requires(primaryKeyWithValues != null);
			Contract.Requires(primaryKeyWithValues.Count > 0);

			ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "item");
			return ExpressionExt.AndAlso<TEntity>(primaryKeyWithValues.Select(primaryKeyWithValuesItem =>
			{
				Expression<Func<TEntity, int>> propertyAccessor = item => EF.Property<int>(item, primaryKeyWithValuesItem.PrimaryKeyName);
				return primaryKeyWithValuesItem.Values.ContainsEffective(propertyAccessor);
			})
				.ToArray()
			);
		}

		private LoadPropertyInternalResult LoadCollectionPropertyInternal_GetResult<TEntity, TPropertyCollection, TPropertyItem>(TEntity[] entities, PropertyLambdaExpression<TEntity, TPropertyCollection> propertyLambdaExpression)
			where TEntity : class
			where TPropertyCollection : class
			where TPropertyItem : class
		{
			return new LoadPropertyInternalResult
			{
				Entities = entities.SelectMany(item => (IEnumerable<TPropertyItem>)propertyLambdaExpression.LambdaCompiled(item)).ToArray(),
				FluentDataLoader = new DbFluentDataLoader<TPropertyCollection>(this, entities.Select(item => propertyLambdaExpression.LambdaCompiled(item)).ToArray())
			};
		}
	}
}
