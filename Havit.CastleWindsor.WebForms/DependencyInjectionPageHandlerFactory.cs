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
		private ConcurrentDictionary<Type, PropertyInfo[]> cachedProperties = new ConcurrentDictionary<Type, PropertyInfo[]>();

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
			DependencyInjectionHandlerFactoryHelper.InitializeInstance(handler, cachedProperties);
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
					InitializeChildControls(page);

					MasterPage master = page.Master;
					while (master != null)
					{
						DependencyInjectionHandlerFactoryHelper.InitializeInstance(master, cachedProperties);
						InitializeChildControls(master);
						master.Unload += (sender, ea) =>
						{
							DependencyInjectionHandlerFactoryHelper.ReleaseDependencies(handler, cachedProperties);
						};

						master = master.Master;
					}
				};

				page.Unload += (s, e) =>
				{
					DependencyInjectionHandlerFactoryHelper.ReleaseDependencies(handler, cachedProperties);
				};
			}
		}

		/// <summary>
		/// Initializes the child controls.
		/// </summary>
		private void InitializeChildControls(Control control)
		{
			Control[] childControls = GetChildControls(control);

			foreach (Control childControl in childControls)
			{
				DependencyInjectionHandlerFactoryHelper.InitializeInstance(childControl, cachedProperties);
				InitializeChildControls(childControl);

				// also hook Unload for release of resolved components/dependencies
				Control childControlCopy = childControl; // dont use iteration variable in lambdas
				childControl.Unload += (s, e) =>
				{
					DependencyInjectionHandlerFactoryHelper.ReleaseDependencies(childControlCopy, cachedProperties);
				};
			}
		}

		/// <summary>
		/// Gets the child controls.
		/// </summary>
		private static Control[] GetChildControls(Control control)
		{
			BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;

			// UserControls jsou vždycky "moje", začínající Havit mají reprezentovat controly, které jsou ve WebBase. Do této podmínky spadnou i controly HFW, což je relativně zbytečné
			// možná optimalizace do budoucna je namísto "Havit." vzít jen ty, které NEJSOU z assembly Havit.Web ani System.Web. Pokud Havit.Web nezíská závislost na Castle Windsoru.

			return (
				from field in control.GetType().GetFields(flags)
				let type = field.FieldType
				where typeof(UserControl).IsAssignableFrom(type) || type.FullName.StartsWith("Havit.")
				let userControl = field.GetValue(control) as Control
				where userControl != null
				select userControl
			).ToArray();
		}
	}
}