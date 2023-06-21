using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using Castle.Core.Internal;
using Havit.BusinessLayerTest;
using Havit.Web.UI.WebControls;

namespace Havit.WebApplicationTest.HavitWebTests;

public partial class DropDownListTest : System.Web.UI.Page
{
	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);
		TestGridView.RowDataBound += new GridViewRowEventHandler(TestGridView_RowDataBound);
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);

		if (!Page.IsPostBack)
		{
			TestGridView.DataSource = new int[] { 1, 2, 3 };
			TestGridView.DataBind();
			SudeLicheDDL.Items.AsEnumerable().ToList().ForEach(item => item.SetOptionGroup(int.Parse(item.Value) % 2 == 0 ? "Sudé" : "Liché"));

			RoleDDL.DataSource = Role.GetAll();
			RoleDDL.DataBind();
		}
	}

	private void TestGridView_RowDataBound(object sender, GridViewRowEventArgs e)
	{
		EnterpriseDropDownList roleDDL = (EnterpriseDropDownList)e.Row.FindControl("RoleDDL");
		if (roleDDL != null)
		{
			roleDDL.DataSource = Role.GetAll().Where(item => item.ID > 0);
			roleDDL.DataBind();
		}
	}
}