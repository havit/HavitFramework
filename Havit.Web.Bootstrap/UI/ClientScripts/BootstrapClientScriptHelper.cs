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
		internal const string RegisterBootstrapScriptResourceMappingMethodName = "RegisterBootstrapScriptResourceMapping";
		#endregion

		#region RegisterBootstrapScriptResourceMapping
		/// <summary>
		/// Register script map resource mapping for "bootstrap" script name.
		/// Bootstrap script are ~/Scripts/bootstrap.min.js, for debug ~/Scripts/bootstrap.js.
		/// CDN is not supported due version in CDN URL (we want to be able to update bootstrap without this class change).
		/// Method called at application startup by PreApplicationStartMethod attribute in StartUp.cs.
		/// </summary>
		public static void RegisterBootstrapScriptResourceMapping()
		{
			ScriptResourceDefinition definition = new ScriptResourceDefinition
			{
				Path = "~/Scripts/bootstrap.min.js",
				DebugPath = "~/Scripts/bootstrap.js",
				//CdnPath = "http://ajax.aspnetcdn.com/ajax/bootstrap/3.1.1/bootstrap.min.js",
				//CdnDebugPath = "http://ajax.aspnetcdn.com/ajax/bootstrap/3.1.1/bootstrap.js",
				//CdnSupportsSecureConnection = true,
				LoadSuccessExpression = "window.jQuery.fn.carousel"
			};
			ScriptManager.ScriptResourceMapping.AddDefinition("bootstrap", definition);
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
