<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="DateTimeBoxTest2.aspx.cs" Inherits="Havit.WebApplicationTest.HavitWebTests.DateTimeBoxTest2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>

		<asp:Panel ID="HiddenPanel" Visible="False" runat="server" >
			<havit:DateTimeBox ID="DateTimeBoxWithDatabindWhenHiddenDTB" Value="<%# DateTime.Now %>" runat="server"/>
		</asp:Panel>
		<asp:Button ID="ShowButton" Text="Show" runat="server" />
		    
    </div>
    </form>
</body>
</html>
