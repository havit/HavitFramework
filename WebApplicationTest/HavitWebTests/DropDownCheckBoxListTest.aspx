<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DropDownCheckBoxListTest.aspx.cs" Inherits="WebApplicationTest.HavitWebTests.DropDownCheckBoxListTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<link href="../Content/multiple-select.css" rel="stylesheet" />
</head>
<body>
	<form id="form1" runat="server">
		<asp:ScriptManager runat="server">
		</asp:ScriptManager>

		<div>
			<havit:DropDownCheckBoxList ID="MyListBox" runat="server">
				<Items>
					<asp:ListItem Text="Jedna" />
					<asp:ListItem Text="Dva" />
					<asp:ListItem Text="Tři" />
					<asp:ListItem Text="Čtyři" />
					<asp:ListItem Text="Pět" />
					<asp:ListItem Text="Šest" />
				</Items>
			</havit:DropDownCheckBoxList>
			
				<havit:DropDownCheckBoxList ID="DropDownCheckBoxList1" PlaceHolder="Vyber" ShowSelectAll="true" AllSelectedText="Vybráno vše" AutoPostBack="true" Width="300px" ItemWidth="100px" runat="server">
				<Items>
					<asp:ListItem Text="Jedna" />
					<asp:ListItem Text="Dva" />
					<asp:ListItem Text="Tři" />
					<asp:ListItem Text="Čtyři" />
					<asp:ListItem Text="Pět" />
					<asp:ListItem Text="Šest" />
				</Items>
			</havit:DropDownCheckBoxList>
			
				<havit:DropDownCheckBoxList ID="DropDownCheckBoxList2" Enabled="false" runat="server">
				<Items>
					<asp:ListItem Text="Jedna" />
					<asp:ListItem Text="Dva" />
					<asp:ListItem Text="Tři" />
					<asp:ListItem Text="Čtyři" />
					<asp:ListItem Text="Pět" />
					<asp:ListItem Text="Šest" />
				</Items>
			</havit:DropDownCheckBoxList>
			
		<asp:Button Text="Postback" runat="server" />
		
			<asp:UpdatePanel runat="server">
				<ContentTemplate>
					<asp:Button Text="Async Postback" runat="server" />
				</ContentTemplate>
			</asp:UpdatePanel>
	</form>
</body>
</html>
