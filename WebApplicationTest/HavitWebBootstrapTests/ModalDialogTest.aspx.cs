using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Havit.Web.UI.WebControls;

namespace Havit.WebApplicationTest.HavitWebBootstrapTests;

public partial class ModalDialogTest : System.Web.UI.Page
{
	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);
		OpenButton.Click += OpenButton_Click;
		ModalDialogUserControlTestUC.SwitchDialogClick += SwitchDialogClick;
		DynarchCalendar.RegisterCalendarSkinStylesheets(this);			
	}

	private void SwitchDialogClick(object sender, EventArgs eventArgs)
	{
		ModalDialogUserControlTestUC.Hide();
		ModalDialogUserControlTest2UC.Show();
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		if (!Page.IsPostBack)
		{
			//ModalDialogUserControlTestUC.Show();
		}

		Form.DefaultButton = TestButton.UniqueID;
	}

	private void OpenButton_Click(object sender, EventArgs e)
	{
		ModalDialogUserControlTestUC.Show();
	}
}