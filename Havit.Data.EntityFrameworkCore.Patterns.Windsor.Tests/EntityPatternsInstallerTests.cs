using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors;
using Havit.Data.EntityFrameworkCore.Patterns.Windsor.Installers;
using Havit.Data.EntityFrameworkCore.Patterns.Windsor.Tests.Infrastructure.DataLayer;
using Havit.Data.EntityFrameworkCore.Patterns.Windsor.Tests.Infrastructure.Entity;
using Havit.Data.EntityFrameworkCore.Patterns.Windsor.Tests.Infrastructure.Model;
using Havit.Data.Patterns.DataLoaders;
using Havit.Data.Patterns.Localizations;
using Havit.Data.Patterns.UnitOfWorks;
using Havit.Services.TimeServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.Patterns.Windsor.Tests
{
	[TestClass]
	public class EntityPatternsInstallerTests
	{
		[TestMethod]
		public void EntityPatternsInstaller_RegisterLocalizationServices_ShouldRegisterLanguageAndLocalizationServices()
		{
			// Arrange
			var container = CreateAndSetupWindsorContainer();

			// Act
			container.Resolve<ILanguageService>();
			container.Resolve<ILocalizationService>();

			// Assert			
			// no exception was thrown
		}

		[TestMethod]
		public void EntityPatternsInstaller_RegisterLocalizationServices_ShouldRegisterDataLoaderAndDependencies()
		{
			// Arrange
			var container = CreateAndSetupWindsorContainer();

			// Act
			container.Resolve<IDataLoader>();

			// Assert
			// no exception was thrown
		}

		[TestMethod]
		public void EntityPatternsInstaller_RegisterLocalizationServices_ShouldRegisterUnitOfWorkAndDependencies()
		{
			// Arrange
			var container = CreateAndSetupWindsorContainer();

			// Act
			container.Resolve<IUnitOfWork>();

			// Assert
			// no exception was thrown
		}

		[TestMethod]
		public void EntityPatternsInstaller_RegisterLocalizationServices_ShouldRegisterBeforeCommitProcessorsServicesAndDependencies()
		{
			// Arrange
			WindsorContainer container = new WindsorContainer();
			container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel, false));

			container.AddFacility<TypedFactoryFacility>();
			container.WithEntityPatternsInstaller(new ComponentRegistrationOptions { GeneralLifestyle = lf => lf.Singleton })
				.RegisterEntityPatterns()
				.RegisterDbContext<TestDbContext>()
				.RegisterLocalizationServices<Language>()
				.RegisterDataLayer(typeof(ILanguageDataSource).Assembly);

			container.Register(Component.For<ITimeService>().ImplementedBy<ServerTimeService>().LifestyleSingleton());

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
			container.WithEntityPatternsInstaller(new ComponentRegistrationOptions { GeneralLifestyle = lf => lf.Singleton })
				.RegisterEntityPatterns()
				.RegisterDbContext<TestDbContext>()
				.RegisterLocalizationServices<Language>()
				.RegisterDataLayer(typeof(ILanguageDataSource).Assembly);

			container.Register(Component.For<ITimeService>().ImplementedBy<ServerTimeService>().LifestyleSingleton());

			return container;
		}
	}
}
