<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PageNavigator_B_Test.aspx.cs" Inherits="Havit.WebApplicationTest.HavitWebTests.PageNavigator_B_Test" %>
<%@ Import Namespace="Havit.Web" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body id="body">
    <form id="form1" runat="server">
		<asp:ScriptManager runat="server"/>
    <div>
		<p>This is <strong>B</strong>.</p>
    	<asp:Button ID="BackButton" Text="Back" runat="server" />
		<asp:Button ID="ToAButton" Text="To A" runat="server" />
		
		<div>
			
			<asp:TextBox Text="Bbb" runat="server" />
			<asp:TextBox Text="Bbb" runat="server" />
			<asp:Button Text="Postback" runat="server"/>
			<asp:UpdatePanel RenderMode="Inline" runat="server">
				<ContentTemplate>
					<asp:Button Text="Callback" runat="server"/>
				</ContentTemplate>
			</asp:UpdatePanel>

		</div>
		
		<p><%= DateTime.Now.ToLongTimeString() %></p>

		<havit:BrowserNavigationController runat="server" />
    </div>
    </form>
</body>
</html>
