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

			// We must register dynamically compiled resources (pages, controls, master pages, handlers ...)
			if ((typeof(UserControl).IsAssignableFrom(serviceType) ||    // User controls (.ascx) and event Master Pages (.master) inherit from UserControl
				typeof(IHttpHandler).IsAssignableFrom(serviceType)) &&   // Geneirc handlers (.ashx) and also pages (.aspx) inherit from IHttpHandler
				!Container.Kernel.HasComponent(serviceType))
			{
				// Lifestyle is *PerWebRequest*, because Castle Windsor can then release this component at the end of request and also child dependencies are release
				Container.Register(Component.For(serviceType).ImplementedBy(serviceType).LifestylePerWebRequest());
			}

			// If we have component registered, we will resolve the service
			if (Container.Kernel.HasComponent(serviceType))
			{
				result = Container.Resolve(serviceType);
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
