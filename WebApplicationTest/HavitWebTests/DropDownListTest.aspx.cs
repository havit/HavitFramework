using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using Havit.BusinessLayerTest;
using Havit.Web.UI.WebControls;

namespace Havit.WebApplicationTest.HavitWebTests
{
	public partial class DropDownListTest : System.Web.UI.Page
	{
		#region OnInit
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			TestGridView.RowDataBound += new GridViewRowEventHandler(TestGridView_RowDataBound);
		}
		#endregion

		#region OnLoad
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (!Page.IsPostBack)
			{
				TestGridView.DataSource = new int[] { 1, 2, 3 };
				TestGridView.DataBind();
			}
		}
		#endregion

		#region TestGridView_RowDataBound
		private void TestGridView_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			EnterpriseDropDownList roleDDL = (EnterpriseDropDownList)e.Row.FindControl("RoleDDL");
			if (roleDDL != null)
			{
				roleDDL.DataSource = Role.GetAll().Where(item => item.ID > 0);
				roleDDL.DataBind();
			}
		}
		#endregion

	}
}