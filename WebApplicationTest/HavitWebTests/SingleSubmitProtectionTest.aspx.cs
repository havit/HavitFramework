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
using System.Collections.Generic;

namespace Havit.WebApplicationTest.HavitWebTests
{
	public partial class SingleSubmitProtectionTest : System.Web.UI.Page
	{
		#region Static field (counter)
		private static int clickCounter = 0;
		#endregion

		#region Protected fields
		protected Button TestButton;
		#endregion

		#region OnInit
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			TestButton.Click += new EventHandler(TestButton_Click);
			Test2Button.Click += new EventHandler(TestButton_Click);
			Link1Button.Click += LinkButton_Click;
			Link2Button.Click += LinkButton_Click;
        }
		#endregion

		#region TestButton_Click
		private void TestButton_Click(object sender, EventArgs e)
		{
			clickCounter += 1;
		}
		#endregion

		#region LinkButton_Click
		private void LinkButton_Click(object sender, EventArgs e)
		{
			Response.Redirect("SingleSubmitProtectionTest2.aspx");
		}
		#endregion

		#region OnPreRender
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			System.Threading.Thread.Sleep(1000);
			TestButton.Text = "Clicks: " + clickCounter.ToString();
		}
		#endregion
	}
}
