using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Threading;
using System.Web.UI.HtmlControls;
using System.Web;

[assembly: WebResource("Havit.Web.UI.WebControls.SingleSubmitProtection.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.SingleSubmitProtection.css", "text/css")]

namespace Havit.Web.UI.WebControls
{
	public class SingleSubmitProtection : WebControl
	{
		#region Constructor
		/// <summary>
		/// Prvek je založen na elementu DIV.
		/// </summary>
		public SingleSubmitProtection()
			: base(HtmlTextWriterTag.Div)
		{
		}
		#endregion

		/// <summary>
		/// Volání JavaScriptové funkce, která zablokuje SetProcessing na SingleSubmitPage.
		/// Tato konstanta se mùže vložit napø. do Button.OnClientClick.
		/// </summary>
		public const string SetProcessingDisableJavaScript = "SingleSubmit_SetProcessing_Disable();";

		#region OnPreRender
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

            if (this.Enabled)
            {
                SingleSubmitProtection.RegisterStylesheets(this.Page);

                // Registruje klientské skripty pro zamezení opakovaného odeslání stránky.
                ScriptManager.RegisterClientScriptResource(
                    this.Page,
                    typeof(SingleSubmitProtection),
                    "Havit.Web.UI.WebControls.SingleSubmitProtection.js");

                // zaregistruje javascript pro OnSubmit 
                ScriptManager.RegisterOnSubmitStatement(
                    this.Page,
                    typeof(SingleSubmitProtection),
                    "HidePage",
                    "if (!_SingleSubmit_IsRecursive) return SingleSubmit_OnSubmit();\n\n");

                // zajištìní mizení progress panelu po async postbacku
                ScriptManager currentScriptManager = ScriptManager.GetCurrent(this.Page);
                if ((currentScriptManager != null) && (currentScriptManager.EnablePartialRendering))
                {
                    ScriptManager.RegisterStartupScript(
                        this.Page,
                        typeof(SingleSubmitProtection),
                        "SingleSubmit_Startup",
                        "SingleSubmit_Startup();",
                        true);
                }
            }
		}
		#endregion		

        #region RegisterStylesheets (static)
        /// <summary>
        /// Zaregistruje css.        
        /// </summary>
        public static void RegisterStylesheets(Page page)
        {
            if (page.Header != null)
            {
                bool registered = (bool)(HttpContext.Current.Items["Havit.Web.UI.WebControls.SingleSubmitProtection.RegisterStylesheets_registered"] ?? false);

                if (!registered)
                {
                    HtmlLink htmlLink = new HtmlLink();
                    string resourceName = "Havit.Web.UI.WebControls.SingleSubmitProtection.css";
                    htmlLink.Href = page.ClientScript.GetWebResourceUrl(typeof(SingleSubmitProtection), resourceName);
                    htmlLink.Attributes.Add("rel", "stylesheet");
                    htmlLink.Attributes.Add("type", "text/css");
                    page.Header.Controls.Add(htmlLink);
                    HttpContext.Current.Items["Havit.Web.UI.WebControls.SingleSubmitProtection.RegisterStylesheets_registered"] = true;
                }
            }
        }
        #endregion

	}
}
