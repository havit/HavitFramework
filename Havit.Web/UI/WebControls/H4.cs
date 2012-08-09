using System;
using System.Web.UI;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Reprezentuje label, který se renderuje jako HTML tag H4.
	/// </summary>
	public class H4 : System.Web.UI.WebControls.Label
	{
		/// <summary>
		/// Vrací HtmlTextWriterTag.H4 zajišťující správné renderování.
		/// </summary>
		protected override HtmlTextWriterTag TagKey
		{
			get
			{
				return HtmlTextWriterTag.H4;
			}
		}
	}
}
