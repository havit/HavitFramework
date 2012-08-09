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
using Havit.Web.UI.WebControls;

namespace WebApplicationTest
{
	public partial class DateTimeBoxTest : System.Web.UI.Page
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			AutoPostBackDateTimeBox.Value = DateTime.Now;
			AutoPostBackDateTimeBox.ValueChanged += new EventHandler(AutoPostBackDateTimeBox_ValueChanged);
			AutoPostBackDateTimeBoxValidator.ServerValidate += new ServerValidateEventHandler(AutoPostBackDateTimeBoxValidator_ServerValidate);
			AutoPostBackOnInitValueLabel.Text = AutoPostBackDateTimeBox.Value.ToString();

			SecondDateTimeBoxValidator.ServerValidate += new ServerValidateEventHandler(SecondDateTimeBoxValidator_ServerValidate);

			VycistitButton.Click += new EventHandler(VycistitButton_Click);
			ZobrazitButton.Click += new EventHandler(ZobrazitButton_Click);

			TestGV.DataBinding += new EventHandler(TestGV_DataBinding);
			TestGV.RowDataBound += new GridViewRowEventHandler(TestGV_RowDataBound);
		}

		void TestGV_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if ((e.Row.RowState & DataControlRowState.Edit) == DataControlRowState.Edit)
			{
				((DateTimeBox)e.Row.FindControl("NestedDateTimeBox")).Value = DateTime.Today;
			}
		}

		private void TestGV_DataBinding(object sender, EventArgs e)
		{
			Random r = new Random();
			TestGV.DataSource = System.Linq.Enumerable.Range(1, 5);
		}

		private void SecondDateTimeBoxValidator_ServerValidate(object source, ServerValidateEventArgs args)
		{
			PostBackButton.Text = "Valid: " + SecondDateTimeBox.IsValid.ToString();
		}	

		private void AutoPostBackDateTimeBox_ValueChanged(object sender, EventArgs e)
		{
			ChangedLabel.Text = "yes";
			if (Page.IsValid)
			{
			}
		}

		private void AutoPostBackDateTimeBoxValidator_ServerValidate(object source, ServerValidateEventArgs args)
		{
			AutoPostBackServerValidatorValueLabel.Text = AutoPostBackDateTimeBox.Value.ToString();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			AutoPostBackOnLoadValueLabel.Text = AutoPostBackDateTimeBox.Value.ToString();
			ChangedLabel.Text = "no";
			TestGV.SetRequiresDatabinding();
		}

		protected override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			AutoPostBackOnLoadCompleteValueLabel.Text = AutoPostBackDateTimeBox.Value.ToString();
		}

		void VycistitButton_Click(object sender, EventArgs e)
		{
			DrubyDateTimeBox.Value = null;
			DrubyDateTimeBox.Visible = false;
		}

		void ZobrazitButton_Click(object sender, EventArgs e)
		{
			DrubyDateTimeBox.Visible = true;
		}
	}
}
