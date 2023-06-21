using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// Style pro použití DateTimeBoxu. Zajišťuje renderování "white-space: nowrap".
/// </summary>
internal class DateTimeBoxStyle : Style
{
	/// <summary>
	/// Indikuje, zda má metoda FillStyleAttributes renderovat style "white-space: nowrap;".
	/// </summary>
	public bool UseWhiteSpaceNoWrap { get; set; }

	/// <summary>
	/// Zajistí vyrenderování hodnoty "white-space: nowrap".
	/// </summary>
	protected override void FillStyleAttributes(CssStyleCollection attributes, IUrlResolutionService urlResolver)
	{
		base.FillStyleAttributes(attributes, urlResolver);

		if (UseWhiteSpaceNoWrap)
		{
			attributes.Add(HtmlTextWriterStyle.WhiteSpace, "nowrap");
		}
	}
}
