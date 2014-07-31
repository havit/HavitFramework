﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using Havit.Linq;

namespace Havit.Web.Bootstrap.UI.WebControls
{
	/// <summary>
	/// Navbar item - text item.
	/// </summary>
	public class NavbarTextItem : NavbarItem
	{
		#region Text
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
		#endregion

		#region IsDecoration
		/// <summary>
		/// Returns false - text item is not a decoration.
		/// </summary>
		public override bool IsDecoration
		{
			get { return false; }
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		public NavbarTextItem()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public NavbarTextItem(string text)
		{
			this.Text = text;
		}
		#endregion

		#region Render
		/// <summary>
		/// Renders menu items.
		/// </summary>
		public override void Render(HtmlTextWriter writer, bool showCaret, int nestingLevel)
		{
			string cssClass = "";

			if (!String.IsNullOrEmpty(cssClass))
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass.Trim());
			}
			writer.RenderBeginTag(HtmlTextWriterTag.Li);
			writer.RenderBeginTag(HtmlTextWriterTag.Span);

			writer.WriteEncodedText(HttpUtilityExt.GetResourceString(Text));

			writer.RenderEndTag(); // Span
			writer.RenderEndTag(); // Li
		}
		#endregion
	}
}
