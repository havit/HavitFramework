using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.ExceptionServices;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.PropertyLambdaExpressions.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Repositories;
using Havit.Data.Patterns.DataLoaders;
using Havit.Data.Patterns.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;

/// <summary>
/// Explicit data loader.
/// Načte hodnoty vlastnosti třídy, pokud ještě nejsou načteny.
/// Podporováno je zřetězení (subjekt => subjekt.Adresa.Zeme.Svetadil) vč. varianty s kolekcemi, kdy je třeba použít AllItems (subjekt => subjekt.Adresy.AllItems().Zeme).
/// Entity smí být typovány i interface za těchto předpokladů:
/// vlastnost předepsaná interfacem je entitou implementována implicitně (stejnojmenná public vlastnost),
/// všechny entity jednoho volání jsou téhož typu.
/// Pro prázdnou kolekci entit typovanou interfacem nedojde k žádnému načtení.
/// </summary>
public partial class DbDataLoader : IDataLoader
{
	// Otevřené generické metody se vyhledají reflexí jen jednou; uzavřené (MakeGenericMethod) se cachují per kombinace typových argumentů, aby se nevyráběly na každé načtení property.
	private static readonly MethodInfo s_loadReferencePropertyInternalMethod = typeof(DbDataLoader).GetMethod(nameof(LoadReferencePropertyInternal), BindingFlags.Instance | BindingFlags.NonPublic);
	private static readonly MethodInfo s_loadCollectionPropertyInternalMethod = typeof(DbDataLoader).GetMethod(nameof(LoadCollectionPropertyInternal), BindingFlags.Instance | BindingFlags.NonPublic);
	private static readonly MethodInfo s_loadReferencePropertyInternalAsyncMethod = typeof(DbDataLoader).GetMethod(nameof(LoadReferencePropertyInternalAsync), BindingFlags.Instance | BindingFlags.NonPublic);
	private static readonly MethodInfo s_loadCollectionPropertyInternalAsyncMethod = typeof(DbDataLoader).GetMethod(nameof(LoadCollectionPropertyInternalAsync), BindingFlags.Instance | BindingFlags.NonPublic);

	private static readonly MethodInfo s_loadInternalMethod = typeof(DbDataLoader).GetMethod(nameof(LoadInternal), BindingFlags.Instance | BindingFlags.NonPublic);
	private static readonly MethodInfo s_loadInternalAsyncMethod = typeof(DbDataLoader).GetMethod(nameof(LoadInternalAsync), BindingFlags.Instance | BindingFlags.NonPublic);

	private static readonly ConcurrentDictionary<(Type SourceType, Type TargetType), MethodInfo> s_loadReferencePropertyInternalGenericMethods = new();
	private static readonly ConcurrentDictionary<(Type SourceType, Type TargetType), MethodInfo> s_loadReferencePropertyInternalAsyncGenericMethods = new();
	private static readonly ConcurrentDictionary<(Type SourceType, Type TargetType, Type OriginalTargetType, Type CollectionItemType), MethodInfo> s_loadCollectionPropertyInternalGenericMethods = new();
	private static readonly ConcurrentDictionary<(Type SourceType, Type TargetType, Type OriginalTargetType, Type CollectionItemType), MethodInfo> s_loadCollectionPropertyInternalAsyncGenericMethods = new();
	private static readonly ConcurrentDictionary<(Type EntityType, Type PropertyType), MethodInfo> s_loadInternalGenericMethods = new();
	private static readonly ConcurrentDictionary<(Type EntityType, Type PropertyType), MethodInfo> s_loadInternalAsyncGenericMethods = new();

	private readonly IDbContext _dbContext;
	private readonly IPropertyLoadSequenceResolver _propertyLoadSequenceResolver;
	private readonly IPropertyLambdaExpressionManager _lambdaExpressionManager;
	private readonly IEntityCacheManager _entityCacheManager;
	private readonly IEntityKeyAccessor _entityKeyAccessor;
	private readonly ILoadedPropertyReader _loadedPropertyReader;
	private readonly ILogger<DbDataLoader> _logger;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	/// <param name="dbContext">DbContext, pomocí něhož budou objekty načítány.</param>
	/// <param name="propertyLoadSequenceResolver">Služba, která poskytne vlastnosti, které mají být načteny, a jejich pořadí.</param>
	/// <param name="lambdaExpressionManager">LambdaExpressionManager, pomocí něhož jsou získávány expression trees a kompilované expression trees pro lambda výrazy přístupu k vlastnostem objektů.</param>
	/// <param name="entityCacheManager">Zajišťuje získávání a ukládání entit z/do cache.</param>
	/// <param name="entityKeyAccessor">Zajišťuje získávání hodnot primárního klíče entit.</param>
	/// <param name="loadedPropertyReader">Zajišťuje získávání informace, zda byla již vlastnost načtena.</param>
	/// <param name="logger">Logger.</param>
	public DbDataLoader(IDbContext dbContext, IPropertyLoadSequenceResolver propertyLoadSequenceResolver, IPropertyLambdaExpressionManager lambdaExpressionManager, IEntityCacheManager entityCacheManager, IEntityKeyAccessor entityKeyAccessor, ILoadedPropertyReader loadedPropertyReader, ILogger<DbDataLoader> logger)
	{
		_dbContext = dbContext;
		_propertyLoadSequenceResolver = propertyLoadSequenceResolver;
		_lambdaExpressionManager = lambdaExpressionManager;
		_entityCacheManager = entityCacheManager;
		_entityKeyAccessor = entityKeyAccessor;
		_loadedPropertyReader = loadedPropertyReader;
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
		ArgumentNullException.ThrowIfNull(propertyPath);

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
		// TEntity může být interface implementovaný entitou modelu.
		// V takovém případě určíme skutečný typ entit, přepíšeme propertyPath na tento typ a načtení delegujeme zpět do LoadInternal uzavřeného nad skutečným typem.
		if (typeof(TEntity).IsInterface)
		{
			return LoadInternalWithParameterTypeSubstitution(distinctNotNullEntities, propertyPath);
		}

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
					MethodInfo loadReferencePropertyInternalMethod = s_loadReferencePropertyInternalGenericMethods.GetOrAdd(
						(propertyToLoad.SourceType, propertyToLoad.TargetType),
						static key => s_loadReferencePropertyInternalMethod.MakeGenericMethod(key.SourceType, key.TargetType));
					loadPropertyInternalResult = (LoadPropertyInternalResult)loadReferencePropertyInternalMethod.Invoke(this, invokeLoadReferencePropertyInternalMethodArguments);
				}
				catch (TargetInvocationException ex)
				{
					_logger.LogError(ex.InnerException, "Error while loading entity property.");
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
					MethodInfo loadCollectionPropertyInternalMethod = s_loadCollectionPropertyInternalGenericMethods.GetOrAdd(
						(propertyToLoad.SourceType, propertyToLoad.TargetType, propertyToLoad.OriginalTargetType, propertyToLoad.CollectionItemType),
						static key => s_loadCollectionPropertyInternalMethod.MakeGenericMethod(key.SourceType, key.TargetType, key.OriginalTargetType, key.CollectionItemType));
					loadPropertyInternalResult = (LoadPropertyInternalResult)loadCollectionPropertyInternalMethod.Invoke(this, invokeLoadCollectionPropertyInternalMethodArguments);
				}
				catch (TargetInvocationException ex)
				{
					_logger.LogError(ex.InnerException, "Error while loading entity property.");
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
		// TEntity může být interface implementovaný entitou modelu.
		// V takovém případě určíme skutečný typ entit, přepíšeme propertyPath na tento typ a načtení delegujeme zpět do LoadInternalAsync uzavřeného nad skutečným typem.
		if (typeof(TEntity).IsInterface)
		{
			return await LoadInternalWithParameterTypeSubstitutionAsync(distinctNotNullEntities, propertyPath, cancellationToken).ConfigureAwait(false);
		}

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
					MethodInfo loadReferencePropertyInternalAsyncMethod = s_loadReferencePropertyInternalAsyncGenericMethods.GetOrAdd(
						(propertyToLoad.SourceType, propertyToLoad.TargetType),
						static key => s_loadReferencePropertyInternalAsyncMethod.MakeGenericMethod(key.SourceType, key.TargetType));
					task = (ValueTask<LoadPropertyInternalResult>)loadReferencePropertyInternalAsyncMethod.Invoke(this, invokeLoadReferencePropertyInternalMethodArguments);
				}
				catch (TargetInvocationException ex)
				{
					_logger.LogError(ex.InnerException, "Error while loading entity property.");
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
					MethodInfo loadCollectionPropertyInternalAsyncMethod = s_loadCollectionPropertyInternalAsyncGenericMethods.GetOrAdd(
						(propertyToLoad.SourceType, propertyToLoad.TargetType, propertyToLoad.OriginalTargetType, propertyToLoad.CollectionItemType),
						static key => s_loadCollectionPropertyInternalAsyncMethod.MakeGenericMethod(key.SourceType, key.TargetType, key.OriginalTargetType, key.CollectionItemType));
					task = (ValueTask<LoadPropertyInternalResult>)loadCollectionPropertyInternalAsyncMethod.Invoke(this, invokeLoadCollectionPropertyInternalMethodArguments);
				}
				catch (TargetInvocationException ex)
				{
					_logger.LogError(ex.InnerException, "Error while loading entity property.");
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
	/// Zajistí načtení vlastností entit, které jsou typovány interface.
	/// Určí skutečný typ entit, přepíše propertyPath na tento typ a deleguje načtení do LoadInternal uzavřeného nad skutečným typem entit.
	/// </summary>
	private IFluentDataLoader<TProperty> LoadInternalWithParameterTypeSubstitution<TEntity, TProperty>(IEnumerable<TEntity> distinctNotNullEntities, Expression<Func<TEntity, TProperty>> propertyPath)
		where TEntity : class
		where TProperty : class
	{
		(Array entities, LambdaExpression substitutedPropertyPath) = LoadInternalWithParameterTypeSubstitution_PrepareArguments(distinctNotNullEntities, propertyPath);

		if (entities == null) // žádné entity - není z čeho určit skutečný typ, ale ani co načítat
		{
			return new NullFluentDataLoader<TProperty>();
		}

		MethodInfo loadInternalMethod = s_loadInternalGenericMethods.GetOrAdd(
			(entities.GetType().GetElementType(), typeof(TProperty)),
			static key => s_loadInternalMethod.MakeGenericMethod(key.EntityType, key.PropertyType));

		try
		{
			return (IFluentDataLoader<TProperty>)loadInternalMethod.Invoke(this, [entities, substitutedPropertyPath]);
		}
		catch (TargetInvocationException ex)
		{
			_logger.LogError(ex.InnerException, "Error while loading entity property.");
			ExceptionDispatchInfo.Throw(ex.InnerException);
			throw; // unreachable
		}
	}

	/// <summary>
	/// Zajistí načtení vlastností entit, které jsou typovány interface.
	/// Určí skutečný typ entit, přepíše propertyPath na tento typ a deleguje načtení do LoadInternalAsync uzavřeného nad skutečným typem entit.
	/// </summary>
	private async ValueTask<IFluentDataLoader<TProperty>> LoadInternalWithParameterTypeSubstitutionAsync<TEntity, TProperty>(IEnumerable<TEntity> distinctNotNullEntities, Expression<Func<TEntity, TProperty>> propertyPath, CancellationToken cancellationToken)
		where TEntity : class
		where TProperty : class
	{
		(Array entities, LambdaExpression substitutedPropertyPath) = LoadInternalWithParameterTypeSubstitution_PrepareArguments(distinctNotNullEntities, propertyPath);

		if (entities == null) // žádné entity - není z čeho určit skutečný typ, ale ani co načítat
		{
			return new NullFluentDataLoader<TProperty>();
		}

		MethodInfo loadInternalAsyncMethod = s_loadInternalAsyncGenericMethods.GetOrAdd(
			(entities.GetType().GetElementType(), typeof(TProperty)),
			static key => s_loadInternalAsyncMethod.MakeGenericMethod(key.EntityType, key.PropertyType));

		ValueTask<IFluentDataLoader<TProperty>> task = default;
		try
		{
			task = (ValueTask<IFluentDataLoader<TProperty>>)loadInternalAsyncMethod.Invoke(this, [entities, substitutedPropertyPath, cancellationToken]);
		}
		catch (TargetInvocationException ex)
		{
			_logger.LogError(ex.InnerException, "Error while loading entity property.");
			ExceptionDispatchInfo.Throw(ex.InnerException);
		}

		return await task.ConfigureAwait(false);
	}

	/// <summary>
	/// Připraví argumenty pro delegování načtení do LoadInternal(Async) uzavřeného nad skutečným typem entit:
	/// pole entit typované skutečným typem entit a propertyPath přepsaný na tento typ.
	/// Pokud nejsou žádné entity, vrací (null, null) - skutečný typ není z čeho určit, avšak není ani co načítat.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	/// Pokud entity nejsou téhož typu, pokud skutečný typ entit není entitou modelu
	/// nebo pokud vlastnost není na skutečném typu entit dostupná jako stejnojmenná public vlastnost.
	/// </exception>
	private (Array Entities, LambdaExpression PropertyPath) LoadInternalWithParameterTypeSubstitution_PrepareArguments<TEntity, TProperty>(IEnumerable<TEntity> distinctNotNullEntities, Expression<Func<TEntity, TProperty>> propertyPath)
		where TEntity : class
	{
		List<TEntity> entitiesList = distinctNotNullEntities.ToList();
		if (entitiesList.Count == 0)
		{
			return (null, null);
		}

		Type runtimeType = entitiesList[0].GetType();

		// kontrola homogenity - všechny entity musí být téhož typu (jinak bychom je nemohli načíst jedním průchodem nad jedním typem)
		for (int i = 1; i < entitiesList.Count; i++)
		{
			Type currentRuntimeType = entitiesList[i].GetType();
			if (currentRuntimeType != runtimeType)
			{
				throw new InvalidOperationException($"DataLoader cannot load properties of entities typed as {typeof(TEntity).FullName} while the entities are instances of different types ({runtimeType.FullName}, {currentRuntimeType.FullName}). All entities in a single call must be of the same type.");
			}
		}

		// určíme skutečný typ entity v modelu (FindRuntimeEntityType řeší i případné proxy či odvozené typy)
		Type entityClrType = _dbContext.Model.FindRuntimeEntityType(runtimeType)?.ClrType;
		if (entityClrType == null)
		{
			throw new InvalidOperationException($"DataLoader cannot load properties of entities typed as {typeof(TEntity).FullName} while their runtime type {runtimeType.FullName} is not an entity type of the model.");
		}

		// K zacyklení delegovaného volání LoadInternal(Async) nemůže dojít - entityClrType pochází z modelu, takže to není interface a substituce se v delegovaném volání znovu nespustí.
		LambdaExpression substitutedPropertyPath = new PropertyPathParameterTypeSubstitutionExpressionVisitor().SubstituteParameterType(propertyPath, entityClrType);

		Array entities = Array.CreateInstance(entityClrType, entitiesList.Count);
		for (int i = 0; i < entitiesList.Count; i++)
		{
			entities.SetValue(entitiesList[i], i);
		}

		return (entities, substitutedPropertyPath);
	}

	/// <summary>
	/// Vrací true, pokud je vlastnost objektu již načtena.
	/// </summary>
	private bool IsEntityPropertyLoaded<TEntity>(TEntity entity, string propertyName)
		where TEntity : class
	{
		return _loadedPropertyReader.IsEntityPropertyLoaded(entity, propertyName);
	}

	/// <summary>
	/// Vrací true, pokud má DataLoader použít chunking při načítání dat.
	/// </summary>
	private bool ShouldUseChunking(int valuesToLoadCount, out int chunkSize)
	{
		return _dbContext.ShouldUseChunkingForContainsCondition(valuesToLoadCount, out chunkSize);
	}
}
