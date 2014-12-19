using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Havit.Web.Bootstrap.UI.WebControls
{
	/// <summary>
	/// Navbar item - separator.
	/// </summary>
	public sealed class NavbarSeparatorItem : NavbarItem
	{
		#region IsDecoration
		/// <summary>
		/// Navbar separator is a decoration.
		/// </summary>
		public override bool IsDecoration
		{
			get { return true; }
		}
		#endregion

		#region Render
		/// <summary>
		/// Renders navbar separator.
		/// </summary>
		public override void Render(HtmlTextWriter writer, Control container, bool showCaret, int nestingLevel)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "divider");
			writer.RenderBeginTag(HtmlTextWriterTag.Li);
			writer.RenderEndTag();
		}
		#endregion
	}
}
