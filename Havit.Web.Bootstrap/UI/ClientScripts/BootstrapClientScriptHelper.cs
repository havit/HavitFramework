using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using Havit.Web.UI;

[assembly: WebResource("Havit.Web.UI.ClientScripts.HavitFrameworkClientScript.js", "text/javascript")]

namespace Havit.Web.Bootstrap.UI.ClientScripts
{
	/// <summary>
	/// Helper for registering Bootstrap Client Scripts.
	/// </summary>
	public static class BootstrapClientScriptHelper
	{
		#region RegisterBootstrapScriptResourceMappingMethodName
		/// <summary>
		/// Used for PreApplicationStartMethod attribute in StartUp.cs.
		/// To have hardcoded string with method name near method.
		/// </summary>
		internal const string RegisterBootstrapScriptResourceMappingMethodName = "RegisterScriptResourceMappings";
		#endregion

		#region RegisterScriptResourceMappings
		/// <summary>
		/// Register script map resource mapping for "bootstrap" and "toastr" script name.
		/// Bootstrap script are ~/Scripts/bootstrap.min.js, for debug ~/Scripts/bootstrap.js.
		/// CDN is not supported due version in CDN URL (we want to be able to update bootstrap without this class change).
		/// Method called at application startup by PreApplicationStartMethod attribute in StartUp.cs.
		/// Toastr script are ~/Scripts/toastr.min.js, for debug ~/Scripts/toastr.js.
		/// </summary>
		public static void RegisterScriptResourceMappings()
		{
			ScriptResourceDefinition bootstrapDefinition = new ScriptResourceDefinition { Path = "~/Scripts/bootstrap.min.js", DebugPath = "~/Scripts/bootstrap.js" };
			ScriptResourceDefinition toastrDefinition = new ScriptResourceDefinition { Path = "~/Scripts/toastr.min.js", DebugPath = "~/Scripts/toastr.js" };

			ScriptManager.ScriptResourceMapping.AddDefinition("bootstrap", bootstrapDefinition);
			ScriptManager.ScriptResourceMapping.AddDefinition("toastr", toastrDefinition);
		}
		#endregion

		#region RegisterBootstrapClientScript
		/// <summary>
		/// Register Bootstrap Client Script to the page. Uses ScriptResourceMapping.
		/// Ensures registration of "jquery".
		/// </summary>
		public static void RegisterBootstrapClientScript(Page page)
		{
			ScriptManager.ScriptResourceMapping.EnsureScriptRegistration(page, "jquery");
			ScriptManager.ScriptResourceMapping.EnsureScriptRegistration(page, "bootstrap");
		}
		#endregion
	}
}
