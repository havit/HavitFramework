<%@ Page Title="" Language="C#" MasterPageFile="~/HavitWebBootstrapTests/Bootstrap.Master" StylesheetTheme="BootstrapTheme" CodeBehind="CollapsiblePanelTestPage2.aspx.cs" Inherits="Havit.WebApplicationTest.HavitWebBootstrapTests.CollapsiblePanelTestPage2" %>

<asp:Content ContentPlaceHolderID="MainCPH" runat="server">

	<havit:ControlsValuesPersister ID="MainControlsValuesPersister" runat="server">

		<style>
			div.Havit_CollapsiblePanel_Header {
				font-size: larger;
				font-weight: 600;
				background-color: indianred;
			}
		</style>

		<h1>PAGE 2</h1>

		<h2>Collapsible panel test 3:</h2>

		<hr />

		<bc:CollapsiblePanel ID="CollapsiblePanel3" HeaderText="It is not showed..." Collapsed="False" AutoPostBack="True" runat="server">
			<HeaderTemplate>
				<h1 style="background-color: indianred">
					<asp:Label ID="HeaderLabel" Text="Header text of the collapsible panel 3 - please click here" runat="server" />
				</h1>
			</HeaderTemplate>
			<ContentTemplate>
				<p>This context 3 is in collapsible section...</p>
				<bc:SwitchButton ID="FirstSwitchButton" AutoPostBack="true" CausesValidation="true" ValidationGroup="Some" runat="server" />
				<asp:Label ID="FirstStateLabel" runat="server" />
			</ContentTemplate>
		</bc:CollapsiblePanel>

		<hr />

		<br/>
		<br/>

		<h2>Collapsible panel test 4:</h2>

		<hr />

		<bc:CollapsiblePanel ID="CollapsiblePanel2" HeaderText="Header text of the collapsible panel 4 - please click here" runat="server">
			<ContentTemplate>
				<p>This context 4 is in collapsible section...</p>
				<bc:SwitchButton ID="SecondSwitchButton" AutoPostBack="true" CausesValidation="true" ValidationGroup="Some" runat="server" />
				<asp:Label ID="SecondStateLabel" runat="server" />
			</ContentTemplate>
		</bc:CollapsiblePanel>

		<hr />

		<br/>
		<br/>

		<h2>Goto second page:</h2>
		<asp:Button ID="CollapsiblePanelTest2Btn" Text="Goto page 1" OnClick="GotoBtn_Click" runat="server" />

		<h1>Empty postback:</h1>
		<asp:UpdatePanel UpdateMode="Always" runat="Server">
			<ContentTemplate>
				<asp:Button ID="PostbackBtn" Text="Empty postback" OnClick="PostbackBtn_Click" runat="server" />
			</ContentTemplate>
		</asp:UpdatePanel>

		<h1>Persistence test:</h1>
		<asp:Button Text="Save" ID="SaveButton" runat="server" />
		<asp:Button Text="Load" ID="LoadButton" runat="server" />

	</havit:ControlsValuesPersister>

</asp:Content>