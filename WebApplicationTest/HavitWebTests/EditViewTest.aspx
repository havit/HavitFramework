<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="EditViewTest.aspx.cs" Inherits="Havit.WebApplicationTest.HavitWebTests.EditViewTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
	<asp:ScriptManager runat="server" />
    <div>	
<%--		<havit:EditView ID="MyEditView" ItemType="Havit.BusinessLayerTest.Uzivatel" RenderOuterTable="false" runat="server">
			<ContentTemplate>
				Nazev: <asp:TextBox ID="NazevTB" Text="<%# BindItem.DisplayAs %>" runat="server" />
			<asp:Button CommandName="Cancel" Text="Cancel" runat="server" />
			<asp:Button CommandName="Update" Text="Update" runat="server" />
			</ContentTemplate>
		</havit:EditView>--%>
    </div>
    </form>
</body>
</html>
