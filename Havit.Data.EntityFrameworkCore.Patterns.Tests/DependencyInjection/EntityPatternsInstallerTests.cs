using Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.DataSeeds;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors;
using Havit.Data.Patterns.DataLoaders;
using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.DataSources;
using Havit.Data.Patterns.Localizations;
using Havit.Data.Patterns.Repositories;
using Havit.Data.Patterns.UnitOfWorks;
using Havit.Services;
using Havit.Services.TimeServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Text;
using System;
using Microsoft.Extensions.DependencyInjection;
using Havit.Data.EntityFrameworkCore.TestHelpers.DependencyInjection.Infrastructure.DataLayer;
using Havit.Services.Caching;
using Havit.Data.EntityFrameworkCore.TestHelpers.DependencyInjection.Infrastructure.Model;
using Havit.Data.EntityFrameworkCore.TestHelpers.DependencyInjection.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DependencyInjection;

[TestClass]
public class EntityPatternsInstallerTests
{
	[TestMethod]
	public void EntityPatternsInstaller_DbContextIsScoped()
	{
		// Arrange
		IServiceProvider serviceProvider = CreateAndSetupServiceProvider();

		// Act
		using var scope1 = serviceProvider.CreateScope();
		using var scope2 = serviceProvider.CreateScope();

		var dbContext1a = scope1.ServiceProvider.GetRequiredService<IDbContext>();
		var dbContext1b = scope1.ServiceProvider.GetRequiredService<IDbContext>();
		var dbContext2 = scope2.ServiceProvider.GetRequiredService<IDbContext>();

		// Assert			
		Assert.AreSame(dbContext1a, dbContext1b);
		Assert.AreNotSame(dbContext1a, dbContext2);
		Assert.AreNotSame(dbContext1b, dbContext2);

		Assert.IsInstanceOfType(dbContext1a, typeof(TestDbContext));
		Assert.IsInstanceOfType(dbContext2, typeof(TestDbContext));
	}

	[TestMethod]
	public void EntityPatternsInstaller_PooledDbContextIsScoped()
	{
		// Arrange
		IServiceProvider serviceProvider = CreateAndSetupServiceProvider(pooling: true);

		// Act

		// scopes jsou paralelně existující
		using var scope1 = serviceProvider.CreateScope();
		using var scope2 = serviceProvider.CreateScope();

		IDbContext dbContext1 = scope1.ServiceProvider.GetRequiredService<IDbContext>();
		IDbContext dbContext2 = scope2.ServiceProvider.GetRequiredService<IDbContext>();

		// Assert			
		Assert.AreNotSame(dbContext1, dbContext2);

		Assert.IsInstanceOfType(dbContext1, typeof(TestDbContext));
		Assert.IsInstanceOfType(dbContext2, typeof(TestDbContext));
	}

	[TestMethod]
	public void EntityPatternsInstaller_PooledDbContextIsReused()
	{
		// Arrange
		IServiceProvider serviceProvider = CreateAndSetupServiceProvider(pooling: true);

		// Act
		IDbContext dbContext1;
		IDbContext dbContext2;

		// scopes jsou za sebou jdoucí
		using (var scope = serviceProvider.CreateScope())
		{
			dbContext1 = scope.ServiceProvider.GetRequiredService<IDbContext>();
		}

		using (var scope = serviceProvider.CreateScope())
		{
			dbContext2 = scope.ServiceProvider.GetRequiredService<IDbContext>();
		}

		// Assert			
		Assert.AreSame(dbContext1, dbContext2);
		Assert.IsInstanceOfType(dbContext1, typeof(TestDbContext));
	}

	[TestMethod]
	public void EntityPatternsInstaller_LanguageAndLocalizationServicesAreScoped()
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
	public void EntityPatternsInstaller_DataSourcesAreTransient()
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
	public void EntityPatternsInstaller_RepositoriesAreScoped()
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
	public void EntityPatternsInstaller_DataLoaderIsScoped()
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
	public void EntityPatternsInstaller_UnitOfWorkIsScoped()
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
	public void EntityPatternsInstaller_EntityCacheManagerIsTransient()
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
	public void EntityPatternsInstaller_BeforeCommitProcessorsRunnerIsRegistered()
	{
		// Arrange
		IServiceProvider serviceProvider = CreateAndSetupServiceProvider();

		// Act
		var beforeCommitProcessorsRunner = serviceProvider.GetRequiredService<IBeforeCommitProcessorsRunner>();

		// Assert
		Assert.IsInstanceOfType(beforeCommitProcessorsRunner, typeof(BeforeCommitProcessorsRunner));
	}

	[TestMethod]
	public void EntityPatternsInstaller_BeforeCommitProcessorsFactoryIsRegistered()
	{
		// Arrange
		IServiceProvider serviceProvider = CreateAndSetupServiceProvider();

		// Act
		var beforeCommitProcessorsFactory = serviceProvider.GetRequiredService<IBeforeCommitProcessorsFactory>();

		// Assert
		Assert.IsInstanceOfType(beforeCommitProcessorsFactory, typeof(IBeforeCommitProcessorsFactory));
	}

	[TestMethod]
	public void EntityPatternsInstaller_SetCreatedToInsertingEntitiesBeforeCommitProcessorIsRegistered()
	{
		// Arrange
		IServiceProvider serviceProvider = CreateAndSetupServiceProvider();

		// Act
		IBeforeCommitProcessorsFactory factory = serviceProvider.GetRequiredService<IBeforeCommitProcessorsFactory>();
		serviceProvider.GetRequiredService<IBeforeCommitProcessorsRunner>();
		var languageBeforeCommitProcessors = factory.Create<Language>().ToList();
		var objectBeforeCommitProcessors = factory.Create<object>().ToList();

		// Assert
		Assert.AreEqual(0, languageBeforeCommitProcessors.Count);
		Assert.AreEqual(1, objectBeforeCommitProcessors.Count);

		Assert.IsInstanceOfType(objectBeforeCommitProcessors.Single(), typeof(SetCreatedToInsertingEntitiesBeforeCommitProcessor));
	}


	[TestMethod]
	public void EntityPatternsInstaller_DataSeedRunnerIsRegistered()
	{
		// Arrange
		IServiceProvider serviceProvider = CreateAndSetupServiceProvider();

		// Act
		using var scope = serviceProvider.CreateScope();

		var dataSeedRunner = scope.ServiceProvider.GetRequiredService<IDataSeedRunner>();

		// Assert
		Assert.IsInstanceOfType(dataSeedRunner, typeof(DbDataSeedRunner));
	}

	internal static IServiceProvider CreateAndSetupServiceProvider(bool pooling = false)
	{
		var host = Host.CreateDefaultBuilder()
			.ConfigureServices(services =>
			{

				var installer = services.WithEntityPatternsInstaller();
				if (pooling)
				{
					installer = installer.AddDbContextPool<TestDbContext>(options => options.UseInMemoryDatabase(nameof(TestDbContext)));
				}
				else
				{
					installer = installer.AddDbContext<TestDbContext>();
				}

				installer.AddEntityPatterns()
					.AddLocalizationServices<Language>()
					.AddDataLayer(typeof(ILanguageDataSource).Assembly);

				services.AddSingleton<ITimeService, ServerTimeService>();
				services.AddSingleton<ICacheService, NullCacheService>();
			}).Build();

		return host.Services;
	}
}
