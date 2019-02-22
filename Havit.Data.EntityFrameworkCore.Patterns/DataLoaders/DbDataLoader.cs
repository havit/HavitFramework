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
		private readonly IEntityKeyAccessor entityKeyAccessor;

		/// <summary>
		/// Konstructor.
		/// </summary>
		/// <param name="dbContext">DbContext, pomocí něhož budou objekty načítány.</param>
		/// <param name="propertyLoadSequenceResolver">Služba, která poskytne vlastnosti, které mají být načteny, a jejich pořadí.</param>
		/// <param name="lambdaExpressionManager">LambdaExpressionManager, pomocí něhož jsou získávány expression trees a kompilované expression trees pro lambda výrazy přístupu k vlastnostem objektů.</param>
		/// <param name="entityCacheManager">Zajišťuje získávání a ukládání entit z/do cache.</param>
		/// <param name="entityKeyAccessor">Zajišťuje získávání hodnot primárního klíče entit.</param>
		public DbDataLoader(IDbContext dbContext, IPropertyLoadSequenceResolver propertyLoadSequenceResolver, IPropertyLambdaExpressionManager lambdaExpressionManager, IEntityCacheManager entityCacheManager, IEntityKeyAccessor entityKeyAccessor)
		{
			Contract.Requires(dbContext != null);

			this.dbContext = dbContext;
			this.propertyLoadSequenceResolver = propertyLoadSequenceResolver;
			this.lambdaExpressionManager = lambdaExpressionManager;
			this.entityCacheManager = entityCacheManager;
			this.entityKeyAccessor = entityKeyAccessor;
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

			return await LoadInternalAsync(new TEntity[] { entity }, propertyPath).ConfigureAwait(false);
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
				await LoadInternalAsync(new TEntity[] { entity }, propertyPath).ConfigureAwait(false);
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
			return await LoadInternalAsync(entities, propertyPath).ConfigureAwait(false);
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
				await LoadInternalAsync(entities, propertyPath).ConfigureAwait(false);
			}
		}
		#endregion

		/// <summary>
		/// Deleguje načtení objektů do metody pro načtení referencí nebo metody pro načtení kolekce.
		/// </summary>
		private IFluentDataLoader<TProperty> LoadInternal<TEntity, TProperty>(IEnumerable<TEntity> entitiesToLoad, Expression<Func<TEntity, TProperty>> propertyPath)
			where TEntity : class
			where TProperty : class
		{
			// přeskočíme prázdné
			TEntity[] entitiesToLoadWithoutNulls = entitiesToLoad.Where(entity => entity != null).ToArray();

			if (entitiesToLoadWithoutNulls.Length == 0) // pokud ne máme, co načítat
			{
				return new DbFluentDataLoader<TProperty>(this, new TProperty[0]);
			}

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
						.Invoke(this, new object[] { propertyToLoad.PropertyName, propertyToLoad.OriginalPropertyName, entities });

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

		/// <summary>
		/// Deleguje načtení objektů do asynchronní metody pro načtení referencí nebo asynchronní metody pro načtení kolekce.
		/// </summary>
		private async Task<IFluentDataLoaderAsync<TProperty>> LoadInternalAsync<TEntity, TProperty>(IEnumerable<TEntity> entitiesToLoad, Expression<Func<TEntity, TProperty>> propertyPath)
			where TEntity : class
			where TProperty : class
		{
			// přeskočíme prázdné
			TEntity[] entitiesToLoadWithoutNulls = entitiesToLoad.Where(entity => entity != null).ToArray();

			if (entitiesToLoadWithoutNulls.Length == 0) // pokud ne máme, co načítat
			{
				return new DbFluentDataLoader<TProperty>(this, new TProperty[0]);
			}

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
						.Invoke(this, new object[] { propertyToLoad.PropertyName, propertyToLoad.OriginalPropertyName, entities });
				}
				await task.ConfigureAwait(false);
				LoadPropertyInternalResult loadPropertyInternalResult = (LoadPropertyInternalResult)((dynamic)task).Result; // task je již dokončen

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

		/// <summary>
		/// Vrací true, pokud je vlastnost objektu již načtena.
		/// Řídí se pomocí IDbContext.IsEntityCollectionLoaded, DbContext.IsEntityReferenceLoaded.
		/// Pozor na předefinování metody v potomku - DbDataLoaderWithLoadedPropertiesMemory. Díky tomu nesmí být tato metoda volána opakovaně (poprvé vrací skutečnou hodnotu, v dalších voláních vrací vždy true).
		/// </summary>
		protected virtual bool IsEntityPropertyLoaded<TEntity>(TEntity entity, string propertyName)
			where TEntity : class
		{
			return dbContext.IsNavigationLoaded(entity, propertyName);
		}
	}
}
