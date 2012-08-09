using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI;

[assembly: WebResource("Havit.Web.UI.WebControls.DefaultStyles.css", "text/css")]

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Control, který vytvoří odkazy (link) na výchozí CSS styly HAVIT umístěné ve frameworkové assembly.
	/// </summary>
	/// <example>
	/// <code>
	/// &lt;head runat="server"&gt;
	///		&lt;havit:DefaultStylesLinker runat="server"&gt;
	/// &lt;/head&gt;
	/// </code>
	/// </example>
	public class DefaultStylesLinker : CompositeControl
	{
		/// <summary>
		/// Vytvoří child-controly.
		/// </summary>
		protected override void CreateChildControls()
		{
			HtmlLink defaultStylesHtmlLink = new HtmlLink();
			defaultStylesHtmlLink.Href = Page.ClientScript.GetWebResourceUrl(typeof(DefaultStylesLinker), "Havit.Web.UI.WebControls.DefaultStyles.css");
			defaultStylesHtmlLink.Attributes.Add("type", "text/css");
			defaultStylesHtmlLink.Attributes.Add("rel", "stylesheet");

			this.Controls.Add(defaultStylesHtmlLink);
		}
	}
}
