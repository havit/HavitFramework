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
	/// <summary>
	/// Control, který zamezuje dvojímu odeslání formuláře.
	/// Při submitu překryje viditelnou část formuláře průhledným, ale neprokliknutelným, DIVem.
	/// </summary>
	public class SingleSubmitProtection : WebControl
	{
		/// <summary>
		/// Volání JavaScriptové funkce, která zablokuje SetProcessing na SingleSubmitPage.
		/// Tato konstanta se může vložit např. do Button.OnClientClick.
		/// </summary>
		public const string SetProcessingDisableJavaScript = "SingleSubmit_SetProcessing_Disable();";

		/// <summary>
		/// Script pro zavolání clientside metody zajišťující blokování klientských operací ve stránce.
		/// </summary>
		public const string SetProcessingJavaScript = "SingleSubmit_SetProcessing();";

		/// <summary>
		/// Script pro zavolání clientside metody pro ukončení blokování klientských operací ve stránce.
		/// </summary>
		public const string ClearProcessingJavaScript = "SingleSubmit_ClearProcessing();";

		/// <summary>
		/// Prvek je založen na elementu DIV.
		/// </summary>
		public SingleSubmitProtection()
			: base(HtmlTextWriterTag.Div)
		{
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data. </param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			ScriptManager currentScriptManager = ScriptManager.GetCurrent(this.Page);

			// Styly i javascript registrujeme i v případě, kdy Enabled == false. To z toho důvodu, protože ve stránkách mohou být použity
			// klienské skripty, které vyžadují přítomnost knihovny. Například SetProcessingDisableJavaScript.
			// Pokud by nebyl javascript zaregistrován, docházelo by k pádu takového klienského volání.

			SingleSubmitProtection.RegisterStylesheets(this.Page);

			SingleSubmitProtection.RegisterScript(this.Page);

			if (this.Enabled)
			{
				// zaregistruje javascript pro OnSubmit
				// javascript se neregistruje pro async postback, protože by se skript jednotlivými callbacky přidával a přidával
				if ((currentScriptManager == null) || (!currentScriptManager.IsInAsyncPostBack))
				{
					ScriptManager.RegisterOnSubmitStatement(
						this.Page,
						typeof(SingleSubmitProtection),
						"HidePage",
						"if (!_SingleSubmit_IsRecursive) return SingleSubmit_OnSubmit();\n\n");
				}

				// zajištění mizení progress panelu po async postbacku
				if ((currentScriptManager != null) && currentScriptManager.EnablePartialRendering && currentScriptManager.IsInAsyncPostBack)
				{
					ScriptManager.RegisterStartupScript(
						this.Page,
						typeof(SingleSubmitProtection),
						"SingleSubmit_Startup",
						ClearProcessingJavaScript,
						true);
				}
			}
		}

		/// <summary>
		/// Zaregistruje script.
		/// </summary>
		public static void RegisterScript(Page page)
		{
			bool registered = (bool)(HttpContext.Current.Items["Havit.Web.UI.WebControls.SingleSubmitProtection.RegisterScript_registered"] ?? false);

			if (!registered)
			{
				// Registruje klientské skripty pro zamezení opakovaného odeslání stránky.
				ScriptManager.RegisterClientScriptResource(
					page,
					typeof(SingleSubmitProtection),
					"Havit.Web.UI.WebControls.SingleSubmitProtection.js");
				HttpContext.Current.Items["Havit.Web.UI.WebControls.SingleSubmitProtection.RegisterScript_registered"] = true;
			}
		}

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
	}
}
