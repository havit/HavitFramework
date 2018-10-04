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
		/// Title.
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

		#region Filename
		public string Filename
		{
			get
			{
				return (string)ViewState["Filename"];
			}
			set
			{
				ViewState["Filename"] = value;
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

		#region OnPreRender
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			string filename;
			if (!String.IsNullOrEmpty(ShowControlID))
			{
				UserControl uc = (UserControl)NamingContainer.FindControl(ShowControlID);
				filename = uc.AppRelativeVirtualPath;
			}
			else
			{
				filename = Filename;
			}

			//unique id of the control
			var controlID = Guid.NewGuid().ToString();
			//unique id of the collapse block
			var collapseID = Guid.NewGuid().ToString();

			StringBuilder html = new StringBuilder();
			html.Append("<div class=\"panel-group\" id=\"accordion" + controlID + "\">");
			html.Append("<div class=\"panel panel-default\">");
			html.Append("<div class=\"panel-heading\">");
			html.Append("<h4 class=\"panel-title\">");
			html.Append("<a data-toggle=\"collapse\" data-parent=\"#accordion\" href=\"#collapse" + collapseID + "\">");

			if (String.IsNullOrEmpty(Title))
			{
				html.Append("Show code");
			}
			else
			{
				html.Append(Title);				
			}
			html.Append("</a></h4></div>");
			html.Append("<div id=\"collapse" + collapseID + "\" class=\"panel-collapse collapse\">");
			html.Append("<div class=\"panel-body\">");

			html.Append("<pre><code>" + RenderFileToString(filename) + "</code></pre>");
			html.Append("</div></div></div>");
			html.Append("</div>");
			Text = html.ToString();

		}
		#endregion

		#region RenderFileToString
		private string RenderFileToString(string filename)
		{
			// check url validity
			Contract.Assert<InvalidOperationException>(!String.IsNullOrEmpty(filename), "Control parameter not specified.");
			Contract.Assert<InvalidOperationException>(!filename.StartsWith(".") && !filename.Contains("..") && !filename.Contains("//") && !filename.Contains("\\\\") && !filename.Contains(":"), "Invalid Url.");

			string[] lines = File.ReadAllLines(HttpContext.Current.Server.MapPath(filename));

			// display file content
			string content = String.Join(Environment.NewLine, lines).Trim();
			return HttpUtility.HtmlEncode(content);
		}
		#endregion
	}
}