using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Havit.BusinessLayerTest;
using Havit.Web.UI.WebControls;
using Havit.Web.UI;
using System.Collections.Generic;

namespace Havit.WebApplicationTest.HavitWebTests
{
	public partial class GridViewExtTest : System.Web.UI.Page
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			//TestGV1.DataBinding += new EventHandler(TestGV_DataBinding);
			TestGV1.RowCustomizingCommandButton += new GridViewRowCustomizingCommandButtonEventHandler(TestGV_RowCustomizingCommandButton);
			TestGV1.GetInsertRowDataItem += TestGV1_GetInsertRowDataItem;
			TestGV1.RowDeleting += TestGV1_RowDeleting;
			TestGV2.DataBinding += new EventHandler(TestGV_DataBinding);
			TestGV3.DataBinding += new EventHandler(TestGV_DataBinding);
			TestGV4.DataBinding += new EventHandler(TestGV_DataBinding);

			HideButton.Click += new EventHandler(HideButton_Click);
			SRDBButton.Click += new EventHandler(SRDBButton_Click);

			TestGV1.Visible = true;
			TestGV2.Visible = false;
			TestGV3.Visible = false;
		}

		public IEnumerable<Subjekt> TestGV1_SelectMethod()
		{
			return Subjekt.GetAll();
		}

		public void TestGV1_UpdateMethod()
		{
			// NOOP
		}

		private void HideButton_Click(object sender, EventArgs e)
		{
			TestGV4.Visible = false;
		}

		private void SRDBButton_Click(object sender, EventArgs e)
		{
			TestGV4.SetRequiresDatabinding();
		}

		private void TestGV_RowCustomizingCommandButton(object sender, GridViewRowCustomizingCommandButtonEventArgs e)
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

		private void TestGV_DataBinding(object sender, EventArgs e)
		{
			//((GridView)sender).DataSource = new object[]
			//									{
			//										new { ID = 1, Nazev = "A" },
			//										new { ID = 2, Nazev = "B" },
			//										new { ID = 3, Nazev = "C" },
			//									};
			((GridView)sender).DataSource = Subjekt.GetAll().ToList();
		}

		private object TestGV1_GetInsertRowDataItem()
		{
			return Subjekt.CreateObject();
		}

		private void TestGV1_RowDeleting(object sender, GridViewDeleteEventArgs e)
		{
			Messenger.Default.AddMessage("Delete clicked");
		}
	}
}
