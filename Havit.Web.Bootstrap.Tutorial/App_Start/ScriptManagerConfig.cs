using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace Havit.NewProjectTemplate.Web.App_Start
{
	public static class ScriptManagerConfig
	{
		#region RegisterScriptResourceMappings
		public static void RegisterScriptResourceMappings()
		{
			// není nutno registrovat jquery - zajišťuje AspNet.Scriptmanager.jquery
			// není nutno registrovat bootstrap - zajišťuje Havit.Web.Bootstrap
		}
		#endregion
	}
}