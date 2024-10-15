﻿using System.Reflection;
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
/// Implementace <see cref="IEntityPatternsInstaller"/>u.
/// </summary>
internal class EntityPatternsInstaller : IEntityPatternsInstaller
{
	private readonly IServiceCollection services;
	private readonly ComponentRegistrationOptions componentRegistrationOptions;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public EntityPatternsInstaller(IServiceCollection services, ComponentRegistrationOptions componentRegistrationOptions)
	{
		this.services = services;
		this.componentRegistrationOptions = componentRegistrationOptions;
	}

	/// <summary>
	/// Viz <see cref="IEntityPatternsInstaller"/>
	/// </summary>
	public IEntityPatternsInstaller AddLocalizationServices<TLanguage>()
		where TLanguage : class, ILanguage
	{
		Type currentLanguageServiceType = typeof(LanguageService<>).MakeGenericType(typeof(TLanguage));
		Type currentLanguageByCultureServiceType = typeof(LanguageByCultureService<>).MakeGenericType(typeof(TLanguage));

		services.TryAddScoped(typeof(ILanguageService), currentLanguageServiceType);
		services.TryAddTransient(typeof(ILanguageByCultureService), currentLanguageByCultureServiceType);
		services.TryAddSingleton<ILanguageByCultureStorage, LanguageByCultureStorage>();
		services.TryAddTransient<ILocalizationService, LocalizationService>();

		return this;
	}

	/// <summary>
	/// Viz <see cref="IEntityPatternsInstaller"/>
	/// </summary>
	public IEntityPatternsInstaller AddEntityPatterns()
	{
		componentRegistrationOptions.CachingInstaller.Install(services);

		services.AddLogging();

		services.TryAddSingleton<ISoftDeleteManager, SoftDeleteManager>();
		services.TryAddSingleton<ICurrentCultureService, CurrentCultureService>();
		services.TryAddTransient<IDataSeedRunner, DbDataSeedRunner>();
		services.TryAddTransient<IDataSeedRunDecision, OncePerVersionDataSeedRunDecision>();
		services.TryAddTransient<IDataSeedRunDecisionStatePersister, DbDataSeedRunDecisionStatePersister>();
		services.TryAddTransient<IDataSeedPersister, DbDataSeedPersister>();

		services.TryAddScoped<IDbDataSeedTransactionContext, DbDataSeedTransactionContext>();
		services.TryAddTransient<IDataSeedPersisterFactory, DataSeedPersisterFactory>();

		services.TryAddScoped(typeof(IUnitOfWork), componentRegistrationOptions.UnitOfWorkType);
		services.TryAddScoped<IDataLoader, DbDataLoaderWithLoadedPropertiesMemory>();
		services.TryAddTransient<ILookupDataInvalidationRunner, LookupDataInvalidationRunner>();

		services.TryAddSingleton<IPropertyLambdaExpressionManager, PropertyLambdaExpressionManager>();
		services.TryAddSingleton<IPropertyLambdaExpressionBuilder, PropertyLambdaExpressionBuilder>();
		services.TryAddSingleton<IPropertyLambdaExpressionStore, PropertyLambdaExpressionStore>();
		services.TryAddSingleton<IPropertyLoadSequenceResolver, PropertyLoadSequenceResolverIncludingDeletedFilteringCollectionsSubstitution>();
		services.TryAddTransient<IBeforeCommitProcessorsRunner, BeforeCommitProcessorsRunner>();

		services.TryAddTransient<IBeforeCommitProcessorsFactory, BeforeCommitProcessorsFactory>();

		services.TryAddSingleton<IBeforeCommitProcessor<object>, SetCreatedToInsertingEntitiesBeforeCommitProcessor>();
		services.TryAddSingleton<IEntityValidationRunner, EntityValidationRunner>();

		services.TryAddTransient<IEntityValidatorsFactory, EntityValidatorsFactory>();

		services.TryAddTransient<IEntityKeyAccessor, DbEntityKeyAccessor>();
		services.TryAddSingleton<IDbEntityKeyAccessorStorage, DbEntityKeyAccessorStorage>();
		services.TryAddTransient<IReferencingNavigationsService, ReferencingNavigationsService>();
		services.TryAddSingleton<IReferencingNavigationsStorage, ReferencingNavigationsStorage>();
		services.TryAddTransient<INavigationTargetService, NavigationTargetService>();
		services.TryAddSingleton<INavigationTargetStorage, NavigationTargetStorage>();
		services.TryAddTransient<IEntityCacheKeyPrefixService, EntityCacheKeyPrefixService>();
		services.TryAddSingleton<IEntityCacheKeyPrefixStorage, EntityCacheKeyPrefixStorage>();
		services.TryAddTransient<IEntityCacheDependencyKeyGenerator, EntityCacheDependencyKeyGenerator>();
		services.TryAddTransient<IEntityCacheDependencyManager, EntityCacheDependencyManager>();

		services.TryAddSingleton<IRepositoryQueryProvider, RepositoryQueryProvider>();
		services.TryAddSingleton<IRepositoryQueryStore, RepositoryQueryStore>();
		services.TryAddSingleton<IRepositoryQueryBuilder, RepositoryCompiledQueryBuilder>();

		return this;
	}

	/// <summary>
	/// Viz <see cref="IEntityPatternsInstaller"/>
	/// </summary>
	public IEntityPatternsInstaller AddDataSeeds(Assembly dataSeedsAssembly)
	{
		Type[] dataSeedTypes = dataSeedsAssembly.GetTypes()
			.Where(type => type.IsClass && type.IsPublic)
			.Where(IsNotAbstract)
			.Where(DoesNotHaveFakeAttribute)
			.Where(type => type.ImplementsInterface(typeof(IDataSeed)))
			.ToArray();

		foreach (Type dataSeedType in dataSeedTypes)
		{
			services.AddTransient(typeof(IDataSeed), dataSeedType); // nesmí být *TryAdd*, ale musí být Add, jinak se nám přidá jen první dataSeedType!
		}

		return this;
	}

	/// <summary>
	/// Viz <see cref="IEntityPatternsInstaller"/>
	/// </summary>
	public IEntityPatternsInstaller AddDataLayer(Assembly dataLayerAssembly)
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

			services.AddServices(new Type[] { dataSourceInterface, dataSourceConstructedInterface }, dataSourceType, ServiceLifetime.Transient);
		}

		// Repositories
		Type[] repositoryTypes = dataLayerDependencyInjectionEnabledTypes.Where(type => type.HasAncestorOfType(typeof(DbRepository<>))).ToArray();
		foreach (Type repositoryType in repositoryTypes)
		{
			Type repositoryConstructedInterface = repositoryType.GetSingleConstructedType(typeof(IRepository<>)); // získáme IRepository<KonkrétníTyp>
			Type repositoryInterface = repositoryType.GetInterfaces().Where(repositoryTypeInterfaceType => repositoryTypeInterfaceType.ImplementsInterface(repositoryConstructedInterface)).Single(); // získáme IKonkrétníTypDataSource

			services.AddServices(new Type[] { repositoryInterface, repositoryConstructedInterface }, repositoryType, ServiceLifetime.Scoped);
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

			services.AddScoped(dataEntryInterface, dataEntryType);

			// DataEntrySymbolService+Storage potřebujeme jen pro ty dataEntryTypes, které mají dva konstruktory.
			// Pokud má jeden konstruktor, je to IRepository.
			// Pokud má dva konstruktory, je to IDataEntrySymbolService a IRepository.

			if (dataEntryType.GetConstructors().Single().GetParameters().Count() == 2)
			{
				Type entityType = dataEntryType.BaseType.GetGenericArguments().Single();  // získáme KonkretníTyp

				services.TryAddTransient(typeof(IDataEntrySymbolService<>).MakeGenericType(entityType), typeof(DataEntrySymbolService<>).MakeGenericType(entityType));
				services.TryAddSingleton(typeof(IDataEntrySymbolStorage<>).MakeGenericType(entityType), typeof(DataEntrySymbolStorage<>).MakeGenericType(entityType));
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

				services.TryAddTransient(interfaceType, implementationType);
			});

		return this;
	}

	/// <summary>
	/// Viz <see cref="IEntityPatternsInstaller"/>
	/// </summary>
	public IEntityPatternsInstaller AddLookupService<TService, TImplementation>()
		where TService : class
		where TImplementation : class, TService, ILookupDataInvalidationService
	{
		return AddLookupService<TService, TImplementation, TImplementation>();
	}

	/// <summary>
	/// Viz <see cref="IEntityPatternsInstaller"/>
	/// </summary>
	public IEntityPatternsInstaller AddLookupService<TService, TImplementation, TLookupDataInvalidationService>()
		where TService : class
		where TImplementation : class, TService
		where TLookupDataInvalidationService : ILookupDataInvalidationService
	{
		services.TryAddSingleton<IEntityLookupDataStorage, CacheEntityLookupDataStorage>();
		services.AddServices(new Type[] { typeof(TService), typeof(ILookupDataInvalidationService) }, typeof(TLookupDataInvalidationService), ServiceLifetime.Transient);

		return this;
	}

	/// <summary>
	/// Viz <see cref="IEntityPatternsInstaller"/>
	/// </summary>
	public IEntityPatternsInstaller AddDbContext<TDbContext>()
		where TDbContext : Havit.Data.EntityFrameworkCore.DbContext, IDbContext
	{
		return AddDbContext<TDbContext>((Action<DbContextOptionsBuilder>)null);
	}

	/// <summary>
	/// Viz <see cref="IEntityPatternsInstaller"/>
	/// </summary>
	public IEntityPatternsInstaller AddDbContext<TDbContext>(Action<DbContextOptionsBuilder> optionsAction = null)
		where TDbContext : Havit.Data.EntityFrameworkCore.DbContext, IDbContext
	{
		services.AddDbContextFactory<TDbContext>();
		services.AddDbContext<IDbContext, TDbContext>(GetDbContextOptionsBuilder(optionsAction), optionsLifetime: ServiceLifetime.Singleton);

		services.TryAddTransient<IDbContextFactory, DbContextFactory<TDbContext>>();
		return this;
	}

	/// <summary>
	/// Viz <see cref="IEntityPatternsInstaller"/>
	/// </summary>
	public IEntityPatternsInstaller AddDbContext<TDbContext>(Action<IServiceProvider, DbContextOptionsBuilder> optionsAction = null)
		where TDbContext : Havit.Data.EntityFrameworkCore.DbContext, IDbContext
	{
		// na pořadí záleží
		services.AddDbContextFactory<TDbContext>();
		services.AddDbContext<IDbContext, TDbContext>(GetDbContextOptionsBuilder(optionsAction), optionsLifetime: ServiceLifetime.Singleton);

		services.TryAddTransient<IDbContextFactory, DbContextFactory<TDbContext>>();
		return this;
	}

	// TODO: Odstranit
	/// <summary>
	/// Viz <see cref="IEntityPatternsInstaller"/>
	/// </summary>
	public IEntityPatternsInstaller AddDbContextPool<TDbContext>(Action<DbContextOptionsBuilder> optionsAction, int poolSize = DbContextPool<DbContext>.DefaultPoolSize)
		where TDbContext : Havit.Data.EntityFrameworkCore.DbContext, IDbContext
	{
		//Contract.Requires(componentRegistrationOptions.DbContextLifestyle == ServiceLifetime.Scoped);

		services.AddPooledDbContextFactory<TDbContext>(GetDbContextOptionsBuilder(optionsAction), poolSize);
		services.AddDbContextPool<IDbContext, TDbContext>(GetDbContextOptionsBuilder(optionsAction), );

		services.TryAddSingleton<IDbContextFactory, DbContextFactory<TDbContext>>();
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
