using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace WebApplicationTest.HavitWebTests
{
	public partial class ScriptletTest4 : System.Web.UI.Page
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			ShowButton.Click += ShowButton_Click;
			HideButton.Click += HideButton_Click;
		}

		private void ShowButton_Click(object sender, EventArgs e)
		{
			HideButton.Visible = true;
			ShowButton.Visible = false;
			MyPanel.Visible = true;
		}

		private void HideButton_Click(object sender, EventArgs e)
		{
			HideButton.Visible = false;
			ShowButton.Visible = true;
			MyPanel.Visible = false;
		}

	}
}
