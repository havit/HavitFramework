<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="DateTimeBoxTest3.aspx.cs" Inherits="Havit.WebApplicationTest.HavitWebTests.DateTimeBoxTest3" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>

		<asp:Panel ID="ContentPanel" runat="server" >			
		</asp:Panel>
		<asp:Button ID="PostbackButton" Text="Postback" runat="server" />
        <br />
		Value: <asp:Label ID="ValueLabel" runat="server" />
    </div>
    </form>
</body>
</html>
