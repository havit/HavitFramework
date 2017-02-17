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
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			TesterACTB.SelectedValueChanged += TesterACTB_SelectedValueChanged;
			TesterACTB.TextChanged += TesterACTB_TextChanged;
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

		private void TesterACTB_SelectedValueChanged(object sender, EventArgs e)
		{
			PostbackValueLabel.Text = "*" + TesterACTB.SelectedValue + "*";
		}

		private void TesterACTB_TextChanged(object sender, EventArgs e)
		{
			PostbackTextLabel.Text = "*" + TesterACTB.Text + "*";
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