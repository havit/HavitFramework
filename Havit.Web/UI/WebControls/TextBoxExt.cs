using System.Web.UI;
using System.Web.UI.WebControls;

using Havit.Web.UI.Scriptlets;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// Rozšíření TextBoxu.
/// Pro IE&lt;10 vypíná autocomplete, pokud je zapnut AutoPostBack
/// </summary>
public class TextBoxExt : System.Web.UI.WebControls.TextBox
{
	/// <summary>
	/// Adds HTML attributes and styles that need to be rendered to the specified HtmlTextWriterTag.
	/// </summary>
	protected override void AddAttributesToRender(HtmlTextWriter writer)
	{
		// MultiLine TextBox nerenderuje MaxLength, ačkoliv dle HTML5 jde o platný atribut k textarea (a od IE10 i funguje).
		// Pro zajištění zpětné kompatibility (MS může doplnit renderování) se snažíme zajistit, abychom jej nevyrenderovali podruhé.

		int multilineMaxLength = 0;

		if ((this.TextMode == TextBoxMode.MultiLine) && (this.MaxLength > 0))
		{
			multilineMaxLength = this.MaxLength;
			this.MaxLength = 0; // jsme v renderu, hodnota není ve ViewState, což je náš cíl
			writer.AddAttribute("maxlength", multilineMaxLength.ToString());
		}

		base.AddAttributesToRender(writer);
	}

	/// <summary>
	/// Zajistí přidání atributu autocomplete="off" pro IE lt;10.
	/// </summary>
	protected override void Render(System.Web.UI.HtmlTextWriter writer)
	{
		if (BrowserHelper.IsInternetExplorer && (this.Page.Request.Browser.MajorVersion < 10))
		{
			if (this.AutoPostBack)
			{
				this.Attributes["autocomplete"] = "off"; // jsme v renderu, hodnota se neuloží do ViewState, což je náš cíl
			}
		}

		// nyní provedeme skutečné vyrenderování controlu
		base.Render(writer);
	}
}
