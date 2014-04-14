<%@ Control Language="C#" CodeBehind="ModalDialogUserControlTest.ascx.cs" Inherits="WebApplicationTest.HavitWebBootstrapTests.ModalDialogUserControlTest" %>

<bc:ModalDialog ID="MainModalDialog" HeaderText="My cool dialog!" runat="server">
	<ContentTemplate>	
		Hello! This is my first cool dialog.
		<br/>
		<asp:CheckBox ID="MyCheckBox" runat="server" />
		<havit:DateTimeBox runat="server"/>
	</ContentTemplate>
	<FooterTemplate>
		<asp:Button ID="CloseButton" Text="Close" runat="server" />
		<asp:Button Text="Refresh" runat="server" />
	</FooterTemplate>	
</bc:ModalDialog>