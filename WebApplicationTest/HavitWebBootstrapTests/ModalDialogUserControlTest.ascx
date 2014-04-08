<%@ Control Language="C#" CodeBehind="ModalDialogUserControlTest.ascx.cs" Inherits="WebApplicationTest.HavitWebBootstrapTests.ModalDialogUserControlTest" %>

<bc:ModalDialog HeaderText="My cool dialog!" runat="server">
	<ContentTemplate>	
		Hello! This is my first cool dialog.
	</ContentTemplate>
	<FooterTemplate>
		<asp:Button ID="CloseButton" Text="Close" runat="server" />
		<asp:Button Text="PostBack" runat="server" />
	</FooterTemplate>	
</bc:ModalDialog>