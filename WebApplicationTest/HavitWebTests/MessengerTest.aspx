<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="MessengerTest.aspx.cs" EnableSessionState="True" Inherits="Havit.WebApplicationTest.HavitWebTests.MessengerTest" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>Untitled Page</title>
	<link rel="stylesheet" type="text/css" href="content/toastr.css" />
</head>
<body>
	<form id="form1" runat="server">
		<asp:ScriptManager runat="server" />
		<div>
			<asp:UpdatePanel RenderMode="Inline" runat="server">
				<ContentTemplate>
					
				<havit:MessengerControl ShowMessageBox="true" ShowSummary="true" ShowToastr="true" runat="server" />
					<asp:Button Text="PostBack" runat="server" />
					<asp:Button ID="MessagesButton" Text="Add messages" runat="server" />
					<asp:Button ID="MessagesWithRedirectButton" Text="Add messages with redirect" runat="server" />			
				</ContentTemplate>
			</asp:UpdatePanel>

		</div>
	</form>
</body>