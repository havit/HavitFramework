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
using Havit.BusinessLayerTest;
using Havit.Web.UI.WebControls;
using Havit.Web.UI;

namespace WebApplicationTest
{
	public partial class GridViewExtTest : System.Web.UI.Page
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			TestGV1.DataBinding += new EventHandler(TestGV_DataBinding);
			TestGV1.RowCustomizingCommandButton += new GridViewRowCustomizingCommandButtonEventHandler(TestGV_RowCustomizingCommandButton);

			TestGV2.DataBinding += new EventHandler(TestGV_DataBinding);
			TestGV3.DataBinding += new EventHandler(TestGV_DataBinding);
			TestGV4.DataBinding += new EventHandler(TestGV_DataBinding);
		}

		void TestGV_RowCustomizingCommandButton(object sender, GridViewRowCustomizingCommandButtonEventArgs e)
		{
			if ((e.CommandName == CommandNames.Delete) && (e.RowIndex == 1))
			{
				e.Enabled = false;
			}
			if ((e.CommandName == CommandNames.Edit) && (e.RowIndex % 5 == 0))
			{
				e.Visible = false;
			}
		}

		void TestGV_DataBinding(object sender, EventArgs e)
		{
			SubjektCollection items = Subjekt.GetAll();
			items.AddRange(items);
			items.AddRange(items);
			items.AddRange(items);
			((GridView)sender).DataSource = items;
		}
	}
}
