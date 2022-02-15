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

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DependencyInjection
{
	[TestClass]
	public class ServiceCollectionEntityPatternsInstallerTests
	{
		[TestMethod]
		public void ServiceCollectionEntityPatternsInstaller_DbContextIsScoped()
		{
			// Arrange
			IServiceProvider serviceProvider = CreateAndSetupServiceProvider();

			// Act
			IDbContext dbContext1a;
			IDbContext dbContext1b;
			IDbContext dbContext2;

			using (var scope = serviceProvider.CreateScope())
			{
				dbContext1a = scope.ServiceProvider.GetRequiredService<IDbContext>();
				dbContext1b = scope.ServiceProvider.GetRequiredService<IDbContext>();
			}

			using (var scope = serviceProvider.CreateScope())
			{
				dbContext2 = scope.ServiceProvider.GetRequiredService<IDbContext>();
			}

			// Assert			
			Assert.AreSame(dbContext1a, dbContext1b);
			Assert.AreNotSame(dbContext1a, dbContext2);
			Assert.AreNotSame(dbContext1b, dbContext2);
		}

		[TestMethod]
		public void ServiceCollectionEntityPatternsInstallerShouldRegisterDbContextTransientAsTransient()
		{
			// Arrange
			IServiceProvider serviceProvider = CreateAndSetupServiceProvider();

			// Act
			IDbContextTransient dbContext1;
			IDbContextTransient dbContext2;

			using (var scope = serviceProvider.CreateScope())
			{
				dbContext1 = scope.ServiceProvider.GetRequiredService<IDbContextTransient>();
				dbContext2 = scope.ServiceProvider.GetRequiredService<IDbContextTransient>();
			}

			// Assert
			Assert.AreNotSame(dbContext1, dbContext2);
		}

		[TestMethod]
		public void ServiceCollectionEntityPatternsInstaller_ShouldRegisterLanguageAndLocalizationServices()
		{
			// Arrange
			IServiceProvider serviceProvider = CreateAndSetupServiceProvider();

			// Act
			using (var scope = serviceProvider.CreateScope())
			{
				scope.ServiceProvider.GetRequiredService<ILanguageService>();
				scope.ServiceProvider.GetRequiredService<ILocalizationService>();
			}

			// Assert			
			// no exception was thrown
		}

		[TestMethod]
		public void ServiceCollectionEntityPatternsInstaller_ShouldRegisterDataSourcesAndDependencies()
		{
			// Arrange
			IServiceProvider serviceProvider = CreateAndSetupServiceProvider();

			// Act
			using (var scope = serviceProvider.CreateScope())
			{
				scope.ServiceProvider.GetRequiredService<ILanguageDataSource>();
				scope.ServiceProvider.GetRequiredService<IDataSource<Language>>();
			}

			// Assert			
			// no exception was thrown
		}

		[TestMethod]
		public void ServiceCollectionEntityPatternsInstaller_ShouldRegisterRepositoriesAndDependencies()
		{
			// Arrange
			IServiceProvider serviceProvider = CreateAndSetupServiceProvider();

			// Act
			using (var scope = serviceProvider.CreateScope())
			{
				scope.ServiceProvider.GetRequiredService<ILanguageRepository>();
				scope.ServiceProvider.GetRequiredService<IRepository<Language>>();
			}

			// Assert			
			// no exception was thrown
		}

		[TestMethod]
		public void ServiceCollectionEntityPatternsInstaller_ShouldRegisterDataLoaderAndDependencies()
		{
			// Arrange
			IServiceProvider serviceProvider = CreateAndSetupServiceProvider();

			// Act
			using (var scope = serviceProvider.CreateScope())
			{
				scope.ServiceProvider.GetRequiredService<IDataLoader>();
			}
			// Assert
			// no exception was thrown
		}

		[TestMethod]
		public void ServiceCollectionEntityPatternsInstaller_ShouldRegisterUnitOfWorkAndDependencies()
		{
			// Arrange
			IServiceProvider serviceProvider = CreateAndSetupServiceProvider();

			// Act
			using (var scope = serviceProvider.CreateScope())
			{
				scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
			}

			// Assert
			// no exception was thrown
		}

		[TestMethod]
		public void ServiceCollectionEntityPatternsInstaller_ShouldRegisterEntityCacheManager()
		{
			// Arrange
			IServiceProvider serviceProvider = CreateAndSetupServiceProvider();

			// Act
			using (var scope = serviceProvider.CreateScope())
			{
				scope.ServiceProvider.GetRequiredService<IEntityCacheManager>();
			}

			// Assert
			// no exception was thrown
		}

		[TestMethod]
		public void ServiceCollectionEntityPatternsInstaller_ShouldRegisterBeforeCommitProcessorsServicesAndDependencies()
		{
			// Arrange
			IServiceProvider serviceProvider = CreateAndSetupServiceProvider();

			// Act
			IBeforeCommitProcessorsFactory factory = serviceProvider.GetRequiredService<IBeforeCommitProcessorsFactory>();
			serviceProvider.GetRequiredService<IBeforeCommitProcessorsRunner>();
			factory.Create<Language>();
			factory.Create<object>();

			// Assert
			// no exception was thrown and...
		}

		[TestMethod]
		public void ServiceCollectionEntityPatternsInstaller_ShouldRegisterDataSeedRunnerAndDependencies()
		{
			// Arrange
			IServiceProvider serviceProvider = CreateAndSetupServiceProvider();

			// Act
			using (var scope = serviceProvider.CreateScope())
			{
				scope.ServiceProvider.GetRequiredService<IDataSeedRunner>();
			}

			// Assert
			// no exception was thrown and...
		}

		[TestMethod]
		public void ServiceCollectionEntityPatternsInstaller_DbDataSeedPersister_DbContextDependencyIsTransient()
		{
			// Arrange
			IServiceProvider serviceProvider = CreateAndSetupServiceProvider();

			// Act
			IDataSeedPersister dataSeedPersister1;
			IDataSeedPersister dataSeedPersister2;

			using (var scope = serviceProvider.CreateScope())
			{
                dataSeedPersister1 = scope.ServiceProvider.GetRequiredService<IDataSeedPersister>();
				dataSeedPersister2 = scope.ServiceProvider.GetRequiredService<IDataSeedPersister>();
			}

			// Assert
			Assert.IsInstanceOfType(dataSeedPersister1, typeof(DbDataSeedPersister));
			Assert.IsInstanceOfType(dataSeedPersister2, typeof(DbDataSeedPersister));
			Assert.AreNotSame(((DbDataSeedPersister)dataSeedPersister1).DbContext, ((DbDataSeedPersister)dataSeedPersister2).DbContext);
		}

		internal static IServiceProvider CreateAndSetupServiceProvider()
		{
			IServiceCollection services = new ServiceCollection();

			services.WithEntityPatternsInstaller()
				.AddEntityPatterns()
				.AddDbContext<TestDbContext>()
				.AddLocalizationServices<Language>()
				.AddDataLayer(typeof(ILanguageDataSource).Assembly);

			services.AddSingleton<ITimeService, ServerTimeService>();
			services.AddSingleton<ICacheService, NullCacheService>();

			return services.BuildServiceProvider(true);
		}
	}
}
