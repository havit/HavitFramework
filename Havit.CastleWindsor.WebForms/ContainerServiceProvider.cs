using Castle.MicroKernel.Registration;
using Castle.Windsor;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;

namespace Havit.CastleWindsor.WebForms
{
	/// <summary>
	/// The Castle Windsor adapter for WebObjectActivator.
	/// </summary>
	internal class ContainerServiceProvider : IServiceProvider, IRegisteredObject
	{
		public IWindsorContainer Container { get; } = new WindsorContainer();

		internal IServiceProvider NextServiceProvider { get; }

		private const int TypesCannotResolveCacheCap = 100000;
		private readonly ConcurrentDictionary<Type, bool> _typesCannotResolve = new ConcurrentDictionary<Type, bool>(); // there is no ConcurrentHashSet in .NET FW.

		public ContainerServiceProvider(IServiceProvider next)
		{
			NextServiceProvider = next;
			HostingEnvironment.RegisterObject(this);
		}

		/// <summary>
		/// Implementation of IServiceProvider. Asp.net will call this method to
		/// create the instances of Page/UserControl/HttpModule etc. 
		/// Unfortunatelly not WebServices (.asmx)
		/// </summary>
		public object GetService(Type serviceType)
		{
			// Try unresolvable types - we cache them
			if (_typesCannotResolve.ContainsKey(serviceType))
			{
				return DefaultCreateInstance(serviceType);
			}

			// Try the container
			object result = null;

			lock (serviceType) // chrání před pokusem o opakovanou registraci do CastleWindsor při prvním paralelním přístupu na stránku
			{
				// We must register dynamically compiled resources (pages, controls, master pages, handlers ...)
				if ((typeof(UserControl).IsAssignableFrom(serviceType) ||    // User controls (.ascx) and event Master Pages (.master) inherit from UserControl
					typeof(IHttpHandler).IsAssignableFrom(serviceType)) &&   // Generic handlers (.ashx) and also pages (.aspx) inherit from IHttpHandler
					!Container.Kernel.HasComponent(serviceType))
				{
					// Lifestyle is *Transient* 
					// If it would be PerWebRequest, we couldn't use the same control on one page twice - resolved would be only the first, and the second would be reused)
					Container.Register(Component.For(serviceType).ImplementedBy(serviceType).LifestyleTransient());
				}
			}

			// If we have component registered, we will resolve the service
			if (Container.Kernel.HasComponent(serviceType))
			{
				result = Container.Resolve(serviceType);
				// And because transient, we must release component on end request - else we would make memory leaks
				HttpContext.Current.AddOnRequestCompleted(_ => Container.Release(result)); // release objektu na konci requestu, abychom předešli memory-leaks
				
				return result;
			}

			// Try the next provider if we don't have result
			if (result == null)
			{
				result = NextServiceProvider?.GetService(serviceType);
			}

			// Default activation
			if (result == null && (result = DefaultCreateInstance(serviceType)) != null)
			{
				// Cache it
				if (_typesCannotResolve.Count < TypesCannotResolveCacheCap)
				{
					_typesCannotResolve.TryAdd(serviceType, true);
				}
			}

			return result;
		}

		/// <summary>
		/// Stop of registration.
		/// </summary>
		public void Stop(bool immediate)
		{
			HostingEnvironment.UnregisterObject(this);

			Container.Dispose();
		}

		private object DefaultCreateInstance(Type type)
		{
			return Activator.CreateInstance(
						type,
						BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.CreateInstance,
						null,
						null,
						null);
		}
	}
}
