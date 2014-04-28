using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.FriendlyUrls;
using System.Text;
using Havit.Diagnostics.Contracts;
using System.IO;

namespace Havit.Web.Bootstrap.Tutorial.Section.Samples
{
	public class ShowControl : LiteralControl
	{
		#region Title
		/// <summary>
		/// ...
		/// </summary>
		public string Title
		{
			get
			{
				return (string)(ViewState["Title"] ?? String.Empty);
			}
			set
			{
				ViewState["Title"] = value;
			}
		}
		#endregion

		#region ShowControlID
		public string ShowControlID
		{
			get
			{
				return (string)(ViewState["ShowControlID"] ?? String.Empty);
			}
			set
			{
				ViewState["ShowControlID"] = value;
			}
		}
		#endregion

		#region Constructor
		public ShowControl()
		{
		}
		#endregion

		#region OnPreRender
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			UserControl uc = (UserControl)NamingContainer.FindControl(ShowControlID);
			StringBuilder html = new StringBuilder();

			//unique id of the control
			var controlID = Guid.NewGuid().ToString();
			//unique id of the collapse block
			var collapseID = Guid.NewGuid().ToString();

			html.Append("<div class=\"panel-group\" id=\"accordion" + controlID + "\">");
			html.Append("<div class=\"panel panel-default\">");
			html.Append("<div class=\"panel-heading\">");
			html.Append("<h4 class=\"panel-title\">");
			html.Append("<a data-toggle=\"collapse\" data-parent=\"#accordion\" href=\"#collapse" + collapseID + "\">");

			html.Append("Show code - " + Title);
			html.Append("</a></h4></div>");
			html.Append("<div id=\"collapse" + collapseID + "\" class=\"panel-collapse collapse\">");
			html.Append("<div class=\"panel-body\">");

			html.Append("<pre><code>" + RenderControlToString(uc.AppRelativeVirtualPath) + "</code></pre>");
			html.Append("</div></div></div>");
			html.Append("</div>");
			Text = html.ToString();

		}
		#endregion

		#region RenderControlToString
		private string RenderControlToString(string controlUrl)
		{
			// check url validity
			Contract.Assert<InvalidOperationException>(!String.IsNullOrEmpty(controlUrl), "Control parameter not specified.");
			Contract.Assert<InvalidOperationException>(!controlUrl.StartsWith(".") && !controlUrl.Contains("..") && !controlUrl.Contains("//") && !controlUrl.Contains("\\\\") && !controlUrl.Contains(":"), "Invalid Url.");

			// load file and remove @Control header
			string path = HttpContext.Current.Server.MapPath(controlUrl);
			string[] lines = File.ReadAllLines(path);
			//lines = lines.Where(line => !line.Contains("<%@")).ToArray();

			// display file content
			string content = String.Join(Environment.NewLine, lines).Trim();
			return HttpUtility.HtmlEncode(content);
		}
		#endregion
	}
}