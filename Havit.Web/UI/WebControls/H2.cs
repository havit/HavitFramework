using System;
using System.Web.UI;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// Reprezentuje label, který se renderuje jako HTML tag H2.
/// </summary>
public class H2 : System.Web.UI.WebControls.Label
{
	/// <summary>
	/// Vrací HtmlTextWriterTag.H2 zajišťující správné renderování.
	/// </summary>
	protected override HtmlTextWriterTag TagKey
	{
		get
		{
			return HtmlTextWriterTag.H2;
		}
	}
}
