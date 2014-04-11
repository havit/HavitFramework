using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using Havit.Web.UI;

namespace Havit.Web.Bootstrap.UI.ClientScripts
{
	/// <summary>
	/// Helper for registering Bootstrap Client Scripts.
	/// </summary>
	public static class BootstrapClientScriptHelper
	{
		#region Consts (internal)
		/// <summary>
		/// Used for PreApplicationStartMethod attribute in StartUp.cs.
		/// To have hardcoded string with method name near method.
		/// </summary>
		internal const string RegisterBootstrapScriptResourceMappingMethodName = "RegisterScriptResourceMappings";

		/// <summary>
		/// Script Resource Definition name for WebUIValidationExtension.js.
		/// </summary>
		internal const string WebUIValidationExtensionScriptResourceMappingName = "Havit.Web.Bootstrap.WebUIValidationExtension";

		/// <summary>
		/// Script Resource Definition name for TabPanelExtension.js.
		/// </summary>
		internal const string TabPanelExtensionScriptResourceMappingName = "Havit.Web.Bootstrap.TabPanelExtension";
		#endregion

		#region RegisterScriptResourceMappings
		/// <summary>
		/// Register script map resource mapping for "bootstrap", "toastr" and other system names.
		/// Bootstrap script are ~/Scripts/bootstrap.min.js, for debug ~/Scripts/bootstrap.js.
		/// Toastr script are ~/Scripts/toastr.min.js, for debug ~/Scripts/toastr.js.
		/// Method called at application startup by PreApplicationStartMethod attribute in StartUp.cs.
		/// </summary>
		public static void RegisterScriptResourceMappings()
		{
			ScriptManager.ScriptResourceMapping.AddDefinition("bootstrap", new ScriptResourceDefinition { Path = "~/Scripts/bootstrap.min.js", DebugPath = "~/Scripts/bootstrap.js" });
			ScriptManager.ScriptResourceMapping.AddDefinition("toastr", new ScriptResourceDefinition { Path = "~/Scripts/toastr.min.js", DebugPath = "~/Scripts/toastr.js" });

			ScriptManager.ScriptResourceMapping.AddDefinition(WebUIValidationExtensionScriptResourceMappingName, new ScriptResourceDefinition { Path = "~/Scripts/havit.web.bootstrap/WebUIValidationExtension.js" });
			ScriptManager.ScriptResourceMapping.AddDefinition(TabPanelExtensionScriptResourceMappingName, new ScriptResourceDefinition { Path = "~/Scripts/havit.web.bootstrap/TabPanelExtension.js" });
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
