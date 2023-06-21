using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Havit.Web.UI.WebControls;

namespace Havit.WebApplicationTest.HavitWebTests;

public partial class MessengerTest : System.Web.UI.Page
{
	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);
		MessagesButton.Click += new EventHandler(MessagesButton_Click);
		MessagesWithRedirectButton.Click += new EventHandler(MessagesWithRedirectButton_Click);
	}

	private void MessagesButton_Click(object sender, EventArgs e)
	{
		AddMessages();
	}

	private void MessagesWithRedirectButton_Click(object sender, EventArgs e)
	{
		AddMessages();
		Havit.Web.QueryStringBuilder builder = new Havit.Web.QueryStringBuilder();
		builder.Add("AfterRedirect", "Yes");
		Response.Redirect(builder.GetUrlWithQueryString(Request.RawUrl));
	}

	private void AddMessages()
	{
		Messenger.Default.AddMessage("1- Je právě: " + DateTime.Now.ToString("g"));
		Messenger.Default.AddMessage(MessageType.Warning, "2- Není <b>už</b> \r\n\r\n pozdě?\r\n\r\n\r\n");
		Messenger.Default.AddMessage(MessageType.Error, "3- Error message's testing single quotation mark.\r\n");
		Messenger.Default.AddMessage(MessageType.Error, "4- Error message\"s testing single quotation mark.");
		Messenger.Default.AddMessage(MessageType.Error, "5- Error message'\"\"'s testing single quotation mark.");
	}
}
