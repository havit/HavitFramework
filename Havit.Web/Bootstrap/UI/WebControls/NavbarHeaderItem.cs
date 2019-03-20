using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Havit.Web.Bootstrap.UI.WebControls
{
	/// <summary>
	/// Navbar item - Header item.
	/// </summary>
	public class NavbarHeaderItem : NavbarItem
	{
		/// <summary>
		/// Header item text.
		/// Supports resources pattern.
		/// </summary>
		public string Text
		{
			get
			{
				return (string)(ViewState["Text"] ?? String.Empty);
			}
			set
			{
				ViewState["Text"] = value;
			}
		}

		/// <summary>
		/// Returns trie - header item is decoration
		/// </summary>
		public override bool IsDecoration
		{
			get { return true; }
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public NavbarHeaderItem()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public NavbarHeaderItem(string text) : this()
		{
			this.Text = text;
		}

		/// <summary>
		/// Renders header item.
		/// </summary>
		public override void Render(HtmlTextWriter writer, Control container, bool showCaret, int nestingLevel)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Class, "dropdown-header");
			writer.RenderBeginTag(HtmlTextWriterTag.Li);
			writer.WriteEncodedText(HttpUtilityExt.GetResourceString(Text));
			writer.RenderEndTag(); // Li
		}
	}
}
