using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Havit.Web.Bootstrap.UI.WebControls;

namespace Havit.WebApplicationTest.HavitWebBootstrapTests
{
	public partial class ModalDialogUserControlTest : ModalDialogUserControlBase
	{
		public event EventHandler SwitchDialogClick;

		#region OnInit
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			CloseButton.Click += CloseButton_Click;
			ShowNestedButton.Click += ShowNestedButton_Click;
			SwitchDialogButton.Click += SwitchDialogButton_Click;
			ScriptManager.GetCurrent(this.Page).RegisterPostBackControl(RefreshPostBackButton);
			ScriptManager.GetCurrent(this.Page).Services.Add(new ServiceReference(SubjektASM.ServicePath));
		}
		#endregion

		#region SwitchDialogButton_Click
		private void SwitchDialogButton_Click(object sender, EventArgs e)
		{
			SwitchDialogClick?.Invoke(this, new EventArgs());
		}
		#endregion

		#region ShowNestedButton_Click
		private void ShowNestedButton_Click(object sender, EventArgs e)
		{
			ModalDialogUserControlTest2.Show();
		}
		#endregion

		#region CloseButton_Click
		private void CloseButton_Click(object sender, EventArgs e)
		{
			Hide();
		}
		#endregion

	}
}