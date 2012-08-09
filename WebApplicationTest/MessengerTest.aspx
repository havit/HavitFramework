<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MessengerTest.aspx.cs" Inherits="WebApplicationTest.MessengerTest" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>Untitled Page</title>
</head>
<body>
	<form id="form1" runat="server">
		<div>
			<havit:MessengerControl ShowMessageBox="true" ShowSummary="true" runat="server" />
			<asp:Button Text="PostBack" runat="server" />
			<asp:Button ID="MessagesButton" Text="Add messages" runat="server" />
			<input type=button value="x" onclick="window.setTimeout(function() {{ alert('hello'); }}, 10);" />
		</div>
	</form>
</body>