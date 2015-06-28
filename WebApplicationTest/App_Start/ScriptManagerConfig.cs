using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace Havit.WebApplicationTest
{
	public static class ScriptManagerConfig
	{
		#region RegisterScriptResourceMappings
		/// <summary>
		/// Registruje pojmenované scripty (ScriptResourceMappings).
		/// </summary>
		public static void RegisterScriptResourceMappings()
		{
			//ScriptManager.ScriptResourceMapping.AddDefinition("jquery", new ScriptResourceDefinition { Path = "//code.jquery.com/jquery-1.10.2.min.js" });
			//ScriptManager.ScriptResourceMapping.AddDefinition("toastr", new ScriptResourceDefinition { DebugPath = "~/scripts/toastr.js", Path = "~/scripts/toastr.min.js" });
		}
		#endregion
	}
}