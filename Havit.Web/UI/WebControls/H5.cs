using System;
using System.Web.UI;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Reprezentuje label, který se renderuje jako HTML tag H5.
	/// </summary>
	public class H5 : System.Web.UI.WebControls.Label
	{
		/// <summary>
		/// Vrací HtmlTextWriterTag.H5 zajišťující správné renderování.
		/// </summary>
		protected override HtmlTextWriterTag TagKey
		{
			get
			{
				return HtmlTextWriterTag.H5;
			}
		}
	}
}
