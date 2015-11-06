<%@ Page Title="" Language="C#" MasterPageFile="~/HavitWebBootstrapTests/Bootstrap.Master" StylesheetTheme="BootstrapTheme" CodeBehind="RetractablePanelTest1.aspx.cs" Inherits="Havit.WebApplicationTest.HavitWebBootstrapTests.RetractablePanelTest" %>

<asp:Content ContentPlaceHolderID="MainCPH" runat="server">
	<style>
		div.Havit_RetractablePanel_Header {
			font-size: larger;
			font-weight: 600;
			background-color: indianred;
		}
	</style>

	<h1>Retractable panel test 1:</h1>

	<bc:RetractablePanel ID="RetractablePanel1RP" HeaderText="TEST" runat="server">
		<HeaderTemplate>
			<h1 style="background-color: indianred">
				<asp:Label ID="HeaderLabel" Text="Header text of the rectractable panel 1 - please click here" runat="server" />
			</h1>
		</HeaderTemplate>
		<ContentTemplate>
			<p>This context 1 is in retractable section...</p>
		</ContentTemplate>
	</bc:RetractablePanel>

	<br/>
	<br/>
	<br/>
	<br/>

	<h1>Retractable panel test 2:</h1>

	<bc:RetractablePanel ID="RetractablePanel2" HeaderText="Header text of the rectractable panel 2 - please click here" runat="server">
		<ContentTemplate>
			<p>This context 2 is in retractable section...</p>
		</ContentTemplate>
	</bc:RetractablePanel>

	<br/>
	<br/>
	<br/>
	<br/>

	<h1>Goto second page:</h1>
	<asp:Button ID="RetractablePanelTest1Btn" Text="RetractablePanelTest2" OnClick="GotoBtn_Click" runat="server" />

	<h1>Empty postback:</h1>
	<asp:Button ID="PostbackBtn" Text="Empty postback" OnClick="PostbackBtn_Click" runat="server" />

</asp:Content>