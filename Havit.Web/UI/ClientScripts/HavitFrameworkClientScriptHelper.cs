using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
		#region Consts
		/// <summary>
		/// Script Resource Definition name for HavitFrameworkClientScript.js.
		/// </summary>
		private const string HavitFrameworkClientScriptResourceMappingName = "Havit.Web.ClientContent.HavitFrameworkClientScript";

		/// <summary>
		/// Script Resource Definition name for jquery.multipleselect.js.
		/// </summary>
		internal const string JQueryMultipleSelectResourceMappingName = "Havit.Web.ClientContent.JQueryMultipleSelect";

		/// <summary>
		/// JQueryAutoComplete script.
		/// </summary>
		internal const string JQueryAutoCompleteResourceMappingName = "Havit.Web.ClientContent.JQueryAutoComplete";
		#endregion

		#region RegisterScriptResourceMappings
		/// <summary>
		/// Register script map resource mapping for framework script.
		/// Method called at application startup.
		/// </summary>
		public static void RegisterScriptResourceMappings()
		{
			string version = GetVersionString();
			ScriptManager.ScriptResourceMapping.AddDefinition(HavitFrameworkClientScriptResourceMappingName, new ScriptResourceDefinition { Path = String.Format("~/Scripts/havit.web.clientcontent/HavitFrameworkClientScript.js?version={0}", version) });
			ScriptManager.ScriptResourceMapping.AddDefinition(JQueryMultipleSelectResourceMappingName, new ScriptResourceDefinition { Path = String.Format("~/Scripts/havit.web.clientcontent/jquery.multiple.select.js?version={0}", version) });
			ScriptManager.ScriptResourceMapping.AddDefinition(JQueryAutoCompleteResourceMappingName, new ScriptResourceDefinition { Path = String.Format("~/Scripts/havit.web.clientcontent/jquery.autocomplete.min.js?version={0}", version) });
		}
		#endregion
		
		#region RegisterHavitFrameworkClientScript
		/// <summary>
		/// Register Havit Framework Extensions Client Script to the page. Uses ScriptResourceMapping.
		/// Ensures registration of "jquery".
		/// </summary>
		public static void RegisterHavitFrameworkClientScript(Page page)
		{
			ScriptManager.ScriptResourceMapping.EnsureScriptRegistration(page, "jquery");
			ScriptManager.ScriptResourceMapping.EnsureScriptRegistration(page, HavitFrameworkClientScriptResourceMappingName);
		}
		#endregion

		#region GetVersionString
		/// <summary>
		/// Vrací verzi z assembly pro přidání do URL.
		/// Vzhledem k tomu, že skripty se do projetu dostávají jinými balíčky, než Havit.Web, není toto rozhodně dokonalé.
		/// Předpokládáme zatím, že budou balíčky aktualizovány současně. Jinak bychom museli vytvořit další assembly
		/// (Havit.Web.Bootstrap a Havit.Web.ClientContent) a registrovat scripty až v těchto assembly (bylo by rozhodně správnější).
		/// </summary>
		internal static string GetVersionString()
		{
			if (_version == null)
			{
				lock (_versionLock)
				{
					if (_version == null)
					{
						Assembly assembly = Assembly.GetAssembly(typeof(HavitFrameworkClientScriptHelper));
						string fileVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
						_version = fileVersion.Replace(".", "_");
					}
				}
			}
			return _version;
		}
		private static string _version;
		private static readonly object _versionLock = new object();
		#endregion
	}
}
