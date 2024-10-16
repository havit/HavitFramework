using System.Reflection;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.DataSeeds;
using Havit.Data.EntityFrameworkCore.Patterns.DataSeeds.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.DataSources;
using Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Infrastructure.Factories;
using Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.Lookups;
using Havit.Data.EntityFrameworkCore.Patterns.PropertyLambdaExpressions.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Repositories;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.EntityValidation;
using Havit.Data.Patterns.Attributes;
using Havit.Data.Patterns.DataEntries;
using Havit.Data.Patterns.DataLoaders;
using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.DataSources;
using Havit.Data.Patterns.Infrastructure;
using Havit.Data.Patterns.Localizations;
using Havit.Data.Patterns.Localizations.Internal;
using Havit.Data.Patterns.Repositories;
using Havit.Data.Patterns.UnitOfWorks;
using Havit.Model.Localizations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection;

/// <summary>
/// Installer Havit.Data.Entity.Patterns a souvisejících služeb.
/// </summary>
public class EntityPatternsInstaller
{
	private readonly IServiceCollection _services;
	private readonly ComponentRegistrationOptions _componentRegistrationOptions;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public EntityPatternsInstaller(IServiceCollection services, ComponentRegistrationOptions componentRegistrationOptions)
	{
		this._services = services;
		this._componentRegistrationOptions = componentRegistrationOptions;
	}

	/// <summary>
	/// Registruje do DI containeru DbContext pod interface IDbContext (scoped lifestyle), DbContextFactory&lt;TDbContext&gt; (singleton) a IDbContextFactory (singleton).
	/// </summary>
	/// <remarks>
	/// DbContext a DbContextFactory&lt;TDbContext&gt; jsou z Entity Framework Core.
	/// IDbContext a IDbContextFactory (negenerická!) jsou z HFW.
	/// </remarks>
	public EntityPatternsInstaller AddDbContext<TDbContext>()
		where TDbContext : Havit.Data.EntityFrameworkCore.DbContext, IDbContext
	{
		return AddDbContext<TDbContext>((Action<DbContextOptionsBuilder>)null);
	}

	/// <summary>
	/// Registruje do DI containeru DbContext pod interface IDbContext (scoped lifestyle), DbContextFactory&lt;TDbContext&gt; (singleton) a IDbContextFactory (singleton).
	/// Dále registruje DbContextOptions&lt;TDbContext&gt; jako singleton.
	/// </summary>
	/// <remarks>
	/// DbContext a DbContextFactory&lt;TDbContext&gt; jsou z Entity Framework Core.
	/// IDbContext a IDbContextFactory (negenerická!) jsou z HFW.
	/// DbContextOptions&lt;TDbContext&gt; jsou v Entity Framework Core ve výchozím chování registrovány jako Scoped. To však brání možnosti
	/// použití jak DbContext, tak DbContextFactory&lt;TDbContext&gt;. Pro podporu obou potřebujeme DbContextOptions&lt;TDbContext&gt; registrovat jako singleton.
	/// </remarks>
	public EntityPatternsInstaller AddDbContext<TDbContext>(Action<DbContextOptionsBuilder> optionsAction = null)
		where TDbContext : Havit.Data.EntityFrameworkCore.DbContext, IDbContext
	{
		_services.AddDbContextFactory<TDbContext>();
		_services.AddDbContext<IDbContext, TDbContext>(GetDbContextOptionsBuilder(optionsAction), optionsLifetime: ServiceLifetime.Singleton);

		_services.TryAddTransient<IDbContextFactory, DbContextFactory<TDbContext>>();
		return this;
	}

	/// <summary>
	/// Registruje do DI containeru DbContext pod interface IDbContext (scoped lifestyle), DbContextFactory&lt;TDbContext&gt; (singleton) a IDbContextFactory (singleton).
	/// Dále registruje DbContextOptions&lt;TDbContext&gt; jako singleton.
	/// </summary>
	/// <remarks>
	/// DbContext a DbContextFactory&lt;TDbContext&gt; jsou z Entity Framework Core.
	/// IDbContext a IDbContextFactory (negenerická!) jsou z HFW.
	/// DbContextOptions&lt;TDbContext&gt; jsou v Entity Framework Core ve výchozím chování registrovány jako Scoped. To však brání možnosti
	/// použití jak DbContext, tak DbContextFactory&lt;TDbContext&gt;. Pro podporu obou potřebujeme DbContextOptions&lt;TDbContext&gt; registrovat jako singleton.
	/// </remarks>
	public EntityPatternsInstaller AddDbContext<TDbContext>(Action<IServiceProvider, DbContextOptionsBuilder> optionsAction = null)
		where TDbContext : Havit.Data.EntityFrameworkCore.DbContext, IDbContext
	{
		_services.AddDbContextFactory<TDbContext>();
		_services.AddDbContext<IDbContext, TDbContext>(GetDbContextOptionsBuilder(optionsAction), optionsLifetime: ServiceLifetime.Singleton);

		_services.TryAddTransient<IDbContextFactory, DbContextFactory<TDbContext>>();
		return this;
	}

	/// <summary>
	/// Registruje do DI containeru DbContext s DbContext poolingem vč. IDbContextFactory.
	/// </summary>
	public EntityPatternsInstaller AddDbContextPool<TDbContext>(Action<DbContextOptionsBuilder> optionsAction, int poolSize = DbContextPool<DbContext>.DefaultPoolSize)
		where TDbContext : Havit.Data.EntityFrameworkCore.DbContext, IDbContext
	{
		//Contract.Requires(componentRegistrationOptions.DbContextLifestyle == ServiceLifetime.Scoped);

		_services.AddPooledDbContextFactory<TDbContext>(GetDbContextOptionsBuilder(optionsAction), poolSize);
		_services.AddDbContextPool<IDbContext, TDbContext>(GetDbContextOptionsBuilder(optionsAction));

		_services.TryAddSingleton<IDbContextFactory, DbContextFactory<TDbContext>>();
		return this;
	}

	/// <summary>
	/// Registruje do DI containeru služby HFW pro Entity Framework Core.
	/// </summary>
	public EntityPatternsInstaller AddEntityPatterns()
	{
		_componentRegistrationOptions.CachingInstaller.Install(_services);

		_services.AddLogging();

		_services.TryAddSingleton<ISoftDeleteManager, SoftDeleteManager>();
		_services.TryAddSingleton<ICurrentCultureService, CurrentCultureService>();
		_services.TryAddTransient<IDataSeedRunner, DbDataSeedRunner>();
		_services.TryAddTransient<IDataSeedRunDecision, OncePerVersionDataSeedRunDecision>();
		_services.TryAddTransient<IDataSeedRunDecisionStatePersister, DbDataSeedRunDecisionStatePersister>();
		_services.TryAddTransient<IDataSeedPersister, DbDataSeedPersister>();

		_services.TryAddScoped<IDbDataSeedTransactionContext, DbDataSeedTransactionContext>();
		_services.TryAddTransient<IDataSeedPersisterFactory, DataSeedPersisterFactory>();

		_services.TryAddScoped(typeof(IUnitOfWork), _componentRegistrationOptions.UnitOfWorkType);
		_services.TryAddScoped<IDataLoader, DbDataLoaderWithLoadedPropertiesMemory>();
		_services.TryAddTransient<ILookupDataInvalidationRunner, LookupDataInvalidationRunner>();

		_services.TryAddSingleton<IPropertyLambdaExpressionManager, PropertyLambdaExpressionManager>();
		_services.TryAddSingleton<IPropertyLambdaExpressionBuilder, PropertyLambdaExpressionBuilder>();
		_services.TryAddSingleton<IPropertyLambdaExpressionStore, PropertyLambdaExpressionStore>();
		_services.TryAddSingleton<IPropertyLoadSequenceResolver, PropertyLoadSequenceResolverIncludingDeletedFilteringCollectionsSubstitution>();
		_services.TryAddTransient<IBeforeCommitProcessorsRunner, BeforeCommitProcessorsRunner>();

		_services.TryAddTransient<IBeforeCommitProcessorsFactory, BeforeCommitProcessorsFactory>();

		_services.TryAddSingleton<IBeforeCommitProcessor<object>, SetCreatedToInsertingEntitiesBeforeCommitProcessor>();
		_services.TryAddSingleton<IEntityValidationRunner, EntityValidationRunner>();

		_services.TryAddTransient<IEntityValidatorsFactory, EntityValidatorsFactory>();

		_services.TryAddTransient<IEntityKeyAccessor, DbEntityKeyAccessor>();
		_services.TryAddSingleton<IDbEntityKeyAccessorStorage, DbEntityKeyAccessorStorage>();
		_services.TryAddTransient<IReferencingNavigationsService, ReferencingNavigationsService>();
		_services.TryAddSingleton<IReferencingNavigationsStorage, ReferencingNavigationsStorage>();
		_services.TryAddTransient<INavigationTargetService, NavigationTargetService>();
		_services.TryAddSingleton<INavigationTargetStorage, NavigationTargetStorage>();
		_services.TryAddTransient<IEntityCacheKeyPrefixService, EntityCacheKeyPrefixService>();
		_services.TryAddSingleton<IEntityCacheKeyPrefixStorage, EntityCacheKeyPrefixStorage>();
		_services.TryAddTransient<IEntityCacheDependencyKeyGenerator, EntityCacheDependencyKeyGenerator>();
		_services.TryAddTransient<IEntityCacheDependencyManager, EntityCacheDependencyManager>();

		_services.TryAddSingleton<IRepositoryQueryProvider, RepositoryQueryProvider>();
		_services.TryAddSingleton<IRepositoryQueryStore, RepositoryQueryStore>();
		_services.TryAddSingleton<IRepositoryQueryBuilder, RepositoryCompiledQueryBuilder>();

		return this;
	}

	/// <summary>
	/// Registruje do DI containeru třídy z assembly předané v parametru dataLayerAssembly.
	/// Registrují se data seedy, data sources, repositories a data entries.
	/// </summary>
	public EntityPatternsInstaller AddDataLayer(Assembly dataLayerAssembly)
	{
		AddDataSeeds(dataLayerAssembly);

		Type[] dataLayerDependencyInjectionEnabledTypes = dataLayerAssembly.GetTypes().Where(type => type.IsClass && type.IsPublic).Where(IsNotAbstract).Where(DoesNotHaveFakeAttribute).ToArray();

		// Registrace přes IDataSource<T> nestačí, protože při pokusu získání instance dostaneme chybu
		// proto registrujeme přes IDataSource<KonkrétníTyp> pomocí metody WithServiceConstructedInterface.
		// Dále registrujeme přes potomky IDataSource<>, např. IKonkrétníTypDataSource.

		// DataSources
		Type[] dataSourceTypes = dataLayerDependencyInjectionEnabledTypes.Where(type => type.HasAncestorOfType(typeof(DbDataSource<>))).ToArray();
		foreach (Type dataSourceType in dataSourceTypes)
		{
			Type dataSourceConstructedInterface = dataSourceType.GetSingleConstructedType(typeof(IDataSource<>)); // získáme IDataSource<KonkrétníTyp>
			Type dataSourceInterface = dataSourceType.GetInterfaces().Where(dataSourceTypeInterfaceType => dataSourceTypeInterfaceType.ImplementsInterface(dataSourceConstructedInterface)).Single(); // získáme IKonkrétníTypDataSource

			_services.AddServices(new Type[] { dataSourceInterface, dataSourceConstructedInterface }, dataSourceType, ServiceLifetime.Transient);
		}

		// Repositories
		Type[] repositoryTypes = dataLayerDependencyInjectionEnabledTypes.Where(type => type.HasAncestorOfType(typeof(DbRepository<>))).ToArray();
		foreach (Type repositoryType in repositoryTypes)
		{
			Type repositoryConstructedInterface = repositoryType.GetSingleConstructedType(typeof(IRepository<>)); // získáme IRepository<KonkrétníTyp>
			Type repositoryInterface = repositoryType.GetInterfaces().Where(repositoryTypeInterfaceType => repositoryTypeInterfaceType.ImplementsInterface(repositoryConstructedInterface)).Single(); // získáme IKonkrétníTypDataSource

			_services.AddServices(new Type[] { repositoryInterface, repositoryConstructedInterface }, repositoryType, ServiceLifetime.Scoped);
		}

		// DataEntries
		Type[] dataEntryTypes = dataLayerDependencyInjectionEnabledTypes
			.Where(type => type.ImplementsInterface(typeof(IDataEntries)) // musí implementovat IDataEntries
				&& (type.BaseType != null)
				&& (type.BaseType.IsGenericType)
				&& (type.BaseType.GetGenericTypeDefinition() == typeof(DataEntries<>))) // a dědit z DataEntries (pro test konstruktorů, viz dále)
			.ToArray();

		foreach (Type dataEntryType in dataEntryTypes)
		{
			Type dataEntryInterface = dataEntryType.GetInterfaces().Where(dataEntryTypeTypeInterfaceType => dataEntryTypeTypeInterfaceType.ImplementsInterface(typeof(IDataEntries))).Single(); // získáme IKonkrétníTypEntries

			_services.AddScoped(dataEntryInterface, dataEntryType);

			// DataEntrySymbolService+Storage potřebujeme jen pro ty dataEntryTypes, které mají dva konstruktory.
			// Pokud má jeden konstruktor, je to IRepository.
			// Pokud má dva konstruktory, je to IDataEntrySymbolService a IRepository.

			if (dataEntryType.GetConstructors().Single().GetParameters().Count() == 2)
			{
				Type entityType = dataEntryType.BaseType.GetGenericArguments().Single();  // získáme KonkretníTyp

				_services.TryAddTransient(typeof(IDataEntrySymbolService<>).MakeGenericType(entityType), typeof(DataEntrySymbolService<>).MakeGenericType(entityType));
				_services.TryAddSingleton(typeof(IDataEntrySymbolStorage<>).MakeGenericType(entityType), typeof(DataEntrySymbolStorage<>).MakeGenericType(entityType));
			}
		}

		// Potřebujeme zaregistrovat IEntityKeyAccessor<TEntity, int>.
		// Nemáme seznam TEntity, tak je získáme z existujících implementací IRepository<>.
		// Pak pro každou TEntity zaregistrujeme DbEntityKeyAccesor<TEntity, int> pod IEntityKeyAccessor<TEntity, int>.
		dataLayerAssembly
			.GetExportedTypes()
			.SelectMany(type => type.GetInterfaces())
			.Where(interfaceType => interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IRepository<>))
			.Select(repositoryInterfaceType => repositoryInterfaceType.GetGenericArguments().Single()) // IRepository<TEntity> --> TEntity (tj. výsledkem je neunikátní seznam modelových tříd)
			.Distinct() // různé interface a třídy nám (DbRepository<Xy>, IXyRepository) nám dají stejné modelové třídy
			.ToList() // aby šel použít foreach
			.ForEach(modelType =>
			{
				Type interfaceType = typeof(IEntityKeyAccessor<,>).MakeGenericType(modelType, typeof(int)); // --> IEntityKeyAccessor<TEntity, int>
				Type implementationType = typeof(DbEntityKeyAccessor<,>).MakeGenericType(modelType, typeof(int)); // --> DbEntityKeyAccessor<TEntity, int>

				_services.TryAddTransient(interfaceType, implementationType);
			});

		return this;
	}

	public EntityPatternsInstaller AddLocalizationServices<TLanguage>()
		where TLanguage : class, ILanguage
	{
		Type currentLanguageServiceType = typeof(LanguageService<>).MakeGenericType(typeof(TLanguage));
		Type currentLanguageByCultureServiceType = typeof(LanguageByCultureService<>).MakeGenericType(typeof(TLanguage));

		_services.TryAddScoped(typeof(ILanguageService), currentLanguageServiceType);
		_services.TryAddTransient(typeof(ILanguageByCultureService), currentLanguageByCultureServiceType);
		_services.TryAddSingleton<ILanguageByCultureStorage, LanguageByCultureStorage>();
		_services.TryAddTransient<ILocalizationService, LocalizationService>();

		return this;
	}

	/// <summary>
	/// Registruje do DI containeru dataseeds z dané assembly.
	/// </summary>
	public EntityPatternsInstaller AddDataSeeds(Assembly dataSeedsAssembly)
	{
		Type[] dataSeedTypes = dataSeedsAssembly.GetTypes()
			.Where(type => type.IsClass && type.IsPublic)
			.Where(IsNotAbstract)
			.Where(DoesNotHaveFakeAttribute)
			.Where(type => type.ImplementsInterface(typeof(IDataSeed)))
			.ToArray();

		foreach (Type dataSeedType in dataSeedTypes)
		{
			_services.AddTransient(typeof(IDataSeed), dataSeedType); // nesmí být *TryAdd*, ale musí být Add, jinak se nám přidá jen první dataSeedType!
		}

		return this;
	}

	/// <summary>
	/// Zaregistruje do DI containeru lookup službu.
	/// Zajistí též takovou registraci, aby byla při uložení změn invalidována data v lookup services
	/// </summary>
	public EntityPatternsInstaller AddLookupService<TService, TImplementation>()
		where TService : class
		where TImplementation : class, TService, ILookupDataInvalidationService
	{
		return AddLookupService<TService, TImplementation, TImplementation>();
	}

	/// <summary>
	/// Zaregistruje do DI containeru lookup službu.
	/// Zajistí též takovou registraci, aby byla při uložení změn invalidována data v lookup services
	/// </summary>
	public EntityPatternsInstaller AddLookupService<TService, TImplementation, TLookupDataInvalidationService>()
		where TService : class
		where TImplementation : class, TService
		where TLookupDataInvalidationService : ILookupDataInvalidationService
	{
		_services.TryAddSingleton<IEntityLookupDataStorage, CacheEntityLookupDataStorage>();
		_services.AddServices(new Type[] { typeof(TService), typeof(ILookupDataInvalidationService) }, typeof(TLookupDataInvalidationService), ServiceLifetime.Transient);

		return this;
	}

	private static Action<DbContextOptionsBuilder> GetDbContextOptionsBuilder(Action<DbContextOptionsBuilder> customOptionsBuilder)
	{
		return (DbContextOptionsBuilder targetOptionsBuilder) =>
		{
			targetOptionsBuilder.UseFrameworkConventions();
			targetOptionsBuilder.UseDbLockedMigrator();
			customOptionsBuilder?.Invoke(targetOptionsBuilder);
		};
	}

	private static Action<IServiceProvider, DbContextOptionsBuilder> GetDbContextOptionsBuilder(Action<IServiceProvider, DbContextOptionsBuilder> customOptionsBuilder)
	{
		return (IServiceProvider serviceProvider, DbContextOptionsBuilder targetOptionsBuilder) =>
		{
			targetOptionsBuilder.UseFrameworkConventions();
			targetOptionsBuilder.UseDbLockedMigrator();
			customOptionsBuilder?.Invoke(serviceProvider, targetOptionsBuilder);
		};
	}

	/// <summary>
	/// Vrací true, pokud NEJDE o abstraktní typ.
	/// </summary>
	private static bool IsNotAbstract(Type type)
	{
		return !type.IsAbstract;
	}

	/// <summary>
	/// Vrací true, pokud typ NENÍ dekorován atributem FakeAttribute.
	/// </summary>
	private static bool DoesNotHaveFakeAttribute(Type type)
	{
		return !type.IsDefined(typeof(FakeAttribute), true);
	}

}
