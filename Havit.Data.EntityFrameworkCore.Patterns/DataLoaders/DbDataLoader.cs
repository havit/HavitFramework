using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.ExceptionServices;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.PropertyLambdaExpressions.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Repositories;
using Havit.Data.Patterns.DataLoaders;
using Havit.Data.Patterns.Infrastructure;
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

	private readonly IDbContext _dbContext;
	private readonly IPropertyLoadSequenceResolver _propertyLoadSequenceResolver;
	private readonly IPropertyLambdaExpressionManager _lambdaExpressionManager;
	private readonly IEntityCacheManager _entityCacheManager;
	private readonly IEntityKeyAccessor _entityKeyAccessor;
	private readonly ILogger<DbDataLoader> _logger;

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
		_dbContext = dbContext;
		_propertyLoadSequenceResolver = propertyLoadSequenceResolver;
		_lambdaExpressionManager = lambdaExpressionManager;
		_entityCacheManager = entityCacheManager;
		_entityKeyAccessor = entityKeyAccessor;
		_logger = logger;
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
		if (entity == null)
		{
			return new NullFluentDataLoader<TProperty>();
		}

		DbDataLoaderHelpers.CheckEntityIsTracked(entity, _dbContext);
		TEntity[] distinctNotNullEntities = [entity];
		return LoadInternal(distinctNotNullEntities, propertyPath);
	}

	/// <summary>
	/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
	/// </summary>
	/// <param name="entity">Objekt, jehož vlastnosti budou načteny.</param>
	/// <param name="propertyPaths">Vlastnosti, které mají být načteny.</param>
	public void Load<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] propertyPaths)
		where TEntity : class
	{
		ArgumentNullException.ThrowIfNull(propertyPaths);
		ArgumentOutOfRangeException.ThrowIfZero(propertyPaths.Length);

		if (entity == null)
		{
			return;
		}

		DbDataLoaderHelpers.CheckEntityIsTracked(entity, _dbContext);
		TEntity[] distinctNotNullEntities = new TEntity[] { entity };
		foreach (Expression<Func<TEntity, object>> propertyPath in propertyPaths)
		{
			LoadInternal(distinctNotNullEntities, propertyPath);
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
		IEnumerable<TEntity> distinctNotNullEntities = entities.Where(item => item != null).Distinct().WithTrackedEntitiesCheck(_dbContext);
		return LoadInternal(distinctNotNullEntities, propertyPath);
	}

	/// <summary>
	/// Načte vlastnosti objektů, pokud ještě nejsou načteny.
	/// </summary>
	/// <param name="entities">Objekty, jejíž vlastnosti budou načteny.</param>
	/// <param name="propertyPaths">Vlastnosti, které mají být načteny.</param>
	public void LoadAll<TEntity>(IEnumerable<TEntity> entities, params Expression<Func<TEntity, object>>[] propertyPaths)
		where TEntity : class
	{
		ArgumentNullException.ThrowIfNull(entities);
		ArgumentNullException.ThrowIfNull(propertyPaths);
		ArgumentOutOfRangeException.ThrowIfZero(propertyPaths.Length);

		TEntity[] distinctNotNullEntities = entities.Where(item => item != null).Distinct().WithTrackedEntitiesCheck(_dbContext).ToArray(); // ToArray: Eliminace vícenásobné iterace v cyklu
		foreach (Expression<Func<TEntity, object>> propertyPath in propertyPaths)
		{
			LoadInternal(distinctNotNullEntities, propertyPath);
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
		ArgumentNullException.ThrowIfNull(propertyPath);

		if (entity == null)
		{
			return new NullFluentDataLoader<TProperty>();
		}

		DbDataLoaderHelpers.CheckEntityIsTracked(entity, _dbContext);
		TEntity[] distinctNotNullEntities = [entity];
		return await LoadInternalAsync(distinctNotNullEntities, propertyPath, cancellationToken).ConfigureAwait(false);
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
		ArgumentNullException.ThrowIfNull(propertyPaths);
		ArgumentOutOfRangeException.ThrowIfZero(propertyPaths.Length);

		if (entity == null)
		{
			return;
		}

		DbDataLoaderHelpers.CheckEntityIsTracked(entity, _dbContext);
		TEntity[] distinctNotNullEntities = new TEntity[] { entity };
		foreach (Expression<Func<TEntity, object>> propertyPath in propertyPaths)
		{
			await LoadInternalAsync(distinctNotNullEntities, propertyPath, cancellationToken).ConfigureAwait(false);
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
		ArgumentNullException.ThrowIfNull(entities);
		ArgumentNullException.ThrowIfNull(propertyPath);

		IEnumerable<TEntity> distinctNotNullEntities = entities.Where(item => item != null).Distinct().WithTrackedEntitiesCheck(_dbContext);
		return await LoadInternalAsync(distinctNotNullEntities, propertyPath, cancellationToken).ConfigureAwait(false);
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
		ArgumentNullException.ThrowIfNull(entities);
		ArgumentNullException.ThrowIfNull(propertyPaths);
		ArgumentOutOfRangeException.ThrowIfZero(propertyPaths.Length);

		TEntity[] distinctNotNullEntities = entities.Where(item => item != null).Distinct().WithTrackedEntitiesCheck(_dbContext).ToArray(); // ToArray: Eliminace vícenásobné iterace v cyklu
		foreach (Expression<Func<TEntity, object>> propertyPath in propertyPaths)
		{
			await LoadInternalAsync(distinctNotNullEntities, propertyPath, cancellationToken).ConfigureAwait(false);
		}
	}

	/// <summary>
	/// Deleguje načtení objektů do metody pro načtení referencí nebo metody pro načtení kolekce.
	/// </summary>
	private IFluentDataLoader<TProperty> LoadInternal<TEntity, TProperty>(IEnumerable<TEntity> distinctNotNullEntities, Expression<Func<TEntity, TProperty>> propertyPath)
		where TEntity : class
		where TProperty : class
	{
		// vytáhneme posloupnost vlastností, které budeme načítat
		PropertyToLoad[] propertiesSequenceToLoad = _propertyLoadSequenceResolver.GetPropertiesToLoad(propertyPath);
		string propertyPathString = propertyPath.ToString(); // ev. by šlo použít CallerArgumentExpression, ale je to breaking change do IDataLoader

		IEnumerable entities = distinctNotNullEntities;
		object fluentDataLoader = null;

		object[] invokeLoadReferencePropertyInternalMethodArguments = null;
		object[] invokeLoadCollectionPropertyInternalMethodArguments = null;
		foreach (PropertyToLoad propertyToLoad in propertiesSequenceToLoad)
		{
			_logger.LogDebug("Loading a property '{Property}' of entity '{Entity}'...", propertyToLoad.OriginalPropertyName, propertyToLoad.SourceType);

			LoadPropertyInternalResult loadPropertyInternalResult = default;

			if (!propertyToLoad.IsCollection)
			{
				invokeLoadReferencePropertyInternalMethodArguments ??= new object[3];
				invokeLoadReferencePropertyInternalMethodArguments[0] = propertyToLoad.PropertyName;
				invokeLoadReferencePropertyInternalMethodArguments[1] = entities; // IEnumerable<>
				invokeLoadReferencePropertyInternalMethodArguments[2] = propertyPathString;
				try
				{
					loadPropertyInternalResult = (LoadPropertyInternalResult)typeof(DbDataLoader)
						.GetMethod(nameof(LoadReferencePropertyInternal), BindingFlags.Instance | BindingFlags.NonPublic)
						.MakeGenericMethod(
							propertyToLoad.SourceType,
							propertyToLoad.TargetType)
						.Invoke(this, invokeLoadReferencePropertyInternalMethodArguments);
				}
				catch (TargetInvocationException ex)
				{
					_logger.LogError(ex.Message);
					ExceptionDispatchInfo.Throw(ex.InnerException);
				}
			}
			else
			{
				invokeLoadCollectionPropertyInternalMethodArguments ??= new object[4];
				invokeLoadCollectionPropertyInternalMethodArguments[0] = propertyToLoad.PropertyName;
				invokeLoadCollectionPropertyInternalMethodArguments[1] = propertyToLoad.OriginalPropertyName;
				invokeLoadCollectionPropertyInternalMethodArguments[2] = entities; // IEnumerable<>
				invokeLoadCollectionPropertyInternalMethodArguments[3] = propertyPathString;
				try
				{
					loadPropertyInternalResult = (LoadPropertyInternalResult)typeof(DbDataLoader)
						.GetMethod(nameof(LoadCollectionPropertyInternal), BindingFlags.Instance | BindingFlags.NonPublic)
						.MakeGenericMethod(
							propertyToLoad.SourceType,
							propertyToLoad.TargetType,
							propertyToLoad.OriginalTargetType,
							propertyToLoad.CollectionItemType)
						.Invoke(this, invokeLoadCollectionPropertyInternalMethodArguments);
				}
				catch (TargetInvocationException ex)
				{
					_logger.LogError(ex.Message);
					ExceptionDispatchInfo.Throw(ex.InnerException);
				}
			}

			entities = loadPropertyInternalResult.Entities;
			fluentDataLoader = loadPropertyInternalResult.FluentDataLoader;

			_logger.LogDebug("Property '{Property}' of entity '{Entity}' loaded.", propertyToLoad.OriginalPropertyName, propertyToLoad.SourceType);
		}

		return (IFluentDataLoader<TProperty>)fluentDataLoader;
	}

	/// <summary>
	/// Deleguje načtení objektů do asynchronní metody pro načtení referencí nebo asynchronní metody pro načtení kolekce.
	/// </summary>
	private async ValueTask<IFluentDataLoader<TProperty>> LoadInternalAsync<TEntity, TProperty>(IEnumerable<TEntity> distinctNotNullEntities, Expression<Func<TEntity, TProperty>> propertyPath, CancellationToken cancellationToken)
		where TEntity : class
		where TProperty : class
	{
		// vytáhneme posloupnost vlastností, které budeme načítat
		PropertyToLoad[] propertiesSequenceToLoad = _propertyLoadSequenceResolver.GetPropertiesToLoad(propertyPath);
		string propertyPathString = propertyPath.ToString(); // ev. by šlo použít CallerArgumentExpression, ale je to breaking change do IDataLoader
		IEnumerable entities = distinctNotNullEntities;
		object fluentDataLoader = null;

		object[] invokeLoadReferencePropertyInternalMethodArguments = null;
		object[] invokeLoadCollectionPropertyInternalMethodArguments = null;

		foreach (PropertyToLoad propertyToLoad in propertiesSequenceToLoad)
		{
			_logger.LogDebug("Loading a property '{Property}' of entity '{Entity}'...", propertyToLoad.OriginalPropertyName, propertyToLoad.SourceType);

			ValueTask<LoadPropertyInternalResult> task = default;
			if (!propertyToLoad.IsCollection)
			{
				invokeLoadReferencePropertyInternalMethodArguments ??= new object[4];
				invokeLoadReferencePropertyInternalMethodArguments[0] = propertyToLoad.PropertyName;
				invokeLoadReferencePropertyInternalMethodArguments[1] = entities; // IEnumerable<>
				invokeLoadReferencePropertyInternalMethodArguments[2] = propertyPathString;
				invokeLoadReferencePropertyInternalMethodArguments[3] = cancellationToken;
				try
				{
					task = (ValueTask<LoadPropertyInternalResult>)typeof(DbDataLoader)
						.GetMethod(nameof(LoadReferencePropertyInternalAsync), BindingFlags.Instance | BindingFlags.NonPublic)
						.MakeGenericMethod(
							propertyToLoad.SourceType,
							propertyToLoad.TargetType)
						.Invoke(this, invokeLoadReferencePropertyInternalMethodArguments);
				}
				catch (TargetInvocationException ex)
				{
					_logger.LogError(ex.Message);
					ExceptionDispatchInfo.Throw(ex.InnerException);
				}
			}
			else
			{
				invokeLoadCollectionPropertyInternalMethodArguments ??= new object[5];
				invokeLoadCollectionPropertyInternalMethodArguments[0] = propertyToLoad.PropertyName;
				invokeLoadCollectionPropertyInternalMethodArguments[1] = propertyToLoad.OriginalPropertyName;
				invokeLoadCollectionPropertyInternalMethodArguments[2] = entities; // IEnumerable<>
				invokeLoadCollectionPropertyInternalMethodArguments[3] = propertyPathString;
				invokeLoadCollectionPropertyInternalMethodArguments[4] = cancellationToken;
				try
				{
					task = (ValueTask<LoadPropertyInternalResult>)typeof(DbDataLoader)
						.GetMethod(nameof(LoadCollectionPropertyInternalAsync), BindingFlags.Instance | BindingFlags.NonPublic)
						.MakeGenericMethod(
							propertyToLoad.SourceType,
							propertyToLoad.TargetType,
							propertyToLoad.OriginalTargetType,
							propertyToLoad.CollectionItemType)
						.Invoke(this, invokeLoadCollectionPropertyInternalMethodArguments);
				}
				catch (TargetInvocationException ex)
				{
					_logger.LogError(ex.Message);
					ExceptionDispatchInfo.Throw(ex.InnerException);
				}
			}

			LoadPropertyInternalResult loadPropertyInternalResult = await task.ConfigureAwait(false);

			entities = loadPropertyInternalResult.Entities;
			fluentDataLoader = loadPropertyInternalResult.FluentDataLoader;

			_logger.LogDebug("Property '{Property}' of entity '{Entity}' loaded.", propertyToLoad.OriginalPropertyName, propertyToLoad.SourceType);
		}

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
		return _dbContext.IsNavigationLoaded(entity, propertyName);
	}
}
