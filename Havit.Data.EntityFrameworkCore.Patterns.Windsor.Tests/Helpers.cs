using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Havit.Data.EntityFrameworkCore.Patterns.Windsor.Installers;
using Havit.Data.EntityFrameworkCore.Patterns.Windsor.Tests.Infrastructure.DataLayer;
using Havit.Data.EntityFrameworkCore.Patterns.Windsor.Tests.Infrastructure.Entity;
using Havit.Data.EntityFrameworkCore.Patterns.Windsor.Tests.Infrastructure.Model;
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
		internal static WindsorContainer CreateAndSetupWindsorContainer(ComponentRegistrationOptions componentRegistrationOptions = null)
		{
			WindsorContainer container = new WindsorContainer();
			container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel));

			container.AddFacility<TypedFactoryFacility>();
			container.Register(Component.For(typeof(IServiceFactory<>)).AsFactory());
			container.WithEntityPatternsInstaller(componentRegistrationOptions ?? new ComponentRegistrationOptions { GeneralLifestyle = lf => lf.Singleton })
				.RegisterEntityPatterns()
				.RegisterDbContext<TestDbContext>()
				.RegisterLocalizationServices<Language>()
				.RegisterDataLayer(typeof(ILanguageDataSource).Assembly);

			container.Register(Component.For<ITimeService>().ImplementedBy<ServerTimeService>().LifestyleSingleton());
			container.Register(Component.For<ICacheService>().ImplementedBy<NullCacheService>().LifestyleSingleton());

			return container;
		}
	}
}
