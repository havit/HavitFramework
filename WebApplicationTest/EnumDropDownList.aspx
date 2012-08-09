<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EnumDropDownList.aspx.cs" Inherits="WebApplicationTest.EnumDropDownList_aspx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<havit:EnumDropDownList ID="TestDDL" EnumType="<%$ Expression: typeof(WebApplicationTest.TestEnum) %>" DataTextFormatString="$resources: {1}, {0}" Nullable="true" runat="server" />
		<havit:EnumDropDownList ID="Test2DDL" EnumType="<%$ Expression: typeof(WebApplicationTest.TestEnum) %>" Nullable="true" runat="server" />
		<havit:EnumDropDownList ID="Test3DDL" EnumType="<%$ Expression: typeof(WebApplicationTest.TestEnum) %>" Nullable="false" runat="server" />
		<havit:EnumDropDownList ID="Test4DDL" EnumType="<%$ Expression: typeof(WebApplicationTest.TestEnum) %>" runat="server" />
		<asp:Label ID="TestLb" runat="server" />
		<asp:Button ID="TestBt" Text="Test" runat="server" />
	
		<asp:GridView ID="MainGridView" runat="server">
			<Columns>
				<asp:TemplateField>
					<ItemTemplate>						
						<havit:EnumDropDownList ID="Test4DDL" EnumType="<%$ Expression: typeof(WebApplicationTest.TestEnum) %>" SelectedEnumValue="<%$ Expression: WebApplicationTest.TestEnum.EnumHodnota2 %>" runat="server" />
					</ItemTemplate>
				</asp:TemplateField>
			</Columns>
		</asp:GridView>
    </div>
    </form>
</body>
</html>
