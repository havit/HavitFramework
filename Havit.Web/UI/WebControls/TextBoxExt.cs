using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Rozšíření TextBoxu.
	/// Pro IE&lt;10 vypíná autocomplete, pokud je zapnut AutoPostBack
	/// </summary>
	public class TextBoxExt : System.Web.UI.WebControls.TextBox
	{
		#region Render
		/// <summary>
		/// Zajistí přidání atributu autocomplete="off" pro IE lt;10.
		/// </summary>
		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			if ((this.Page.Request.Browser.Browser == "IE") && (this.Page.Request.Browser.MajorVersion < 10))
			{
				if (this.AutoPostBack)
				{
					this.Attributes["autocomplete"] = "off"; // hodnota se neuloží do ViewState, což je náš cíl
				}
			}

			// nyní provedeme skutečné vyrenderování controlu
			base.Render(writer);
		}
		#endregion
	}
}
