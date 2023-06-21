using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.WebApplicationTest.HavitWebTests;

public partial class DateTimeBoxTest2 : System.Web.UI.Page
{
	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);
		ShowButton.Click += ShowButton_Click;
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		if (!Page.IsPostBack)
		{
			HiddenPanel.DataBind();
		}
	}

	private void ShowButton_Click(object sender, EventArgs e)
	{
		HiddenPanel.Visible = true;
	}
}