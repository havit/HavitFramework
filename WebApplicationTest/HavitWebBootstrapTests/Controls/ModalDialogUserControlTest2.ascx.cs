using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Havit.Web.Bootstrap.UI.WebControls;

namespace Havit.WebApplicationTest.HavitWebBootstrapTests
{
	public partial class ModalDialogUserControlTest2 : ModalDialogUserControlBase
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			CloseButton.Click += CloseButton_Click;
			ScriptManager.GetCurrent(this.Page).RegisterPostBackControl(RefreshPostBackButton);
		}

		private void CloseButton_Click(object sender, EventArgs e)
		{
			Hide();
		}
	}
}