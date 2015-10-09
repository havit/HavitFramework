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
				DependencyInjectionWebFormsHelper.InitializePage((Page)handler);
			}
			return handler;
		}
	}
}