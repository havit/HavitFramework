using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Havit.CastleWindsor.WebForms;
using WebApplicationTest.HavitCastleWindsorWebFormsTests;

namespace WebApplicationTest.App_Start
{
	public class WindsorCastleConfig
	{
		public static void RegisterDiContainer()
		{
			IWindsorContainer container = new WindsorContainer();
			container.Register(Component.For<IDisposableComponent>().ImplementedBy<DisposableComponent>().LifestyleTransient());

			DependencyInjectionHandlerFactoryHelper.SetResolver(container);

		}
	}
}