<%@ Page Language="C#" MasterPageFile="Bootstrap.Master" AutoEventWireup="false" CodeBehind="CheckBoxListTest.aspx.cs" Inherits="WebApplicationTest.HavitWebBootstrapTests.CheckBoxListListTest" %>

<asp:Content ContentPlaceHolderID="MainCPH" runat="server">

	<asp:UpdatePanel runat="server">
		<Triggers>
			<asp:AsyncPostBackTrigger ControlID="DynamicalyAddItemButton" />
		</Triggers>
		<ContentTemplate>

			<bc:CheckBoxList ID="HorizontalCheckBoxList" RepeatDirection="Horizontal" runat="server">
				<asp:ListItem Value="Abc" Text="A<b>C" Selected="true"></asp:ListItem>
				<asp:ListItem Text="DEF"></asp:ListItem>
				<asp:ListItem Text="GHI"></asp:ListItem>
			</bc:CheckBoxList>
			<bc:CheckBoxList ID="VerticalCheckBoxList" RepeatDirection="Vertical" runat="server">
				<asp:ListItem Value="Abc" Text="A<b>C" Selected="true"></asp:ListItem>
				<asp:ListItem Text="DEF"></asp:ListItem>
				<asp:ListItem Text="GHI"></asp:ListItem>
			</bc:CheckBoxList>
		</ContentTemplate>
	</asp:UpdatePanel>

	<asp:Button ID="DynamicalyAddItemButton" Text="Add Items" runat="server" />
</asp:Content>
