using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;
using Havit.Data.EntityFrameworkCore.Patterns.DataSeeds;
using Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors;
using Havit.Data.EntityFrameworkCore.TestHelpers.DependencyInjection.Infrastructure.DataLayer;
using Havit.Data.EntityFrameworkCore.TestHelpers.DependencyInjection.Infrastructure.Entity;
using Havit.Data.EntityFrameworkCore.TestHelpers.DependencyInjection.Infrastructure.Model;
using Havit.Data.Patterns.DataLoaders;
using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.DataSources;
using Havit.Data.Patterns.Localizations;
using Havit.Data.Patterns.Repositories;
using Havit.Data.Patterns.UnitOfWorks;
using Havit.Services.Caching;
using Havit.Services.TimeServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DependencyInjection;

[TestClass]
public class ServiceCollectionExtensionsTests
{
	[TestMethod]
	public void ServiceCollectionExtensions_DbContext_ConventionsUsesDbLockedMigrator()
	{
		// Arrange
		// noop

		// Act
		IServiceProvider serviceProvider = CreateAndSetupServiceProvider(pooling: false);

		// Assert
		using var scope = serviceProvider.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<IDbContext>();
		var migrator = ((IInfrastructure<IServiceProvider>)dbContext).GetService<IMigrator>();
		Assert.IsInstanceOfType(migrator, typeof(Havit.Data.EntityFrameworkCore.Migrations.Internal.DbLockedMigrator));
	}

	[TestMethod]
	public void ServiceCollectionExtensions_PooledDbContext_ConventionsUsesDbLockedMigrator()
	{
		// Arrange
		// noop

		// Act
		IServiceProvider serviceProvider = CreateAndSetupServiceProvider(pooling: true);

		// Assert
		using var scope = serviceProvider.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<IDbContext>();
		var migrator = ((IInfrastructure<IServiceProvider>)dbContext).GetService<IMigrator>();
		Assert.IsInstanceOfType(migrator, typeof(Havit.Data.EntityFrameworkCore.Migrations.Internal.DbLockedMigrator));
	}

	[TestMethod]
	public void ServiceCollectionExtensions_LanguageAndLocalizationServicesAreScoped()
	{
		// Arrange
		IServiceProvider serviceProvider = CreateAndSetupServiceProvider();

		// Act
		using var scope1 = serviceProvider.CreateScope();
		using var scope2 = serviceProvider.CreateScope();

		var languageService1 = scope1.ServiceProvider.GetRequiredService<ILanguageService>();
		var localizationService1 = scope1.ServiceProvider.GetRequiredService<ILocalizationService>();

		var languageService2 = scope2.ServiceProvider.GetRequiredService<ILanguageService>();
		var localizationService2 = scope2.ServiceProvider.GetRequiredService<ILocalizationService>();

		// Assert			
		Assert.AreNotSame(languageService1, languageService2);
		Assert.AreNotSame(localizationService1, localizationService2);

		Assert.IsInstanceOfType(languageService1, typeof(LanguageService<Language>));
		Assert.IsInstanceOfType(languageService2, typeof(LanguageService<Language>));

	}

	[TestMethod]
	public void ServiceCollectionExtensions_DataSourcesAreTransient()
	{
		// Arrange
		IServiceProvider serviceProvider = CreateAndSetupServiceProvider();

		// Act
		using var scope1 = serviceProvider.CreateScope();
		using var scope2 = serviceProvider.CreateScope();

		var languageDataSource1 = scope1.ServiceProvider.GetRequiredService<ILanguageDataSource>();
		var languageDataSource2 = scope2.ServiceProvider.GetRequiredService<IDataSource<Language>>();

		// Assert			
		Assert.AreNotSame(languageDataSource1, languageDataSource2);

		Assert.IsInstanceOfType(languageDataSource1, typeof(LanguageDataSource));
		Assert.IsInstanceOfType(languageDataSource2, typeof(LanguageDataSource));
	}

	[TestMethod]
	public void ServiceCollectionExtensions_RepositoriesAreScoped()
	{
		// Arrange
		IServiceProvider serviceProvider = CreateAndSetupServiceProvider();

		// Act
		using var scope1 = serviceProvider.CreateScope();
		using var scope2 = serviceProvider.CreateScope();

		var languageRepository1a = scope1.ServiceProvider.GetRequiredService<ILanguageRepository>();
		var languageRepository1b = scope1.ServiceProvider.GetRequiredService<IRepository<Language>>();
		var languageRepository2a = scope2.ServiceProvider.GetRequiredService<ILanguageRepository>();
		var languageRepository2b = scope2.ServiceProvider.GetRequiredService<IRepository<Language>>();

		// Assert			
		Assert.AreSame(languageRepository1a, languageRepository1b);
		Assert.AreSame(languageRepository2a, languageRepository2b);
		Assert.AreNotSame(languageRepository1a, languageRepository2a);

		Assert.IsInstanceOfType(languageRepository1a, typeof(LanguageRepository));
		Assert.IsInstanceOfType(languageRepository2a, typeof(LanguageRepository));
	}

	[TestMethod]
	public void ServiceCollectionExtensions_DataLoaderIsScoped()
	{
		// Arrange
		IServiceProvider serviceProvider = CreateAndSetupServiceProvider();

		// Act
		using var scope1 = serviceProvider.CreateScope();
		using var scope2 = serviceProvider.CreateScope();

		var dataLoader1 = scope1.ServiceProvider.GetRequiredService<IDataLoader>();
		var dataLoader2 = scope2.ServiceProvider.GetRequiredService<IDataLoader>();

		// Assert
		Assert.AreNotSame(dataLoader1, dataLoader2);

		Assert.IsInstanceOfType(dataLoader1, typeof(DbDataLoader));
		Assert.IsInstanceOfType(dataLoader2, typeof(DbDataLoader));
	}

	[TestMethod]
	public void ServiceCollectionExtensions_UnitOfWorkIsScoped()
	{
		// Arrange
		IServiceProvider serviceProvider = CreateAndSetupServiceProvider();

		// Act
		using var scope1 = serviceProvider.CreateScope();
		using var scope2 = serviceProvider.CreateScope();

		var unitOfWork1 = scope1.ServiceProvider.GetRequiredService<IUnitOfWork>();
		var unitOfWork2 = scope2.ServiceProvider.GetRequiredService<IUnitOfWork>();

		// Assert
		Assert.AreNotSame(unitOfWork1, unitOfWork2);

		Assert.IsInstanceOfType(unitOfWork1, typeof(DbUnitOfWork));
		Assert.IsInstanceOfType(unitOfWork2, typeof(DbUnitOfWork));
	}

	[TestMethod]
	public void ServiceCollectionExtensions_EntityCacheManagerIsTransient()
	{
		// Arrange
		IServiceProvider serviceProvider = CreateAndSetupServiceProvider();

		// Act

		using var scope = serviceProvider.CreateScope();

		var entityCacheManager1 = scope.ServiceProvider.GetRequiredService<IEntityCacheManager>();
		var entityCacheManager2 = scope.ServiceProvider.GetRequiredService<IEntityCacheManager>();

		// Assert
		Assert.AreNotSame(entityCacheManager1, entityCacheManager2);

		Assert.IsInstanceOfType(entityCacheManager1, typeof(EntityCacheManager));
		Assert.IsInstanceOfType(entityCacheManager2, typeof(EntityCacheManager));
	}

	[TestMethod]
	public void ServiceCollectionExtensions_BeforeCommitProcessorsRunnerIsRegistered()
	{
		// Arrange
		IServiceProvider serviceProvider = CreateAndSetupServiceProvider();

		// Act
		var beforeCommitProcessorsRunner = serviceProvider.GetRequiredService<IBeforeCommitProcessorsRunner>();

		// Assert
		Assert.IsInstanceOfType(beforeCommitProcessorsRunner, typeof(BeforeCommitProcessorsRunner));
	}

	[TestMethod]
	public void ServiceCollectionExtensions_BeforeCommitProcessorsFactoryIsRegistered()
	{
		// Arrange
		IServiceProvider serviceProvider = CreateAndSetupServiceProvider();

		// Act
		var beforeCommitProcessorsFactory = serviceProvider.GetRequiredService<IBeforeCommitProcessorsFactory>();

		// Assert
		Assert.IsInstanceOfType(beforeCommitProcessorsFactory, typeof(IBeforeCommitProcessorsFactory));
	}

	[TestMethod]
	public void ServiceCollectionExtensions_SetCreatedToInsertingEntitiesBeforeCommitProcessorIsRegistered()
	{
		// Arrange
		IServiceProvider serviceProvider = CreateAndSetupServiceProvider();

		// Act
		IBeforeCommitProcessorsFactory factory = serviceProvider.GetRequiredService<IBeforeCommitProcessorsFactory>();
		serviceProvider.GetRequiredService<IBeforeCommitProcessorsRunner>();
		var languageBeforeCommitProcessors = factory.Create(typeof(Language)).ToList();
		var objectBeforeCommitProcessors = factory.Create(typeof(object)).ToList();

		// Assert
		Assert.IsInstanceOfType<SetCreatedToInsertingEntitiesBeforeCommitProcessor>(languageBeforeCommitProcessors.Single());
		Assert.IsInstanceOfType<SetCreatedToInsertingEntitiesBeforeCommitProcessor>(objectBeforeCommitProcessors.Single());

		Assert.IsInstanceOfType(objectBeforeCommitProcessors.Single(), typeof(SetCreatedToInsertingEntitiesBeforeCommitProcessor));
	}

	[TestMethod]
	public void ServiceCollectionExtensions_DataSeedRunnerIsRegistered()
	{
		// Arrange
		IServiceProvider serviceProvider = CreateAndSetupServiceProvider();

		// Act
		using var scope = serviceProvider.CreateScope();

		var dataSeedRunner = scope.ServiceProvider.GetRequiredService<IDataSeedRunner>();

		// Assert
		Assert.IsInstanceOfType(dataSeedRunner, typeof(DbDataSeedRunner));
	}


	[TestMethod]
	public void ServiceCollectionExtensions_UnitOfWorks_CanBeResolvedWithoutCacheSupport()
	{
		// Arrange
		IServiceProvider serviceProvider = CreateAndSetupServiceProvider(withNoCaching: true);

		// Act
		using var scope = serviceProvider.CreateScope();

		var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
		var entityCacheManager = scope.ServiceProvider.GetRequiredService<IEntityCacheManager>();

		// Assert
		Assert.IsInstanceOfType(unitOfWork, typeof(DbUnitOfWork));
		Assert.IsInstanceOfType(entityCacheManager, typeof(NoCachingEntityCacheManager));
	}

	internal static IServiceProvider CreateAndSetupServiceProvider(bool pooling = false, bool withNoCaching = false)
	{
		// V Development dochází k více kontrolám
		var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings { EnvironmentName = "Development" });

		if (pooling)
		{
			builder.Services.AddDbContextPool<IDbContext, TestDbContext>(options => options
				.UseSqlServer("Data Source=FAKE")
				.UseDefaultHavitConventions());
		}
		else
		{
			builder.Services.AddDbContext<IDbContext, TestDbContext>(options => options
				.UseSqlServer("Data Source=FAKE")
				.UseDefaultHavitConventions());
		}

		ComponentRegistrationOptions componentRegistrationOptions = new ComponentRegistrationOptions();
		if (withNoCaching)
		{
			componentRegistrationOptions.ConfigureNoCaching();
		}
		else
		{
			builder.Services.AddSingleton<ICacheService, NullCacheService>();
		}

		builder.Services
			.AddDataLayerServices(componentRegistrationOptions)
			.AddLocalizationServices<Language>();
		builder.Services.AddSingleton<ITimeService, ServerTimeService>();

		var application = builder.Build();
		return application.Services;
	}
}
