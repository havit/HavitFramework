<%@ Control Language="C#" CodeBehind="ModalDialogUserControlTest2.ascx.cs" Inherits="WebApplicationTest.HavitWebBootstrapTests.ModalDialogUserControlTest2" %>

<bc:ModalDialog ID="MainModalDialog" HeaderText="My cool dialog!" runat="server">
	<ContentTemplate>	
		Hello! This is my second cool dialog.
	</ContentTemplate>
	<FooterTemplate>
		<asp:Button ID="CloseButton" Text="Close" runat="server" />
		<asp:Button Text="Refresh Async PostBack" runat="server" />
		<asp:Button ID="RefreshPostBackButton" Text="Refresh PostBack" runat="server" />
	</FooterTemplate>	
</bc:ModalDialog>