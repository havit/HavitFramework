<%@ Page Language="C#" MasterPageFile="Bootstrap.Master" AutoEventWireup="false" CodeBehind="RadioButtonListTest.aspx.cs" Inherits="WebApplicationTest.HavitWebBootstrapTests.RadioButtonListTest" %>

<asp:Content ContentPlaceHolderID="MainCPH" runat="server">

	<asp:UpdatePanel runat="server">
		<Triggers>
			<asp:AsyncPostBackTrigger ControlID="DynamicalyAddItemButton" />
		</Triggers>
		<ContentTemplate>

			<bc:RadioButtonList ID="HorizontalRadioButtonList" RepeatDirection="Horizontal" runat="server">
				<asp:ListItem Value="Abc" Text="A<b>C" Selected="true"></asp:ListItem>
				<asp:ListItem Text="DEF"></asp:ListItem>
				<asp:ListItem Text="GHI"></asp:ListItem>
			</bc:RadioButtonList>
			<bc:RadioButtonList ID="VerticalRadioButtonList" RepeatDirection="Vertical" runat="server">
				<asp:ListItem Value="Abc" Text="A<b>C" Selected="true"></asp:ListItem>
				<asp:ListItem Text="DEF"></asp:ListItem>
				<asp:ListItem Text="GHI"></asp:ListItem>
			</bc:RadioButtonList>
		</ContentTemplate>
	</asp:UpdatePanel>

	<asp:Button ID="DynamicalyAddItemButton" Text="Add Items" runat="server" />
</asp:Content>
