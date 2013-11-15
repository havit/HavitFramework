using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Havit.Web.UI.WebControls;

namespace WebApplicationTest
{
	public partial class MessengerTest : System.Web.UI.Page
	{
		#region OnInit
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			MessagesButton.Click += new EventHandler(MessagesButton_Click);
		}
		#endregion

		#region MessagesButton_Click
		private void MessagesButton_Click(object sender, EventArgs e)
		{
			Messenger.Default.AddMessage("Je právě: " + DateTime.Now.ToString("g"));
			Messenger.Default.AddMessage(MessageType.Warning, "Není <b>už</b> pozdě?");
			Messenger.Default.AddMessage(MessageType.Error, "Error message's testing single quotation mark.");
			Messenger.Default.AddMessage(MessageType.Error, "Error message\"s testing single quotation mark.");
			Messenger.Default.AddMessage(MessageType.Error, "Error message'\"\"'s testing single quotation mark.");
		}
		#endregion
	}
}
