using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using Havit.NewProjectTemplate.Web.App_Start;

namespace Havit.NewProjectTemplate.Web
{
	public class Global : System.Web.HttpApplication
	{
		#region Application_Start
		private void Application_Start(object sender, EventArgs e)
		{
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

		#region IdentityMapScope management (Application_PreRequestHandlerExecute, Application_EndRequest)
		protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
		{
			this.Context.Items["IdentityMapScope"] = new Havit.Business.IdentityMapScope();
		}

		protected void Application_EndRequest(object sender, EventArgs e)
		{
			Havit.Business.IdentityMapScope identityMapScope = this.Context.Items["IdentityMapScope"] as Havit.Business.IdentityMapScope;
			if (identityMapScope != null)
			{
				identityMapScope.Dispose();
			}
		}
		#endregion
	}
}