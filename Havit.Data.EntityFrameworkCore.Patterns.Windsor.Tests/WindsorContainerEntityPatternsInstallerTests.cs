using Castle.Facilities.TypedFactory;
using Castle.Facilities.TypedFactory.Internal;
using Castle.MicroKernel;
using Castle.MicroKernel.Handlers;
using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Diagnostics;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.DataSeeds;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors;
using Havit.Data.EntityFrameworkCore.Patterns.Windsor.Installers;
using Havit.Data.EntityFrameworkCore.TestHelpers.DependencyInjection.Infrastructure.Entity;
using Havit.Data.Patterns.DataLoaders;
using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.DataSources;
using Havit.Data.Patterns.Localizations;
using Havit.Data.Patterns.Repositories;
using Havit.Data.Patterns.UnitOfWorks;
using Havit.Services;
using Havit.Services.TimeServices;
using Havit.TestHelpers.CastleWindsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Text;
using Havit.Data.EntityFrameworkCore.TestHelpers.DependencyInjection.Infrastructure.DataLayer;
using Havit.Data.EntityFrameworkCore.TestHelpers.DependencyInjection.Infrastructure.Model;
using Havit.Services.Caching;
using System;

namespace Havit.Data.EntityFrameworkCore.Patterns.Windsor.Tests
{
	[TestClass]
	public class WindsorContainerEntityPatternsInstallerTests
	{
		[TestMethod]
		public void WindsorContainerEntityPatternsInstaller_RegisteredComponentsShouldHaveRegisteredDependencies()
		{
			// Arrange + Act
			var container = Helpers.CreateAndSetupWindsorContainer();

			// Assert
			MisconfiguredComponentsHelper.AssertMisconfiguredComponents(container);
		}

		[TestMethod]
		public void WindsorContainerEntityPatternsInstaller_RegisteredComponentsShouldNotHaveLifestyleMismatches()
		{
			// Arrange + Act
			var container = Helpers.CreateAndSetupWindsorContainer();

			// Assert
			PotentialLifestyleMismatchesHelper.AssertPotentialLifestyleMismatches(container, cm => cm.Implementation != typeof(TypedFactoryInterceptor));
		}

		[TestMethod]
		public void WindsorContainerEntityPatternsInstaller_ShouldRegisterLanguageAndLocalizationServices()
		{
			// Arrange
			var container = Helpers.CreateAndSetupWindsorContainer();

			// Act
			using (container.BeginScope())
			{
				container.Resolve<ILanguageService>();
				container.Resolve<ILocalizationService>();
			}

			// Assert			
			// no exception was thrown
		}

		[TestMethod]
		public void WindsorContainerEntityPatternsInstaller_ShouldRegisterDataSourcesAndDependencies()
		{
			// Arrange
			var container = Helpers.CreateAndSetupWindsorContainer();

			// Act
			using (container.BeginScope())
			{
				container.Resolve<ILanguageDataSource>();
				container.Resolve<IDataSource<Language>>();
			}

			// Assert			
			// no exception was thrown
		}

		[TestMethod]
		public void WindsorContainerEntityPatternsInstaller_ShouldRegisterRepositoriesAndDependencies()
		{
			// Arrange
			var container = Helpers.CreateAndSetupWindsorContainer();

			// Act
			using (container.BeginScope())
			{
				container.Resolve<ILanguageRepository>();
				container.Resolve<IRepository<Language>>();
			}

			// Assert			
			// no exception was thrown
		}

		[TestMethod]
		public void WindsorContainerEntityPatternsInstaller_ShouldRegisterDataLoaderAndDependencies()
		{
			// Arrange
			var container = Helpers.CreateAndSetupWindsorContainer();

			// Act
			using (container.BeginScope())
			{
				container.Resolve<IDataLoader>();
			}
			// Assert
			// no exception was thrown
		}

		[TestMethod]
		public void WindsorContainerEntityPatternsInstaller_ShouldRegisterUnitOfWorkAndDependencies()
		{
			// Arrange
			var container = Helpers.CreateAndSetupWindsorContainer();

			// Act
			using (container.BeginScope())
			{
				container.Resolve<IUnitOfWork>();
			}

			// Assert
			// no exception was thrown
		}

		[TestMethod]
		public void WindsorContainerEntityPatternsInstaller_ShouldRegisterEntityCacheManager()
		{
			// Arrange
			var container = Helpers.CreateAndSetupWindsorContainer();

			// Act
			using (container.BeginScope())
			{
				container.Resolve<IEntityCacheManager>();
			}

			// Assert
			// no exception was thrown
		}

		[TestMethod]
		public void WindsorContainerEntityPatternsInstaller_ShouldRegisterBeforeCommitProcessorsServicesAndDependencies()
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
		public void WindsorContainerEntityPatternsInstaller_ShouldRegisterDataSeedRunnerAndDependencies()
		{
			// Arrange
			WindsorContainer container = Helpers.CreateAndSetupWindsorContainer();

			// Act
			using (container.BeginScope())
			{
				container.Resolve<IDataSeedRunner>();
			}

			// Assert
			// no exception was thrown and...
		}

		[TestMethod]
		public void WindsorContainerEntityPatternsInstaller_DbDataSeedPersister_DbContextDependencyIsTransient()
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
