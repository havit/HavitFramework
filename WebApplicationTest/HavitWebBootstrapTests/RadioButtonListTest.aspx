<%@ Page Language="C#" MasterPageFile="Bootstrap.Master" AutoEventWireup="false" CodeBehind="RadioButtonListTest.aspx.cs" Inherits="Havit.WebApplicationTest.HavitWebBootstrapTests.RadioButtonListTest" %>

<asp:Content ContentPlaceHolderID="MainCPH" runat="server">

	<asp:UpdatePanel runat="server">
		<Triggers>
			<asp:AsyncPostBackTrigger ControlID="DynamicalyAddItemButton" />
		</Triggers>
		<ContentTemplate>

			<bc:RadioButtonList ID="HorizontalRadioButtonList" RepeatDirection="Horizontal" runat="server">
				<asp:ListItem Value="Abc" Text="A<b>C" Selected="true"></asp:ListItem>
				<asp:ListItem Text="Def"></asp:ListItem>
				<asp:ListItem Text="Ghi"></asp:ListItem>
			</bc:RadioButtonList>
			<bc:RadioButtonList ID="VerticalRadioButtonList" HtmlEncode="false" RepeatDirection="Vertical" runat="server">
				<asp:ListItem Value="Abc" Text="Abc &lt;span class=&quot;badge&quot;&gt;1&lt;/span&gt;" Selected="true"></asp:ListItem>
				<asp:ListItem Value="Def" Text="Def &lt;span class=&quot;badge&quot;&gt;2&lt;/span&gt;"></asp:ListItem>
				<asp:ListItem Value="Ghi" Text="Ghi &lt;span class=&quot;badge&quot;&gt;3&lt;/span&gt;"></asp:ListItem>
			</bc:RadioButtonList>
		</ContentTemplate>
	</asp:UpdatePanel>

	<asp:Button ID="DynamicalyAddItemButton" Text="Add Items" runat="server" />
</asp:Content>
