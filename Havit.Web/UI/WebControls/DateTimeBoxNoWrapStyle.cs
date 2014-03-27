using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Style pro použití DateTimeBoxu. Zajišťuje renderování "white-space: nowrap".
	/// </summary>
	internal class DateTimeBoxNoWrapStyle : Style
	{
		#region FillStyleAttributes
		/// <summary>
		/// Zajistí vyrenderování hodnoty "white-space: nowrap".
		/// </summary>
		protected override void FillStyleAttributes(CssStyleCollection attributes, IUrlResolutionService urlResolver)
		{
			base.FillStyleAttributes(attributes, urlResolver);
			
			attributes.Add(HtmlTextWriterStyle.WhiteSpace, "nowrap");
		}
		#endregion
	}
}
