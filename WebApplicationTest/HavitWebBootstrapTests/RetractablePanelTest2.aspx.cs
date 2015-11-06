using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.WebApplicationTest.HavitWebBootstrapTests
{
	public partial class RetractablePanelTestHelper : System.Web.UI.Page
	{
		protected void GotoBtn_Click(object sender, EventArgs e)
		{
			this.Response.Redirect("~/HavitWebBootstrapTests/RetractablePanelTest1.aspx");
		}

		protected void PostbackBtn_Click(object sender, EventArgs e)
		{

		}
	}
}