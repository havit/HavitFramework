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
				serviceProvider.GetRequiredService<IRepositoryAsync<Language>>();
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
				serviceProvider.GetRequiredService<IDataLoaderAsync>();
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
				serviceProvider.GetRequiredService<IUnitOfWorkAsync>();
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
