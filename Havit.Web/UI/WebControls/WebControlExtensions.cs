using System.Web.UI.WebControls;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// Extension metody ke třídě WebControl.
/// </summary>
internal static class WebControlExtensions
{
	/// <summary>
	/// Provede přidání stylů do WebControlu.
	/// Standardně (vestavěná metoda MergeStyle) použije ze stylu CssClass jen tehdy, pokud na WebControlu dosud není nastavena.
	/// Chování této metody toto mění a cssclass přidává vždy.
	/// </summary>
	public static void MergeStyleIncludingCssClass(this WebControl webControl, Style style)
	{
		if (!String.IsNullOrEmpty(style.CssClass))
		{
			if (String.IsNullOrEmpty(webControl.CssClass))
			{
				webControl.CssClass = style.CssClass;
			}
			else
			{
				webControl.CssClass += " " + style.CssClass;
			}
		}
		webControl.MergeStyle(style);
	}
}
