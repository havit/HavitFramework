<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PageNavigator_A_Test.aspx.cs" Inherits="WebApplicationTest.HavitWebTests.PageNavigator_A_Test" %>
<%@ Import Namespace="Havit.Web" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<p>This is <strong>A</strong>.</p>

		<asp:Button ID="BackButton" Text="Back" runat="server" />
		<asp:Button ID="ToBButton" Text="To B" runat="server" />
				
		<p><%= DateTime.Now.ToLongTimeString() %></p>
		
		<havit:BrowserNavigationController runat="server" />
    </div>
    </form>
</body>
</html>
