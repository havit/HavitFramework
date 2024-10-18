﻿using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.ExceptionServices;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.PropertyLambdaExpressions.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Repositories;
using Havit.Data.Patterns.DataLoaders;
using Havit.Data.Patterns.Infrastructure;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;

/// <summary>
/// Explicity data loader.
/// Načte hodnoty vlastnosti třídy, pokud ještě nejsou načteny.
/// Podporováno je zřetězení (subjekt => subjekt.Adresa.Zeme.Svetadil) vč. varianty s kolekcemi, kdy je třeba použít AllItems (subjekt => subjekt.Adresy.AllItems().Zeme).
/// </summary>
public partial class DbDataLoader : IDataLoader
{
	/// <summary>
	/// Počet entit, ke kterým jsou načítána data jedním dotazem.
	/// Ovlivňuje maximální počet hodnot v WHERE Id IN (...) s SQL statementu.
	/// Viz komentář v <see cref="LoadCollectionPropertyInternal" />.
	/// </summary>
	internal const int ChunkSize = DbRepository<object>.GetObjectsChunkSize;

	private readonly IDbContext dbContext;
	private readonly IPropertyLoadSequenceResolver propertyLoadSequenceResolver;
	private readonly IPropertyLambdaExpressionManager lambdaExpressionManager;
	private readonly IEntityCacheManager entityCacheManager;
	private readonly IEntityKeyAccessor entityKeyAccessor;
	private readonly ILogger<DbDataLoader> logger;

	/// <summary>
	/// Konstructor.
	/// </summary>
	/// <param name="dbContext">DbContext, pomocí něhož budou objekty načítány.</param>
	/// <param name="propertyLoadSequenceResolver">Služba, která poskytne vlastnosti, které mají být načteny, a jejich pořadí.</param>
	/// <param name="lambdaExpressionManager">LambdaExpressionManager, pomocí něhož jsou získávány expression trees a kompilované expression trees pro lambda výrazy přístupu k vlastnostem objektů.</param>
	/// <param name="entityCacheManager">Zajišťuje získávání a ukládání entit z/do cache.</param>
	/// <param name="entityKeyAccessor">Zajišťuje získávání hodnot primárního klíče entit.</param>
	/// <param name="logger">Logger.</param>
	public DbDataLoader(IDbContext dbContext, IPropertyLoadSequenceResolver propertyLoadSequenceResolver, IPropertyLambdaExpressionManager lambdaExpressionManager, IEntityCacheManager entityCacheManager, IEntityKeyAccessor entityKeyAccessor, ILogger<DbDataLoader> logger)
	{
		Contract.Requires(dbContext != null);

		this.dbContext = dbContext;
		this.propertyLoadSequenceResolver = propertyLoadSequenceResolver;
		this.lambdaExpressionManager = lambdaExpressionManager;
		this.entityCacheManager = entityCacheManager;
		this.entityKeyAccessor = entityKeyAccessor;
		this.logger = logger;
	}

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
		Contract.Requires(propertyPaths.Length > 0);

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
		Contract.Requires(propertyPaths.Length > 0);

		foreach (Expression<Func<TEntity, object>> propertyPath in propertyPaths)
		{
			LoadInternal(entities, propertyPath);
		}
	}

	/// <summary>
	/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
	/// </summary>
	/// <param name="entity">Objekt, jehož vlastnosti budou načteny.</param>
	/// <param name="propertyPath">Vlastnost, která má být načtena.</param>
	/// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
	public async Task<IFluentDataLoader<TProperty>> LoadAsync<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> propertyPath, CancellationToken cancellationToken = default)
		where TEntity : class
		where TProperty : class
	{
		Contract.Requires(propertyPath != null);

		return await LoadInternalAsync(new TEntity[] { entity }, propertyPath, cancellationToken).ConfigureAwait(false);
	}

	/// <summary>
	/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
	/// </summary>
	/// <param name="entity">Objekt, jehož vlastnosti budou načteny.</param>
	/// <param name="propertyPaths">Vlastnosti, které mají být načteny.</param>
	/// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
	public async Task LoadAsync<TEntity>(TEntity entity, Expression<Func<TEntity, object>>[] propertyPaths, CancellationToken cancellationToken = default)
		where TEntity : class
	{
		Contract.Requires(propertyPaths != null);
		Contract.Requires(propertyPaths.Length > 0);

		foreach (Expression<Func<TEntity, object>> propertyPath in propertyPaths)
		{
			await LoadInternalAsync(new TEntity[] { entity }, propertyPath, cancellationToken).ConfigureAwait(false);
		}
	}

	/// <summary>
	/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
	/// </summary>
	/// <param name="entities">Objekty, jejíž vlastnosti budou načteny.</param>
	/// <param name="propertyPath">Vlastnost, který má být načtena.</param>
	/// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
	public async Task<IFluentDataLoader<TProperty>> LoadAllAsync<TEntity, TProperty>(IEnumerable<TEntity> entities, Expression<Func<TEntity, TProperty>> propertyPath, CancellationToken cancellationToken = default)
		where TEntity : class
		where TProperty : class
	{
		Contract.Requires(entities != null);
		Contract.Requires(propertyPath != null);
		return await LoadInternalAsync(entities, propertyPath, cancellationToken).ConfigureAwait(false);
	}

	/// <summary>
	/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
	/// </summary>
	/// <param name="entities">Objekty, jejíž vlastnosti budou načteny.</param>
	/// <param name="propertyPaths">Vlastnosti, které mají být načteny.</param>
	/// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
	public async Task LoadAllAsync<TEntity>(IEnumerable<TEntity> entities, Expression<Func<TEntity, object>>[] propertyPaths, CancellationToken cancellationToken = default)
		where TEntity : class
	{
		Contract.Requires(propertyPaths != null);
		Contract.Requires(propertyPaths.Length > 0);

		// TODO EFCore9: Pozor, tady máme několik enumerací entities!!!

		foreach (Expression<Func<TEntity, object>> propertyPath in propertyPaths)
		{
			await LoadInternalAsync(entities, propertyPath, cancellationToken).ConfigureAwait(false);
		}
	}

	/// <summary>
	/// Deleguje načtení objektů do metody pro načtení referencí nebo metody pro načtení kolekce.
	/// </summary>
	private IFluentDataLoader<TProperty> LoadInternal<TEntity, TProperty>(IEnumerable<TEntity> entitiesToLoad, Expression<Func<TEntity, TProperty>> propertyPath)
		where TEntity : class
		where TProperty : class
	{
		LogDebug("Processing {0} entities.", args: entitiesToLoad.Count());

		// přeskočíme prázdné
		TEntity[] entitiesToLoadWithoutNulls = entitiesToLoad.Where(entity => entity != null).ToArray();

		if (entitiesToLoadWithoutNulls.Length == 0) // pokud ne máme, co načítat
		{
			LogDebug("Exiting, there is nothing to load.");
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
			LogDebug("Loading a property '{0}'.", args: propertyToLoad.OriginalPropertyName);

			LoadPropertyInternalResult loadPropertyInternalResult = default;

			if (!propertyToLoad.IsCollection)
			{
				try
				{
					loadPropertyInternalResult = (LoadPropertyInternalResult)typeof(DbDataLoader)
						.GetMethod(nameof(LoadReferencePropertyInternal), BindingFlags.Instance | BindingFlags.NonPublic)
						.MakeGenericMethod(
							propertyToLoad.SourceType,
							propertyToLoad.TargetType)
						.Invoke(this, new object[] { propertyToLoad.PropertyName, entities });
				}
				catch (TargetInvocationException ex)
				{
					LogDebug("Exception: {0}", args: ex.Message);
					ExceptionDispatchInfo.Throw(ex.InnerException);
				}
			}
			else
			{
				try
				{
					loadPropertyInternalResult = (LoadPropertyInternalResult)typeof(DbDataLoader)
						.GetMethod(nameof(LoadCollectionPropertyInternal), BindingFlags.Instance | BindingFlags.NonPublic)
						.MakeGenericMethod(
							propertyToLoad.SourceType,
							propertyToLoad.TargetType,
							propertyToLoad.OriginalTargetType,
							propertyToLoad.CollectionItemType)
						.Invoke(this, new object[] { propertyToLoad.PropertyName, propertyToLoad.OriginalPropertyName, entities });
				}
				catch (TargetInvocationException ex)
				{
					LogDebug("Exception: {0}", args: ex.Message);
					ExceptionDispatchInfo.Throw(ex.InnerException);
				}
			}

			entities = loadPropertyInternalResult.Entities;
			fluentDataLoader = loadPropertyInternalResult.FluentDataLoader;

			if (entities.Length == 0)
			{
				// shortcut
				LogDebug("Returning via shortcut.");
				return new DbFluentDataLoader<TProperty>(this, new TProperty[0]);
			}
		}

		LogDebug("Returning.");
		return (IFluentDataLoader<TProperty>)fluentDataLoader;
	}

	/// <summary>
	/// Deleguje načtení objektů do asynchronní metody pro načtení referencí nebo asynchronní metody pro načtení kolekce.
	/// </summary>
	private async Task<IFluentDataLoader<TProperty>> LoadInternalAsync<TEntity, TProperty>(IEnumerable<TEntity> entitiesToLoad, Expression<Func<TEntity, TProperty>> propertyPath, CancellationToken cancellationToken)
		where TEntity : class
		where TProperty : class
	{
		LogDebug("Processing {0} entities.", args: entitiesToLoad.Count());
		// přeskočíme prázdné
		TEntity[] entitiesToLoadWithoutNulls = entitiesToLoad.Where(entity => entity != null).ToArray();

		if (entitiesToLoadWithoutNulls.Length == 0) // pokud ne máme, co načítat
		{
			LogDebug("Exiting, there is nothing to load.");
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
			LogDebug("Loading a property '{0}'.", args: propertyToLoad.OriginalPropertyName);

			ValueTask<LoadPropertyInternalResult> task = default;
			if (!propertyToLoad.IsCollection)
			{
				try
				{
					task = (ValueTask<LoadPropertyInternalResult>)typeof(DbDataLoader)
						.GetMethod(nameof(LoadReferencePropertyInternalAsync), BindingFlags.Instance | BindingFlags.NonPublic)
						.MakeGenericMethod(
							propertyToLoad.SourceType,
							propertyToLoad.TargetType)
						.Invoke(this, new object[] { propertyToLoad.PropertyName, entities, cancellationToken });
				}
				catch (TargetInvocationException ex)
				{
					LogDebug("Exception: {0}", args: ex.Message);
					ExceptionDispatchInfo.Throw(ex.InnerException);
				}
			}
			else
			{
				try
				{
					task = (ValueTask<LoadPropertyInternalResult>)typeof(DbDataLoader)
						.GetMethod(nameof(LoadCollectionPropertyInternalAsync), BindingFlags.Instance | BindingFlags.NonPublic)
						.MakeGenericMethod(
							propertyToLoad.SourceType,
							propertyToLoad.TargetType,
							propertyToLoad.OriginalTargetType,
							propertyToLoad.CollectionItemType)
						.Invoke(this, new object[] { propertyToLoad.PropertyName, propertyToLoad.OriginalPropertyName, entities, cancellationToken });
				}
				catch (TargetInvocationException ex)
				{
					LogDebug("Exception: {0}", args: ex.Message);
					ExceptionDispatchInfo.Throw(ex.InnerException);
				}
			}

			LoadPropertyInternalResult loadPropertyInternalResult = await task.ConfigureAwait(false);

			entities = loadPropertyInternalResult.Entities;
			fluentDataLoader = loadPropertyInternalResult.FluentDataLoader;

			if (entities.Length == 0)
			{
				// shortcut
				LogDebug("Returning via shortcut.");
				return new DbFluentDataLoader<TProperty>(this, new TProperty[0]);
			}
		}

		LogDebug("Returning.");
		return (IFluentDataLoader<TProperty>)fluentDataLoader;
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

	private void LogDebug(string message, [System.Runtime.CompilerServices.CallerMemberName] string caller = null, params object[] args)
	{
		logger.LogDebug(String.Concat(caller, "[", this.GetHashCode(), "]: ", message), args);
	}
}
