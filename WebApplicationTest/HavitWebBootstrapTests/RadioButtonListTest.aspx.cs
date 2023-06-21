using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.WebApplicationTest.HavitWebBootstrapTests;

public partial class RadioButtonListTest : System.Web.UI.Page
{
	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);
		DynamicalyAddItemButton.Click += DynamicalyAddItemButton_Click;
	}

	private void DynamicalyAddItemButton_Click(object sender, EventArgs e)
	{
		HorizontalRadioButtonList.Items.Add((HorizontalRadioButtonList.Items.Count + 1).ToString());
		VerticalRadioButtonList.Items.Add((VerticalRadioButtonList.Items.Count + 1).ToString());
	}
}