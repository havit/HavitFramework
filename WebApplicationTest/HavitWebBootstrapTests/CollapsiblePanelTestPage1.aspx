<%@ Page Title="" Language="C#" MasterPageFile="~/HavitWebBootstrapTests/Bootstrap.Master" StylesheetTheme="BootstrapTheme" CodeBehind="CollapsiblePanelTestPage1.aspx.cs" Inherits="Havit.WebApplicationTest.HavitWebBootstrapTests.CollapsiblePanelTestPage1" %>

<asp:Content ContentPlaceHolderID="MainCPH" runat="server">

	<havit:ControlsValuesPersister ID="MainControlsValuesPersister" runat="server">

		<style>
			div.Havit_CollapsiblePanel_Header {
				font-size: larger;
				font-weight: 600;
				background-color: indianred;
			}
		</style>

		<h1>PAGE 1</h1>

		<h2>Collapsible panel test 1:</h2>

		<hr />

		<bc:CollapsiblePanel ID="CollapsiblePanel1" HeaderText="It is not showed..." runat="server">
			<HeaderTemplate>
				<h1 style="background-color: indianred">
					<asp:Label ID="HeaderLabel" Text="Header text of the collapsible panel 1 - please click here" runat="server" />
				</h1>
			</HeaderTemplate>
			<ContentTemplate>
				<p>This context 1 is in collapsible section...</p>
				<bc:SwitchButton ID="FirstSwitchButton" AutoPostBack="true" CausesValidation="true" ValidationGroup="Some" runat="server" />
				<asp:Label ID="FirstStateLabel" runat="server" />
			</ContentTemplate>
		</bc:CollapsiblePanel>

		<hr />

		<br/>
		<br/>

		<h1>Collapsible panel test 2:</h1>

		<hr />

		<bc:CollapsiblePanel ID="CollapsiblePanel2" HeaderText="Header text of the collapsible panel 2 - please click here" runat="server">
			<ContentTemplate>
				<p>This context 2 is in collapsible section...</p>
				<bc:SwitchButton ID="SecondSwitchButton" AutoPostBack="true" CausesValidation="true" ValidationGroup="Some" runat="server" />
				<asp:Label ID="SecondStateLabel" runat="server" />
			</ContentTemplate>
		</bc:CollapsiblePanel>

		<hr />

		<br/>
		<br/>

		<h2>Goto second page:</h2>
		<asp:Button ID="CollapsiblePanelTest1Btn" Text="Goto page 2" OnClick="GotoBtn_Click" runat="server" />

		<h1>Empty postback:</h1>
		<asp:Button ID="PostbackBtn" Text="Empty postback" OnClick="PostbackBtn_Click" runat="server" />

		<h1>Persistence test:</h1>
		<asp:Button Text="Save" ID="SaveButton" runat="server" />
		<asp:Button Text="Load" ID="LoadButton" runat="server" />

	</havit:ControlsValuesPersister>

</asp:Content>