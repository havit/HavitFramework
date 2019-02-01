using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Havit.Data.Entity.Patterns.UnitOfWorks;
using Havit.Data.Entity.Patterns.UnitOfWorks.BeforeCommitProcessors;
using Havit.Data.Entity.Patterns.Windsor;
using Havit.Data.Entity.Patterns.Windsor.Installers;
using Havit.Data.Entity6.Patterns.Windsor.Tests.Infrastructure.DataLayer;
using Havit.Data.Entity6.Patterns.Windsor.Tests.Infrastructure.Entity;
using Havit.Data.Entity6.Patterns.Windsor.Tests.Infrastructure.Model;
using Havit.Data.Patterns.DataLoaders;
using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.Localizations;
using Havit.Data.Patterns.UnitOfWorks;
using Havit.Services;
using Havit.Services.TimeServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.Entity6.Patterns.Windsor.Tests
{
	[TestClass]
	public class EntityPatternsInstallerTests
	{
		[TestMethod]
		public void EntityPatternsInstaller_ShouldRegisterLanguageAndLocalizationServices()
		{
			// Arrange
			var container = CreateAndSetupWindsorContainer();

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
		public void EntityPatternsInstaller_ShouldRegisterDataLoaderAndDependencies()
		{
			// Arrange
			var container = CreateAndSetupWindsorContainer();

			// Act
			using (container.BeginScope())
			{
				container.Resolve<IDataLoader>();
			}

			// Assert
			// no exception was thrown
		}

		[TestMethod]
		public void EntityPatternsInstaller_ShouldRegisterUnitOfWorkAndDependencies()
		{
			// Arrange
			var container = CreateAndSetupWindsorContainer();

			// Act
			using (container.BeginScope())
			{
				container.Resolve<IUnitOfWork>();
			}

			// Assert
			// no exception was thrown
		}

		[TestMethod]
		public void EntityPatternsInstaller_ShouldRegisterDataSeedRunnerAndDependencies()
		{
			// Arrange
			var container = CreateAndSetupWindsorContainer();

			// Act
			using (container.BeginScope())
			{
				container.Resolve<IDataSeedRunner>();
			}

			// Assert
			// no exception was thrown
		}


		[TestMethod]
		public void EntityPatternsInstaller_ShouldRegisterBeforeCommitProcessorsServicesAndDependencies()
		{
			// Arrange
			WindsorContainer container = CreateAndSetupWindsorContainer();

			// Act
			IBeforeCommitProcessorsFactory factory = container.Resolve<IBeforeCommitProcessorsFactory>();
			container.Resolve<IBeforeCommitProcessorsRunner>();
			factory.Create<Language>();
			factory.Create<object>();

			// Assert
			// no exception was thrown and...
		}

		private static WindsorContainer CreateAndSetupWindsorContainer()
		{
			WindsorContainer container = new WindsorContainer();
			container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel));

			container.AddFacility<TypedFactoryFacility>();
			container.Register(Component.For(typeof(IServiceFactory<>)).AsFactory());
			container.WithEntityPatternsInstaller(new ComponentRegistrationOptions { GeneralLifestyle = l => l.Scoped() })
				.RegisterEntityPatterns()
				.RegisterDbContext<TestDbContext>()
				.RegisterLocalizationServices<Language>()
				.RegisterDataLayer(typeof(ILanguageDataSource).Assembly);

			container.Register(Component.For<ITimeService>().ImplementedBy<ServerTimeService>().LifestyleSingleton());

			return container;
		}
	}
}
