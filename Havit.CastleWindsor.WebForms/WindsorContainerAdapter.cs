using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Havit.CastleWindsor.WebForms
{
	/// <summary>
	/// Extension methods of HttpApplication that help use Castle Windsor Container.
	/// </summary>
	public static class WindsorContainerAdapter
	{
		private static object _lock = new object();

		/// <summary>
		/// Add a new Castle Windsor Container in ASP.NET application. If there is WebObjectActivator already registered,
		/// it will return the registered isntance. When the new WebObjectActivator can't resolve the type, the previous WebObjectActivator
		/// will be used. If the previous WebObjectActivator can't resolve it either, DefaultCreateInstance will be used
		/// which creates instance through none public default constructor based on reflection.
		/// </summary>
		public static IWindsorContainer AddWindsorContainer()
		{
			lock (_lock)
			{
				var registeredWindsorContainer = GetWindsorContainer();
				if (registeredWindsorContainer != null)
				{
					return registeredWindsorContainer;
				}

				var provider = new ContainerServiceProvider(HttpRuntime.WebObjectActivator);
				HttpRuntime.WebObjectActivator = provider;

				return provider.Container;
			}
		}

		/// <summary>
		/// Get most recent added Unity container
		/// </summary>
		public static IWindsorContainer GetWindsorContainer()
		{
			return (HttpRuntime.WebObjectActivator as ContainerServiceProvider)?.Container;
		}
	}
}