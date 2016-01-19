using Havit.Web.UI.WebControls.ControlsValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.WebApplicationTest.HavitWebBootstrapTests
{
	public partial class CollapsiblePanelTestPage2 : System.Web.UI.Page
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			SaveButton.Click += new EventHandler(SaveButton_Click);			
			LoadButton.Click += new EventHandler(LoadButton_Click);

			FirstSwitchButton.ValueChanged += FirstSwitchButton_ValueChanged;
			SecondSwitchButton.ValueChanged += SecondSwitchButton_ValueChanged;
		}

		protected void GotoBtn_Click(object sender, EventArgs e)
		{
			this.Response.Redirect("~/HavitWebBootstrapTests/CollapsiblePanelTestPage1.aspx");
		}

		protected void PostbackBtn_Click(object sender, EventArgs e)
		{
			// nothing todo
		}
		
		private void SaveButton_Click(object sender, EventArgs e)
		{
			ControlsValuesHolder data = MainControlsValuesPersister.RetrieveValues();
			Session["CollapsiblePanelTestPage2.aspx"] = data;
		}

		private void LoadButton_Click(object sender, EventArgs e)
		{
			ControlsValuesHolder data = (ControlsValuesHolder)Session["CollapsiblePanelTestPage2.aspx"];
			if (data != null)
			{
				MainControlsValuesPersister.ApplyValues(data);
			}
		}

		private void FirstSwitchButton_ValueChanged(object sender, EventArgs e)
		{
			FirstStateLabel.Text = FirstSwitchButton.Value ? FirstSwitchButton.YesText : FirstSwitchButton.NoText;
		}

		private void SecondSwitchButton_ValueChanged(object sender, EventArgs e)
		{
			SecondStateLabel.Text = SecondSwitchButton.Value ? SecondSwitchButton.YesText : SecondSwitchButton.NoText;
		}
	}
}