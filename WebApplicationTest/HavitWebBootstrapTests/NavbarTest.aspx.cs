using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Havit.Web.Bootstrap.UI.WebControls;

namespace Havit.WebApplicationTest.HavitWebBootstrapTests
{
	public partial class NavbarTest : System.Web.UI.Page
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			FirstNavbar.DataBind();
		}
	}
}