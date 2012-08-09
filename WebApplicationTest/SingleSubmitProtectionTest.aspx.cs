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

namespace WebApplicationTest
{
	public partial class SingleSubmitProtectionTest : System.Web.UI.Page
	{
		protected Button TestButton;
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			TestButton.Click += new EventHandler(TestButton_Click);
		}

		private void TestButton_Click(object sender, EventArgs e)
		{
			Response.Clear();
			Response.AddHeader("Content-Disposition", "attachment; filename=x.txt");
			Response.Write("test");
			Response.End();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			System.Threading.Thread.Sleep(1000);
		}
	}
}
