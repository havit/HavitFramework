using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Havit.Web.Bootstrap.UI.WebControls;

namespace Havit.WebApplicationTest.HavitWebBootstrapTests;

public partial class ModalDialogUserControlTest : ModalDialogUserControlBase
{
	public event EventHandler SwitchDialogClick;

	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);
		CloseButton.Click += CloseButton_Click;
		ShowNestedButton.Click += ShowNestedButton_Click;
		SwitchDialogButton.Click += SwitchDialogButton_Click;
		ScriptManager.GetCurrent(this.Page).RegisterPostBackControl(RefreshPostBackButton);
		ScriptManager.GetCurrent(this.Page).Services.Add(new ServiceReference(SubjektASM.ServicePath));
	}

	private void SwitchDialogButton_Click(object sender, EventArgs e)
	{
		SwitchDialogClick?.Invoke(this, new EventArgs());
	}

	private void ShowNestedButton_Click(object sender, EventArgs e)
	{
		ModalDialogUserControlTest2.Show();
	}

	private void CloseButton_Click(object sender, EventArgs e)
	{
		Hide();
	}
}