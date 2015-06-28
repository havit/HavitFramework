using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.WebApplicationTest.HavitWebBootstrapTests
{
	public partial class TabsTest : System.Web.UI.Page
	{
		#region OnLoad
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (!Page.IsPostBack)
			{
				TabContainersRepeater.DataSource = new int[] { 1, 2, 3 };
				TabContainersRepeater.DataBind();
			}
		}
		#endregion
	}
}