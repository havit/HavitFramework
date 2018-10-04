<%@ Control Language="C#" Inherits="Havit.Web.Bootstrap.UI.WebControls.ModalDialogUserControlBase"  %>

<style>	
	.modal-blue {
		background-color: #c9e6ff;
	}
</style>

<bc:ModalDialog HeaderCssClass="modal-blue" runat="server">
	<HeaderTemplate>Drag Me!</HeaderTemplate>
	<ContentTemplate>
		<p>Be carefull with custom styling. This demo shows different modal appereance when used custom background color:</p>
		<ol>
			<li>There are no more rounded corners.</li>
			<li>In Internet Explorer right side of the dialog does not fit with dialog shadow.</li>
		</ol>
	</ContentTemplate>		
	<FooterTemplate>Try escape key!</FooterTemplate>
</bc:ModalDialog>

