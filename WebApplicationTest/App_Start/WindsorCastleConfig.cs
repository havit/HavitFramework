using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Havit.CastleWindsor.WebForms;
using Havit.WebApplicationTest.HavitCastleWindsorWebFormsTests;

namespace Havit.WebApplicationTest.App_Start
{
	public static class WindsorCastleConfig
	{
		public static void RegisterDiContainer()
		{
			IWindsorContainer container = new WindsorContainer();
			container.Register(
				Component.For<IDisposableComponent>().ImplementedBy<DisposableComponent>().LifestyleTransient(),
				Component.For<IDisposableComponent>().ImplementedBy<AnotherDisposableComponent>().LifestyleTransient()
				);

			DependencyInjectionWebFormsHelper.SetResolver(container);

		}
	}
}