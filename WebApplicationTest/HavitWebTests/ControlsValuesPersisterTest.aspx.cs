using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Havit.Web.UI.WebControls.ControlsValues;

namespace Havit.WebApplicationTest.HavitWebTests
{
	public partial class ControlsValuesPersisterTest : System.Web.UI.Page
	{
		#region OnInit
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			SaveButton.Click += new EventHandler(SaveButton_Click);
			ClearButton.Click += new EventHandler(ClearButton_Click);
			LoadButton.Click += new EventHandler(LoadButton_Click);
			TestGVE.DataBinding += TestGVE_DataBinding;
			TestGVE.DataBind();
		}

		private void TestGVE_DataBinding(object sender, EventArgs e)
		{
			TestGVE.DataSource = Enumerable.Range(0, 5);
		}
		#endregion

		#region ClearButton_Click
		private void ClearButton_Click(object sender, EventArgs e)
		{
			TestTextBox.Text = "";
			TestNB.Value = null;
			TestChBL.ClearSelection();
			TestLB.ClearSelection();
			TestRBL.ClearSelection();

			foreach (GridViewRow row in TestGVE.Rows)
			{
				TextBox tb = row.FindControl("NazevTextBox") as TextBox;
				if (tb != null)
				{
					tb.Text = "";
				}
			}
		}
		#endregion

		#region SaveButton_Click
		private void SaveButton_Click(object sender, EventArgs e)
		{		
			ControlsValuesHolder data = MainControlsValuesPersister.RetrieveValues();
			ViewState["Data"] = data;
		}
		#endregion

		#region LoadButton_Click
		private void LoadButton_Click(object sender, EventArgs e)
		{			
			ControlsValuesHolder data = (ControlsValuesHolder)ViewState["Data"];
			if (data != null)
			{
				MainControlsValuesPersister.ApplyValues(data);
			}
		}
		#endregion

	}
}