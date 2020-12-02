using Havit.Web.UI.WebControls;
using Havit.Web.UI.WebControls.ControlsValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace Havit.WebApplicationTest.HavitWebTests
{
	public partial class AutoCompleteTextBox : System.Web.UI.Page
	{
		// pozn.: krkolomné kombinace jsou tu kvůli registraci a ověření scriptu longClick
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			TesterACTB.SelectedValueChanged += TesterACTB_SelectedValueChanged;
			TesterACTB.TextChanged += TesterACTB_TextChanged;
			AutoCompleteTextBox1.TextChanged += AutoCompleteTextBox1_TextChanged;
			ConfimBt.Click += ConfimBt_Click;
			HideBt.Click += HideBt_Click;
			ShowBt.Click += ShowBt_Click;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			PostbackValueLabel.Text = TesterACTB.SelectedValue;
			PostbackTextLabel.Text = TesterACTB.Text;

			if (!IsPostBack)
			{
				TesterACTB.SelectedValue = "15";
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			base.Render(writer);
			ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(ConfimBt);
		}

		private void TesterACTB_SelectedValueChanged(object sender, EventArgs e)
		{
			PostbackValueLabel.Text = "*" + TesterACTB.SelectedValue + "*";
		}

		private void TesterACTB_TextChanged(object sender, EventArgs e)
		{
			PostbackTextLabel.Text = "*" + TesterACTB.Text + "*";
		}

		private void AutoCompleteTextBox1_TextChanged(object sender, EventArgs e)
		{
			PostbackTextLabel.Text = "*" + AutoCompleteTextBox1.Text + "*";
			UpdatePanel1.Update();
		}

		private void ConfimBt_Click(object sender, EventArgs e)
		{
			UpdatePanel1.Update();
		}

		private void ShowBt_Click(object sender, EventArgs e)
		{
			TesterACTB.Visible = true;
		}

		private void HideBt_Click(object sender, EventArgs e)
		{
			TesterACTB.Visible = false;
		}
	}
}