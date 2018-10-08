using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Havit.CastleWindsor.WebForms;
using Havit.WebApplicationTest.HavitCastleWindsorWebFormsTests;

namespace Havit.WebApplicationTest.App_Start
{
	public static class WindsorCastleConfig
	{
		public static void RegisterDiContainer(HttpApplication application)
		{
			IWindsorContainer container = application.AddWindsorContainer();
			
			// umožní resolvovat i kolekce závislostí - IEnumerable<IDependency>
			container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel));

			// facilities
			container.AddFacility<TypedFactoryFacility>();

			container.Register(
				Component.For<IDisposableComponent>().ImplementedBy<DisposableComponent>().LifestyleTransient(),
				Component.For<IDisposableComponent>().ImplementedBy<AnotherDisposableComponent>().LifestyleTransient()
				);

		}
	}
}