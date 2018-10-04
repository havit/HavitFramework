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

		<h2>Test update panel - PostBackTrigger - FullPostBack:</h2>

		<asp:UpdatePanel runat="server">
			<Triggers>
				<asp:PostBackTrigger ControlID="TestUpdatePanelCollapsiblePanel1" />
			</Triggers>
			<ContentTemplate>
				<bc:CollapsiblePanel ID="TestUpdatePanelCollapsiblePanel1" AutoPostBack="True" runat="server">
					<HeaderTemplate>
						HEADER
					</HeaderTemplate>
					<ContentTemplate>
						BODY
					</ContentTemplate>
				</bc:CollapsiblePanel>
			</ContentTemplate>
		</asp:UpdatePanel>

		<hr />

		<br/>
		<br/>

		<h2>Test update panel - AsyncPostBack:</h2>

		<asp:UpdatePanel runat="server">
			<ContentTemplate>
				<bc:CollapsiblePanel ID="CollapsiblePanel3" OnCollapsedStateChanged="CollapsiblePanel3_CollapsedStateChanged" AutoPostBack="True" runat="server">
					<HeaderTemplate>
						<h1 id="CollapsiblePanel3H1" class="panel-title" runat="server" />
					</HeaderTemplate>
					<ContentTemplate>
						BODY
					</ContentTemplate>
				</bc:CollapsiblePanel>
			</ContentTemplate>
		</asp:UpdatePanel>

		<hr />

		<br/>
		<br/>

		<h2>Test repeater - AsyncPostBack:</h2>

		<asp:Repeater ID="TestUpdatePanelRepeater3" runat="server">
		<ItemTemplate>
			<asp:UpdatePanel runat="server">
				<ContentTemplate>
					<bc:CollapsiblePanel ID="TestUpdatePanelCollapsiblePanel3" AutoPostBack="True" runat="server">
						<HeaderTemplate>
							<h1 class="panel-title" runat="server">HEADER</h1>							
						</HeaderTemplate>
						<ContentTemplate>
							BODY
						</ContentTemplate>
					</bc:CollapsiblePanel>
				</ContentTemplate>
			</asp:UpdatePanel>
	   </ItemTemplate >
	   </asp:Repeater >


		<hr />

		<br/>
		<br/>

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

		<h2>Collapsible panel test 2:</h2>

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