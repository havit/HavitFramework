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
		/// <summary>
		/// Returns an instance of the <see cref="T:System.Web.IHttpHandler" /> interface to process the requested resource.
		/// </summary>
		public override IHttpHandler GetHandler(HttpContext context, string requestType, string virtualPath, string path)
		{
			IHttpHandler handler = base.GetHandler(context, requestType, virtualPath, path);

			if (handler is Page)
			{
				SetUpDependencyInjections((Page)handler);
			}
			return handler;
		}

		internal void SetUpDependencyInjections(Page page)
		{
			DependencyInjectionWebFormsHelper.InitializeControlInstance(page);

			// Child controls are not created at this point.
			// They will be when PreInit fires.
			// Potřebuju se chytit na Preload jakožto první událost po načtení view-state. Procházení kolekce Controls před LoadViewState rozbije načtení viewstate u databindovaných controls ( http://forums.asp.net/t/1043999.aspx?GridView+losing+viewState+if+controls+collection+is+accessed+in+Page_Init+event )
			page.PreLoad += (s, e) =>
			{
				DependencyInjectionWebFormsHelper.InitializeChildControls(page);

				MasterPage master = page.Master;
				while (master != null)
				{
					DependencyInjectionWebFormsHelper.InitializeControlInstance(master);
					DependencyInjectionWebFormsHelper.InitializeChildControls(master);

					master = master.Master;
				}
			};
		}
	}
}