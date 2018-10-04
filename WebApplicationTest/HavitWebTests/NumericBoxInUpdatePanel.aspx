<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NumericBoxInUpdatePanel.aspx.cs" Inherits="Havit.WebApplicationTest.HavitWebTests.NumericBoxInUpdatePanel" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
</head>
<body>
	<form id="form1" runat="server">
	<div>

		<asp:ScriptManager runat="server">
			<Scripts>
				<asp:ScriptReference Name="jquery" />
			</Scripts>
		</asp:ScriptManager>

		
		<%--<havit:NumericBox ID="NumericBoxOutsideUpdatePanel" runat="server" />--%>
		<div style="border: 1px solid">
			Update panel
			<asp:UpdatePanel runat="server">
				<ContentTemplate>
					<havit:NumericBox ID="NumericBoxInUP" runat="server" />
					<asp:Button ID="AsyncPostButton" Text="Async post" runat="server" />
				</ContentTemplate>
			</asp:UpdatePanel>
		</div>

	</div>
	</form>
</body>
</html>