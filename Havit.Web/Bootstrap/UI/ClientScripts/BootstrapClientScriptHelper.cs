using System.Web.UI;
using Havit.Web.UI;
using Havit.Web.UI.ClientScripts;

namespace Havit.Web.Bootstrap.UI.ClientScripts;

/// <summary>
/// Helper for registering Bootstrap Client Scripts.
/// </summary>
public static class BootstrapClientScriptHelper
{
	/// <summary>
	/// Script Resource Definition name for WebUIValidationExtension.js.
	/// </summary>
	internal const string WebUIValidationExtensionScriptResourceMappingName = "Havit.Web.Bootstrap.WebUIValidationExtension";

	/// <summary>
	/// Script Resource Definition name for TabPanelExtension.js.
	/// </summary>
	internal const string TabPanelExtensionScriptResourceMappingName = "Havit.Web.Bootstrap.TabPanelExtension";

	/// <summary>
	/// Script Resource Definition name for WebUIValidationExtension.js.
	/// </summary>
	internal const string ModalScriptResourceMappingName = "Havit.Web.Bootstrap.ModalExtension";

	/// <summary>
	/// Script Resource Definition name for CollabsiblePanel.js.
	/// </summary>
	internal const string CollapsiblePanelScriptResourceMappingName = "Havit.Web.Bootstrap.CollapsiblePanel";

	/// <summary>
	/// Register script map resource mapping for "bootstrap", "toastr" and other system names.
	/// Bootstrap script are ~/Scripts/bootstrap.min.js, for debug ~/Scripts/bootstrap.js.
	/// Toastr script are ~/Scripts/toastr.min.js, for debug ~/Scripts/toastr.js.
	/// Method called at application startup by PreApplicationStartMethod attribute in StartUp.cs.
	/// </summary>
	public static void RegisterScriptResourceMappings()
	{
		string version = HavitFrameworkClientScriptHelper.GetVersionString();

		ScriptManager.ScriptResourceMapping.AddDefinition("bootstrap", new ScriptResourceDefinition { Path = String.Format("~/Scripts/bootstrap.min.js?version={0}", version), DebugPath = String.Format("~/Scripts/bootstrap.js?version={0}", version) });
		ScriptManager.ScriptResourceMapping.AddDefinition("toastr", new ScriptResourceDefinition { Path = String.Format("~/Scripts/toastr.min.js?version={0}", version), DebugPath = String.Format("~/Scripts/toastr.js?version={0}", version) });

		ScriptManager.ScriptResourceMapping.AddDefinition(WebUIValidationExtensionScriptResourceMappingName, new ScriptResourceDefinition { Path = String.Format("~/Scripts/havit.web.bootstrap/WebUIValidationExtension.js?version={0}", version) });
		ScriptManager.ScriptResourceMapping.AddDefinition(TabPanelExtensionScriptResourceMappingName, new ScriptResourceDefinition { Path = String.Format("~/Scripts/havit.web.bootstrap/TabPanelExtension.js?version={0}", version) });
		ScriptManager.ScriptResourceMapping.AddDefinition(ModalScriptResourceMappingName, new ScriptResourceDefinition { Path = String.Format("~/Scripts/havit.web.bootstrap/ModalExtension.js?version={0}", version) });
		ScriptManager.ScriptResourceMapping.AddDefinition(CollapsiblePanelScriptResourceMappingName, new ScriptResourceDefinition { Path = String.Format("~/Scripts/havit.web.bootstrap/CollapsiblePanel.js?version={0}", version) });
	}

	/// <summary>
	/// Register Bootstrap Client Script to the page. Uses ScriptResourceMapping.
	/// Ensures registration of "jquery".
	/// </summary>
	public static void RegisterBootstrapClientScript(Page page)
	{
		ScriptManager.ScriptResourceMapping.EnsureScriptRegistration(page, "jquery");
		ScriptManager.ScriptResourceMapping.EnsureScriptRegistration(page, "bootstrap");
	}
}
