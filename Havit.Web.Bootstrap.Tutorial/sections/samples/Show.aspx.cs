using System;
using System.Collections.Generic;
using Havit.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Security;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.FriendlyUrls;

namespace Havit.Web.Bootstrap.Tutorial.Section.Samples
{
	public partial class ShowControlPage : System.Web.UI.Page
	{
		#region OnLoad
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!Page.IsPostBack)
			{
				string title = Request.QueryString["title"];
				string controlUrl = Request.QueryString["control"];

				// check url validity
				Contract.Assert<InvalidOperationException>(!String.IsNullOrEmpty(controlUrl), "Control parameter not specified.");
				Contract.Assert<InvalidOperationException>(!controlUrl.StartsWith(".") && !controlUrl.Contains("..") && !controlUrl.Contains("//") && !controlUrl.Contains("\\\\") && !controlUrl.Contains(":"), "Invalid Url.");

				controlUrl = "~/" + controlUrl + ".ascx";

				// load file and remove @Control header
				string path = Server.MapPath(controlUrl);
				string[] lines = File.ReadAllLines(path);
				//lines = lines.Where(line => !line.Contains("<%@")).ToArray();

				// display file content
				string content = String.Join(Environment.NewLine, lines).Trim();
				ContentLiteral.Text = HttpUtility.HtmlEncode(content);

				// display title
				TitleLiteral.Text = HttpUtility.HtmlEncode(title);
			}
		}
		#endregion
	}
}