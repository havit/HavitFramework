using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

namespace Havit.Web.Bootstrap.UI.WebControls
{
	/// <summary>
	/// Button rendered as &lt;button type=&amp;button&amp; ... &lt;/button&gt; instead of &lt;input type=&amp;button|submit&amp; ... /&gt;.
	/// Supports also child controls (ASP.NET standard button does not support child controls for Button). When child controls are used, Text property cannot be used.
	/// Gives Text property support of localization.
	/// </summary>
	[ParseChildren(false), PersistChildren(true)] // Enables child controls
	public class Button : System.Web.UI.WebControls.Button
	{
		#region IconCssClass
		/// <summary>
		/// Css class for icon.
		/// When set Button renders &lt;span class=&amp;IconCssClass&amp;&gt;&lt;span /&gt; at the beginning of button element content (before child controls).
		/// </summary>
		/// <example>&lt;bc:Button IconCssClass="glyphicon glyphicon-asterisk" runat="server" /&lt;</example>
		[DefaultValue("")]
		public string IconCssClass
		{
			get
			{
				return (string)(ViewState["IconCssClass"] ?? String.Empty);
			}
			set
			{
				ViewState["IconCssClass"] = value;
			}
		}
		#endregion

		#region TagKey
		/// <summary>
		/// Esures rendering as button element instead of input element.
		/// </summary>
		protected override HtmlTextWriterTag TagKey
		{
			get { return HtmlTextWriterTag.Button; }
		}
		#endregion

		#region UseSubmitBehavior
		/// <summary>
		/// Ensures rendering type="button" instead of type="submit".
		/// </summary>
		public override bool UseSubmitBehavior
		{
			get
			{
				return false;
			}
			set
			{
				if (value)
				{
					throw new ArgumentException(String.Format("Button ID '{0}': Setting UseSubmitBehavior to true is not supported.", this.ID));
				}
			}
		}
		#endregion

		#region OnPreRender
		/// <summary>
		/// Checks if there is not both Text property and child control used at one time.
		/// </summary>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			if (HasControls() && !String.IsNullOrEmpty(Text))
			{
				throw new HttpException(String.Format("Button with ID '{0}' contains child controls and has nonempty Text property thats not supported.'", this.ID));
			}
		}
		#endregion

		#region RenderContents
		/// <summary>
		/// Add support for child control rendering, text rendering and span with IconCssClass rendering.
		/// </summary>
		protected override void RenderContents(HtmlTextWriter writer)
		{
			// no base call - it does nothing (overrides WebControl's RenderContent which overrides RenderChildren call).
			//base.RenderContents(writer);

			// carefully - we are expecting rendering as button element because input element does not support rendering of child content
			// (input element is auto-closed and it cannot be changed easily).

			string text = HttpUtilityExt.GetResourceString(Text);

			bool renderIcon = !String.IsNullOrEmpty(IconCssClass);
			bool renderChildren = HasControls();
			bool renderText = !String.IsNullOrEmpty(text);

			if (renderIcon)
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Class, IconCssClass);
				writer.RenderBeginTag(HtmlTextWriterTag.Span);
				writer.RenderEndTag();
				if (renderChildren || renderText)
				{
					writer.Write((char)160);
				}
			}

			if (renderChildren)
			{
				RenderChildren(writer); // Enables child controls (with attribute ParseChildren and PersistChildren on this class)
			}

			if (renderText)
			{
				writer.WriteEncodedText(text);
			}
		}
		#endregion

	}
}
