using System.Reflection;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.DataSeeds;
using Havit.Data.EntityFrameworkCore.Patterns.DataSources;
using Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Infrastructure.Factories;
using Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.Lookups;
using Havit.Data.EntityFrameworkCore.Patterns.PropertyLambdaExpressions.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Repositories;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;
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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection;

/// <summary>
/// Extension metody pro IServiceCollection. Pro získání installeru Havit.Data.Entity.Patterns a souvisejících služeb.
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Registruje do DI containeru služby HFW pro Entity Framework Core.
	/// </summary>
	public static IServiceCollection AddDataLayerCoreServices(this IServiceCollection services, ComponentRegistrationOptions componentRegistrationOptions = null)
	{
		componentRegistrationOptions ??= new ComponentRegistrationOptions();

		componentRegistrationOptions.CachingInstaller.Install(services);

		services.AddLogging();

		services.TryAddSingleton<ISoftDeleteManager, SoftDeleteManager>();
		services.TryAddSingleton<ICurrentCultureService, CurrentCultureService>();
		services.TryAddTransient<IDataSeedRunner, DbDataSeedRunner>();
		services.TryAddTransient<IDataSeedRunDecision, OncePerVersionDataSeedRunDecision>();
		services.TryAddTransient<IDataSeedRunDecisionStatePersister, DbDataSeedRunDecisionStatePersister>();
		services.TryAddTransient<IDataSeedPersister, DbDataSeedPersister>();

		services.TryAddTransient<IDataSeedPersisterFactory, DataSeedPersisterFactory>();

		services.TryAddScoped<IUnitOfWork, DbUnitOfWork>();

		services.TryAddScoped<IDataLoader, DbDataLoader>();
		services.TryAddScoped<ILoadedPropertyReader, DbLoadedPropertyReaderWithMemory>();

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

		services.TryAddSingleton<IEntityKeyAccessor, DbEntityKeyAccessor>();
		services.TryAddTransient<IDbEntityKeyAccessorStorageBuilder, DbEntityKeyAccessorStorageBuilder>();
		services.TryAddSingletonFromScopedServiceProvider<IDbEntityKeyAccessorStorage>(sp => sp.GetRequiredService<IDbEntityKeyAccessorStorageBuilder>().Build());
		services.TryAddSingleton<IDbEntityKeyAccessorStorage, DbEntityKeyAccessorStorage>();
		services.TryAddSingleton<IReferencingNavigationsService, ReferencingNavigationsService>();
		services.TryAddTransient<IReferencingNavigationsStorageBuilder, ReferencingNavigationsStorageBuilder>();
		services.TryAddSingletonFromScopedServiceProvider<IReferencingNavigationsStorage>(sp => sp.GetRequiredService<IReferencingNavigationsStorageBuilder>().Build());
		services.TryAddSingleton<INavigationTargetService, NavigationTargetService>();
		services.TryAddTransient<INavigationTargetStorageBuilder, NavigationTargetStorageBuilder>();
		services.TryAddSingletonFromScopedServiceProvider<INavigationTargetStorage>(sp => sp.GetRequiredService<INavigationTargetStorageBuilder>().Build());
		services.TryAddSingleton<IEntityCacheKeyPrefixService, EntityCacheKeyPrefixService>();
		services.TryAddTransient<IEntityCacheKeyPrefixStorageBuilder, EntityCacheKeyPrefixStorageBuilder>();
		services.TryAddSingletonFromScopedServiceProvider<IEntityCacheKeyPrefixStorage>(sp => sp.GetRequiredService<IEntityCacheKeyPrefixStorageBuilder>().Build());
		services.TryAddTransient<IEntityCacheDependencyKeyGenerator, EntityCacheDependencyKeyGenerator>();
		services.TryAddTransient<IEntityCacheDependencyManager, EntityCacheDependencyManager>();

		services.TryAddSingleton<IRepositoryQueryProvider, RepositoryQueryProvider>();
		services.TryAddSingleton<IRepositoryQueryStore, RepositoryQueryStore>();
		services.TryAddSingleton<IRepositoryQueryBuilder, RepositoryCompiledQueryBuilder>();

		return services;
	}

	/// <summary>
	/// Vyhazuje výjimku NotSupportedException.
	/// </summary>
	[Obsolete("Use generated UseDataLayerServices method.")]
	public static IServiceCollection AddDataLayer(this IServiceCollection services, Assembly dataLayerAssembly)
	{
		throw new NotSupportedException();
	}

	/// <summary>
	/// Registruje do DI containeru služby pro lokalizaci.
	/// </summary>
	public static IServiceCollection AddLocalizationServices<TLanguage>(this IServiceCollection services)
		where TLanguage : class, ILanguage
	{
		Type currentLanguageServiceType = typeof(LanguageService<>).MakeGenericType(typeof(TLanguage));
		Type currentLanguageByCultureServiceType = typeof(LanguageByCultureService<>).MakeGenericType(typeof(TLanguage));

		services.TryAddScoped(typeof(ILanguageService), currentLanguageServiceType);
		services.TryAddTransient(typeof(ILanguageByCultureService), currentLanguageByCultureServiceType);
		services.TryAddSingleton<ILanguageByCultureStorage, LanguageByCultureStorage>();
		services.TryAddTransient<ILocalizationService, LocalizationService>();

		return services;
	}

	/// <summary>
	/// Registruje do DI containeru dataseeds z dané assembly.
	/// </summary>
	public static IServiceCollection AddDataSeeds(this IServiceCollection services, Assembly dataSeedsAssembly)
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

		return services;
	}

	/// <summary>
	/// Zaregistruje do DI containeru lookup službu.
	/// Zajistí též takovou registraci, aby byla při uložení změn invalidována data v lookup services
	/// </summary>
	public static IServiceCollection AddLookupService<TService, TImplementation>(this IServiceCollection services)
		where TService : class
		where TImplementation : class, TService, ILookupDataInvalidationService
	{
		return services.AddLookupService<TService, TImplementation, TImplementation>();
	}

	/// <summary>
	/// Zaregistruje do DI containeru lookup službu.
	/// Zajistí též takovou registraci, aby byla při uložení změn invalidována data v lookup services
	/// </summary>
	public static IServiceCollection AddLookupService<TService, TImplementation, TLookupDataInvalidationService>(this IServiceCollection services)
		where TService : class
		where TImplementation : class, TService
		where TLookupDataInvalidationService : ILookupDataInvalidationService
	{
		services.TryAddSingleton<IEntityLookupDataStorage, CacheEntityLookupDataStorage>();
		services.AddServices(new Type[] { typeof(TService), typeof(ILookupDataInvalidationService) }, typeof(TLookupDataInvalidationService), ServiceLifetime.Transient);

		return services;
	}

	internal static IServiceCollection AddServices(this IServiceCollection services, Type[] serviceTypes, Type implementationType, ServiceLifetime lifetime)
	{
		if (serviceTypes.Length == 0)
		{
			throw new ArgumentException("ServiceTypes required.", nameof(serviceTypes));
		}

		if (serviceTypes.Length == 1)
		{
			services.Add(new ServiceDescriptor(serviceTypes.Single(), implementationType, lifetime));
		}
		else
		{
			services.AddMultipleServices(serviceTypes, implementationType, lifetime);
		}

		return services;
	}

	internal static void AddMultipleServices(this IServiceCollection services, Type[] serviceTypes, Type implementationType, ServiceLifetime lifetime)
	{
		if (lifetime == ServiceLifetime.Transient)
		{
			foreach (var serviceType in serviceTypes)
			{
				services.Add(new ServiceDescriptor(serviceType, implementationType, ServiceLifetime.Transient));
			}
			return;
		}

		// je Scoped nebo Singleton a zároveň máme více interfaces
		var firstServiceTypeToRegister = serviceTypes.First();

		// registrace prvního interface
		services.Add(new ServiceDescriptor(firstServiceTypeToRegister, implementationType, lifetime /* Scoped nebo Singleton */));

		// registrace druhého a dalšího interface
		foreach (var serviceType in serviceTypes.Skip(1) /* až od druhého */)
		{
			services.AddTransient(serviceType, sp => sp.GetRequiredService(firstServiceTypeToRegister));
		}
	}

	internal static IServiceCollection TryAddSingletonFromScopedServiceProvider<TService>(this IServiceCollection services, Func<IServiceProvider, TService> factory)
		where TService : class
	{
		services.TryAddSingleton<TService>(sp =>
		{
			using var scope = sp.CreateScope();
			return factory(scope.ServiceProvider);
		});

		return services;
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
