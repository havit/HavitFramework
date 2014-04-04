using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace Havit.Web.Bootstrap.Tutorial
{
	public class Global : System.Web.HttpApplication
	{
		#region Application_Start
		private void Application_Start(object sender, EventArgs e)
		{
			RouteConfig.RegisterRoutes(RouteTable.Routes); // configures ASP.NET Friendly Urls
			ScriptManagerConfig.RegisterScriptResourceMappings();
		}
		#endregion

		#region Application_Error
		private void Application_Error(object sender, EventArgs e)
		{
			Exception exception = Server.GetLastError();
			if (exception != null)
			{
				Havit.Web.Management.WebRequestErrorEventExt customEvent = new Havit.Web.Management.WebRequestErrorEventExt(exception.Message, this, exception, HttpContext.Current);
				customEvent.Raise();
			}
		}
		#endregion

	}
}