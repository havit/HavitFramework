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
	internal static class BootstrapClientScriptHelper
	{
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
