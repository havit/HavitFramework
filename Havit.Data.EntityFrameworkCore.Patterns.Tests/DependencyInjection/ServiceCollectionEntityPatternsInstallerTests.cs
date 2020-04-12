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

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DependencyInjection
{
	[TestClass]
	public class ServiceCollectionEntityPatternsInstallerTests
	{
		[TestMethod]
		public void ServiceCollectionEntityPatternsInstaller_ShouldRegisterDbContextFactory()
		{
			// Arrange
			IServiceProvider serviceProvider = CreateAndSetupServiceProvider();

			// Act
			serviceProvider.GetRequiredService<IDbContextFactory>();

			// Assert			
			// no exception was thrown
		}

		/// <summary>
		/// repro #49402 ServiceCollectionServiceInstaller - resolvování Scoped services ze service-factories padá na InvalidOperationException: Cannot resolve scoped service XYZ from root provider.
		/// </summary>
		[TestMethod]
		public void ServiceCollectionEntityPatternsInstaller_ShouldResolveScopedDbContext()
		{
			// Arrange
			IServiceProvider serviceProvider = CreateAndSetupServiceProvider();
			var s1 = serviceProvider.GetService<IDbContextFactory>();

			// Act
			IDbContext dbContextScope1 = null;
			using (var scope1 = serviceProvider.CreateScope())
			{
				var service = scope1.ServiceProvider.GetRequiredService<IDbContextFactory>();
				service.ExecuteAction(dbContext =>
				{
					dbContextScope1 = dbContext;
				});
			}

			using (var scope = serviceProvider.CreateScope())
			{
				var service = scope.ServiceProvider.GetRequiredService<IDbContextFactory>();
				service.ExecuteAction(dbContextScope2 =>
				{
					// assert
					Assert.IsNotNull(dbContextScope2);
					Assert.AreNotSame(dbContextScope1, dbContextScope2);
				});
			}
		}

		[TestMethod]
		public void ServiceCollectionEntityPatternsInstaller_ShouldRegisterLanguageAndLocalizationServices()
		{
			// Arrange
			IServiceProvider serviceProvider = CreateAndSetupServiceProvider();

			// Act
			using (serviceProvider.CreateScope())
			{
				serviceProvider.GetRequiredService<ILanguageService>();
				serviceProvider.GetRequiredService<ILocalizationService>();
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
			using (serviceProvider.CreateScope())
			{
				serviceProvider.GetRequiredService<ILanguageDataSource>();
				serviceProvider.GetRequiredService<IDataSource<Language>>();
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
			using (serviceProvider.CreateScope())
			{
				serviceProvider.GetRequiredService<ILanguageRepository>();
				serviceProvider.GetRequiredService<IRepository<Language>>();
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
			using (serviceProvider.CreateScope())
			{
				serviceProvider.GetRequiredService<IDataLoader>();
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
			using (serviceProvider.CreateScope())
			{
				serviceProvider.GetRequiredService<IUnitOfWork>();
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
			using (serviceProvider.CreateScope())
			{
				serviceProvider.GetRequiredService<IEntityCacheManager>();
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
			using (serviceProvider.CreateScope())
			{
				serviceProvider.GetRequiredService<IDataSeedRunner>();
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
			var dataSeedPersister1 = serviceProvider.GetRequiredService<IDataSeedPersister>();
			var dataSeedPersister2 = serviceProvider.GetRequiredService<IDataSeedPersister>();

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

			return services.BuildServiceProvider();
		}
	}
}
