<%@ Page Title="" Language="C#" MasterPageFile="~/HavitWebBootstrapTests/Bootstrap.Master" StylesheetTheme="BootstrapTheme" CodeBehind="RetractablePanelTest2.aspx.cs" Inherits="Havit.WebApplicationTest.HavitWebBootstrapTests.RetractablePanelTestHelper" %>

<asp:Content ContentPlaceHolderID="MainCPH" runat="server">
	<style>
		div.Havit_RetractablePanel_Header {
			font-size: larger;
			font-weight: 600;
			background-color: indianred;
		}
	</style>

	<h1>Retractable panel test 3:</h1>

	<bc:RetractablePanel ID="RetractablePanel1RP" HeaderText="TEST" runat="server">
		<HeaderTemplate>
			<h1 style="background-color: indianred">
				<asp:Label ID="HeaderLabel" Text="Header text of the rectractable panel 3 - please click here" runat="server" />
			</h1>
		</HeaderTemplate>
		<ContentTemplate>
			<p>This context 3 is in retractable section...</p>
		</ContentTemplate>
	</bc:RetractablePanel>

	<br/>
	<br/>
	<br/>
	<br/>

	<h1>Retractable panel test 4:</h1>

	<bc:RetractablePanel ID="RetractablePanel4" HeaderText="Header text of the rectractable panel 4 - please click here" runat="server">
		<ContentTemplate>
			<p>This context 4 is in retractable section...</p>
		</ContentTemplate>
	</bc:RetractablePanel>

	<br/>
	<br/>
	<br/>
	<br/>

	<h1>Goto second page:</h1>
	<asp:Button ID="RetractablePanelTest2Btn" Text="RetractablePanelTest1" OnClick="GotoBtn_Click" runat="server" />

	<h1>Empty postback:</h1>
	<asp:Button ID="PostbackBtn" Text="Empty postback" OnClick="PostbackBtn_Click" runat="server" />

</asp:Content>