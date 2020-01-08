using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Havit.Data.EntityFrameworkCore.Patterns.Windsor.Installers;
using Havit.Data.EntityFrameworkCore.TestHelpers.DependencyInjection.Infrastructure.DataLayer;
using Havit.Data.EntityFrameworkCore.TestHelpers.DependencyInjection.Infrastructure.Entity;
using Havit.Data.EntityFrameworkCore.TestHelpers.DependencyInjection.Infrastructure.Model;
using Havit.Services;
using Havit.Services.Caching;
using Havit.Services.TimeServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Patterns.Windsor.Tests
{
	internal static class Helpers
	{
		internal static WindsorContainer CreateAndSetupWindsorContainer(Action<WindsorContainerComponentRegistrationOptions> componentRegistrationAction = null)
		{
			WindsorContainer container = new WindsorContainer();
			container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel));

			container.AddFacility<TypedFactoryFacility>();
			container.WithEntityPatternsInstaller(componentRegistrationAction ?? (c => c.GeneralLifestyle = lf => lf.Scoped()))
				.AddEntityPatterns()
				.AddDbContext<TestDbContext>()
				.AddLocalizationServices<Language>()
				.AddDataLayer(typeof(ILanguageDataSource).Assembly);

			container.Register(Component.For<ITimeService>().ImplementedBy<ServerTimeService>().LifestyleSingleton());
			container.Register(Component.For<ICacheService>().ImplementedBy<NullCacheService>().LifestyleSingleton());

			return container;
		}
	}
}
