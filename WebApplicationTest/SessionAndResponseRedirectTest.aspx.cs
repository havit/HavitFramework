using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Havit.Web.Security;
using System.Web.Security;

using Havit.Web.UI.WebControls;

namespace WebApplicationTest
{
	public partial class SessionAndResponseRedirectTest : System.Web.UI.Page
	{
		#region OnInit
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			TrueButton.Click += TrueButton_Click;
			FalseButton.Click += FalseButton_Click;
		}
		#endregion

		#region TrueButton_Click, FalseButton_Click, DoTest
		private void TrueButton_Click(object sender, EventArgs e)
		{
			DoTest(true);
		}

		private void FalseButton_Click(object sender, EventArgs e)
		{
			DoTest(false);
		}

		private void DoTest(bool condition)
		{
			// Incrementujeme obsah session
			Messenger.Default.AddMessage("Doing test...");
			if (condition)
			{
				FormsRolesAuthentication.RedirectFromLoginPage(Guid.NewGuid().ToString(), new string[] { }, false, FormsAuthentication.FormsCookiePath, this.Request.RawUrl);
			}
		}
		#endregion

		#region OnLoad
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			CounterLabel.Text = Messenger.Default.Messages.Count.ToString();
		}
		#endregion
	}
}