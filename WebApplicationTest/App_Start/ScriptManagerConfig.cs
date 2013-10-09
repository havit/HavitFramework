using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace WebApplicationTest
{
	public static class ScriptManagerConfig
	{
		#region RegisterScriptResourceMappings
		/// <summary>
		/// Registruje pojmenované scripty (ScriptResourceMappings).
		/// </summary>
		public static void RegisterScriptResourceMappings()
		{
			ScriptManager.ScriptResourceMapping.AddDefinition("jquery", new ScriptResourceDefinition { Path = "~/scripts/jquery-2.0.3.js" });
			ScriptManager.ScriptResourceMapping.AddDefinition("toastr", new ScriptResourceDefinition { Path = "~/scripts/toastr.js" });
		}
		#endregion
	}
}