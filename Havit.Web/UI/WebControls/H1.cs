using System.Web.UI;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// Reprezentuje label, který se renderuje jako HTML tag H1.
/// </summary>
public class H1 : System.Web.UI.WebControls.Label
{
	/// <summary>
	/// Vrací HtmlTextWriterTag.H1 zajišťující správné renderování.
	/// </summary>
	protected override HtmlTextWriterTag TagKey
	{
		get
		{
			return HtmlTextWriterTag.H1;
		}
	}
}
