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
		//protected override void OnInit(EventArgs e)
		//{
		//	base.OnInit(e);

		//	TesterACTB.SelectedValueChanged += TesterACTB_SelectedValueChanged;
		//	ConfimBt.Click += ConfimBt_Click;
		//}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			PostbackLabel.Text = TesterACTB.SelectedValue;

			if (!IsPostBack)
			{
				TesterACTB.SelectedValue = "15";
			}
		}

		//private void TesterACTB_SelectedValueChanged(object sender, EventArgs e)
		//{
		//	PostbackLabel.Text = TesterACTB.SelectedText;
		//}

		//private void ConfimBt_Click(object sender, EventArgs e)
		//{
		//	PostbackLabel.Text = TesterACTB.SelectedText;
		//}
	}
}