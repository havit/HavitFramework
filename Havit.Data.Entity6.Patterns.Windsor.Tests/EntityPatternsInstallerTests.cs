using System;
using Castle.Facilities.TypedFactory;
using Castle.Windsor;
using Havit.Data.Entity.Patterns.Windsor;
using Havit.Data.Entity.Patterns.Windsor.Installers;
using Havit.Data.Entity6.Patterns.Windsor.Tests.Infrastructure.DataLayer;
using Havit.Data.Entity6.Patterns.Windsor.Tests.Infrastructure.Entity;
using Havit.Data.Entity6.Patterns.Windsor.Tests.Infrastructure.Model;
using Havit.Data.Patterns.Localizations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.Entity6.Patterns.Windsor.Tests
{
	[TestClass]
	public class EntityPatternsInstallerTests
	{
		[TestMethod]
		public void EntityPatternsInstaller_RegisterLocalizationServices_ShouldRegisterLanguageAndLocalizationServices()
		{
			// Arrange
			WindsorContainer container = new WindsorContainer();
			container.AddFacility<TypedFactoryFacility>();
			container.WithEntityPatternsInstaller(new WebApplicationComponentRegistrationOptions())
				.RegisterEntityPatterns()
				.RegisterDbContext<TestDbContext>()
				.RegisterLocalizationServices<Language>()
				.RegisterDataLayer(typeof(ILanguageDataSource).Assembly);

			// Act
			container.Resolve<ILanguageService>();
			container.Resolve<ILocalizationService>();

			// Assert
			// no exception was thrown
		}
	}
}
