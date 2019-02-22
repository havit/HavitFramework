using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.DataSeeds;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors;
using Havit.Data.EntityFrameworkCore.Patterns.Windsor.Installers;
using Havit.Data.EntityFrameworkCore.Patterns.Windsor.Tests.Infrastructure.DataLayer;
using Havit.Data.EntityFrameworkCore.Patterns.Windsor.Tests.Infrastructure.Entity;
using Havit.Data.EntityFrameworkCore.Patterns.Windsor.Tests.Infrastructure.Model;
using Havit.Data.Patterns.DataLoaders;
using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.DataSources;
using Havit.Data.Patterns.Localizations;
using Havit.Data.Patterns.Repositories;
using Havit.Data.Patterns.UnitOfWorks;
using Havit.Services;
using Havit.Services.TimeServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.Patterns.Windsor.Tests
{
	[TestClass]
	public class EntityPatternsInstallerTests
	{
		[TestMethod]
		public void EntityPatternsInstaller_ShouldRegisterDbContextFactory()
		{
			// Arrange
			var container = Helpers.CreateAndSetupWindsorContainer();

			// Act
			container.Resolve<IDbContextFactory>();

			// Assert			
			// no exception was thrown
		}

		[TestMethod]
		public void EntityPatternsInstaller_ShouldRegisterLanguageAndLocalizationServices()
		{
			// Arrange
			var container = Helpers.CreateAndSetupWindsorContainer();

			// Act
			container.Resolve<ILanguageService>();
			container.Resolve<ILocalizationService>();

			// Assert			
			// no exception was thrown
		}

		[TestMethod]
		public void EntityPatternsInstaller_ShouldRegisterDataSourcesAndDependencies()
		{
			// Arrange
			var container = Helpers.CreateAndSetupWindsorContainer();

			// Act
			container.Resolve<ILanguageDataSource>();
			container.Resolve<IDataSource<Language>>();

			// Assert			
			// no exception was thrown
		}

		[TestMethod]
		public void EntityPatternsInstaller_ShouldRegisterRepositoriesAndDependencies()
		{
			// Arrange
			var container = Helpers.CreateAndSetupWindsorContainer();

			// Act
			container.Resolve<ILanguageRepository>();
			container.Resolve<IRepository<Language>>();
			container.Resolve<IRepositoryAsync<Language>>();

			// Assert			
			// no exception was thrown
		}

		[TestMethod]
		public void EntityPatternsInstaller_ShouldRegisterDataLoaderAndDependencies()
		{
			// Arrange
			var container = Helpers.CreateAndSetupWindsorContainer();

			// Act
			container.Resolve<IDataLoader>();
			container.Resolve<IDataLoaderAsync>();

			// Assert
			// no exception was thrown
		}

		[TestMethod]
		public void EntityPatternsInstaller_ShouldRegisterUnitOfWorkAndDependencies()
		{
			// Arrange
			var container = Helpers.CreateAndSetupWindsorContainer();

			// Act
			container.Resolve<IUnitOfWork>();
			container.Resolve<IUnitOfWorkAsync>();

			// Assert
			// no exception was thrown
		}

		[TestMethod]
		public void EntityPatternsInstaller_ShouldRegisterEntityCacheManager()
		{
			// Arrange
			var container = Helpers.CreateAndSetupWindsorContainer();

			// Act
			container.Resolve<IEntityCacheManager>();

			// Assert
			// no exception was thrown
		}

		[TestMethod]
		public void EntityPatternsInstaller_ShouldRegisterBeforeCommitProcessorsServicesAndDependencies()
		{
			// Arrange
			WindsorContainer container = Helpers.CreateAndSetupWindsorContainer();

			// Act
			IBeforeCommitProcessorsFactory factory = container.Resolve<IBeforeCommitProcessorsFactory>();
			container.Resolve<IBeforeCommitProcessorsRunner>();
			factory.Create<Language>();
			factory.Create<object>();

			// Assert
			// no exception was thrown and...
		}

		[TestMethod]
		public void EntityPatternsInstaller_ShouldRegisterDataSeedRunnerAndDependencies()
		{
			// Arrange
			WindsorContainer container = Helpers.CreateAndSetupWindsorContainer();

			// Act
			container.Resolve<IDataSeedRunner>();

			// Assert
			// no exception was thrown and...
		}

		[TestMethod]
		public void EntityPatternsInstaller_DbDataSeedPersister_DbContextDependencyIsTransient()
		{
			// Arrange
			WindsorContainer container = Helpers.CreateAndSetupWindsorContainer();

			// Act
			var dataSeedPersister1  = container.Resolve<IDataSeedPersister>();
			var dataSeedPersister2 = container.Resolve<IDataSeedPersister>();

			// Assert
			Assert.IsInstanceOfType(dataSeedPersister1, typeof(DbDataSeedPersister));
			Assert.IsInstanceOfType(dataSeedPersister2, typeof(DbDataSeedPersister));
			Assert.AreNotSame(((DbDataSeedPersister)dataSeedPersister1).DbContext, ((DbDataSeedPersister)dataSeedPersister2).DbContext);
		}
	}
}
