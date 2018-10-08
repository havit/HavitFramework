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
	/// Extension methods of HttpApplication that help use Castle Windsor container.
	/// </summary>
	public static class HttpApplicationExtensions
	{
		/// <summary>
		/// Register Castle Windsor Container to HttpRuntime.WebObjectActivator and return its instance.
		/// If Castle Windsor Container is currently registered, returns the registered Castle Windsor Container instance.
		/// </summary>		
		public static IWindsorContainer AddWindsorContainer(this HttpApplication application)
		{
			// ...(this HttpApplication application) - jen aby šlo použít jako extension metodu

			if (application == null)
			{
				throw new ArgumentNullException(nameof(application));
			}

			return WindsorContainerAdapter.AddWindsorContainer();
		}

		/// <summary>
		/// Get registered Castle Windsor Container instance (or null if not registered).
		/// </summary>
		public static IWindsorContainer GetWindsorContainer()
		{
			return WindsorContainerAdapter.GetWindsorContainer();
		}
	}

}
