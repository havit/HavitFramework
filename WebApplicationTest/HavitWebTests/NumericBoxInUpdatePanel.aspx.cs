using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.WebApplicationTest.HavitWebTests;

public partial class NumericBoxInUpdatePanel : System.Web.UI.Page
{
	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);

		AsyncPostButton.Click += AsyncPostButton_Click;
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);

		if (!IsPostBack)
		{
			NumericBoxInUP.Visible = false;
		}
	}

	private void AsyncPostButton_Click(object sender, EventArgs e)
	{
		NumericBoxInUP.Visible = true;
	}
}