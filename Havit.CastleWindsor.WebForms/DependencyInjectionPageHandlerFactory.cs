using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;

namespace Havit.CastleWindsor.WebForms
{
	/// <summary>
	/// Page factory, která dokáže injektovat dependence přes Windsor
	/// </summary>
	public class DependencyInjectionPageHandlerFactory : PageHandlerFactory
	{
		private readonly ConcurrentDictionary<Type, PropertyInfo[]> cachedProperties = new ConcurrentDictionary<Type, PropertyInfo[]>();

		/// <summary>
		/// Returns an instance of the <see cref="T:System.Web.IHttpHandler" /> interface to process the requested resource.
		/// </summary>
		public override IHttpHandler GetHandler(HttpContext context, string requestType, string virtualPath, string path)
		{
			IHttpHandler handler = base.GetHandler(context, requestType, virtualPath, path);

			if (handler != null)
			{
				SetUpDependencyInjections(handler);
			}

			return handler;
		}

		internal void SetUpDependencyInjections(IHttpHandler handler)
		{
			DependencyInjectionWebFormsHelper.InitializeInstance(handler, cachedProperties);
			HookChildControlInitialization(handler);
		}

		/// <summary>
		/// Hooks the child control initialization.
		/// </summary>
		private void HookChildControlInitialization(IHttpHandler handler)
		{
			Page page = handler as Page;

			if (page != null)
			{
				// Child controls are not created at this point.
				// They will be when PreInit fires.
				page.Init += (s, e) =>
				{
					DependencyInjectionWebFormsHelper.InitializeChildControls(page, this.cachedProperties);

					MasterPage master = page.Master;
					while (master != null)
					{
						DependencyInjectionWebFormsHelper.InitializeInstance(master, cachedProperties);
						DependencyInjectionWebFormsHelper.InitializeChildControls(master, this.cachedProperties);
						MasterPage currentMasterPage = master;
						master.Unload += (sender, ea) =>
						{
							DependencyInjectionWebFormsHelper.ReleaseDependencies(currentMasterPage, cachedProperties);
						};

						master = master.Master;
					}
				};

				page.Unload += (s, e) =>
				{
					DependencyInjectionWebFormsHelper.ReleaseDependencies(handler, cachedProperties);
				};
			}
		}
	}
}