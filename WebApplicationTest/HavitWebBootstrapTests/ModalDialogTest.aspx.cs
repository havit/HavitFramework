using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplicationTest.HavitWebBootstrapTests
{
	public partial class ModalDialogTest : System.Web.UI.Page
	{
		#region OnInit
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			OpenButton.Click += OpenButton_Click;
		}
		#endregion

		#region OpenButton_Click
		private void OpenButton_Click(object sender, EventArgs e)
		{
			ModalDialogUserControlTestUC.Show();
		}
		#endregion
	}
}