<%@ Page Title="" Language="C#" MasterPageFile="~/HavitWebBootstrapTests/Bootstrap.Master" AutoEventWireup="false" CodeBehind="TabsTest.aspx.cs" Inherits="Havit.WebApplicationTest.HavitWebBootstrapTests.TabsTest" %>

<asp:Content ContentPlaceHolderID="MainCPH" runat="server">
	<!-- -------------------- -->
	<bc:TabContainer ID="MainTabContainer" runat="server">
		<bc:TabPanel HeaderText="Invisible" Visible="False" runat="server">
			<ContentTemplate>Invisible content</ContentTemplate>
		</bc:TabPanel>
		<bc:TabPanel HeaderText="Disabled" Enabled="False" runat="server">
			<ContentTemplate>Disabled content</ContentTemplate>
		</bc:TabPanel>
		<asp:Repeater ID="TabContainersRepeater" ItemType="System.Int32" runat="server">
			<ItemTemplate>
				<bc:TabPanel HeaderText='<%# "#" + Item.ToString() %>' runat="server">
					<ContentTemplate>Repeater Item <%# ((RepeaterItem)Container).DataItem %> content</ContentTemplate>
				</bc:TabPanel>
			</ItemTemplate>
		</asp:Repeater>
		<bc:TabPanel runat="server">
			<HeaderTemplate>Normal tab <i>with header template</i></HeaderTemplate>
			<ContentTemplate>
				<asp:TextBox runat="server" />
				<asp:Button Text="Postback" runat="server" />
			</ContentTemplate>
		</bc:TabPanel>

	</bc:TabContainer>
	<!-- -------------------- -->
	
	<bc:TabContainer TabMode="Pills" runat="server">
		<bc:TabPanel HeaderText="Panel 1" runat="server">
			<ContentTemplate>Panel 1</ContentTemplate>
		</bc:TabPanel>
		<bc:TabPanel HeaderText="Panel 2" runat="server">
			<ContentTemplate>Panel 2</ContentTemplate>
		</bc:TabPanel>
		<bc:TabPanel HeaderText="Panel 3" runat="server">
			<ContentTemplate>Panel 3</ContentTemplate>
		</bc:TabPanel>
	</bc:TabContainer>

</asp:Content>
