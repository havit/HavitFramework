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
        }

		private void TesterACTB_SelectedValueChanged(object sender, EventArgs e)
		{
			PostbackLabel.Text = TesterACTB.SelectedText;
		}
	}
}