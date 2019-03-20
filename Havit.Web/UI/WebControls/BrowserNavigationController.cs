using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

using Havit.Web.UI.ClientScripts;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Control řešící provázání tlačítka zpět v prohlížeci s PageNavigatorem.
	/// </summary>
	/// <remarks>
	/// Implementace vychází z http://www.thecssninja.com/javascript/stealing-history-api
	/// </remarks>
	public class BrowserNavigationController : Control
	{
		/// <summary>
		/// Url použitá pro návrat, pokud není v PageNavigatoru, kam se vrátit.
		/// Pokud není hodnodnota nastavena, zůstane se na aktuální stránce.
		/// </summary>
		public string DefaultUrl
		{
			get
			{
				return (string)(ViewState["DefaultUrl"] ?? String.Empty);
			}
			set
			{
				ViewState["DefaultUrl"] = value;
			}
		}

		/// <summary>
		/// Zaregistruje klientský skript pro ovládání tlačítka zpět.
		/// </summary>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			HavitFrameworkClientScriptHelper.RegisterHavitFrameworkClientScript(this.Page);

			string backUrl = PageNavigator.Current.CanNavigateBack() ? PageNavigator.Current.GetNavigationBackUrl() : DefaultUrl;
			if (!String.IsNullOrEmpty(backUrl))
			{
				backUrl = Page.ResolveClientUrl(backUrl);
			}
			string script = String.Format("havitBrowserNavigationControllerExtension.startup('{0}');", backUrl.Replace("'", "\\'"));

			ScriptManager.RegisterStartupScript(this.Page, typeof(BrowserNavigationController), "StartUp", script, true);
		}
	}
}
