using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

[assembly: WebResource("Havit.Web.UI.ClientScripts.HavitFrameworkClientScript.js", "text/javascript")]

namespace Havit.Web.UI.ClientScripts
{
	/// <summary>
	/// Helper for registering Havit Framework Extensions Client Scripts.
	/// </summary>
	internal static class HavitFrameworkClientScriptHelper
	{
		#region RegisterHavitFrameworkClientScript
		/// <summary>
		/// Register Havit Framework Extensions Client Script to the page. Uses ScriptResourceMapping.
		/// Ensures registration of "jquery".
		/// </summary>
		public static void RegisterHavitFrameworkClientScript(Page page)
		{
			ScriptManager.ScriptResourceMapping.EnsureScriptRegistration(page, "jquery");
			ScriptManager.ScriptResourceMapping.EnsureScriptRegistrationForEmbeddedResource(page, typeof(HavitFrameworkClientScriptHelper), "Havit.Web.UI.ClientScripts.HavitFrameworkClientScript.js");
		}
		#endregion
	}
}
