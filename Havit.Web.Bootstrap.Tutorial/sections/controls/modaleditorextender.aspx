<%@ Page Title="" Language="C#" MasterPageFile="~/Templates/Main.Master" %>
<%@ Register TagPrefix="uc" TagName="ModalDialogUserControlBaseSample" Src="~/sections/controls/samples/ModalDialogUserControlBaseSample.ascx" %>
<%@ Register TagPrefix="uc" TagName="ModalDialogSample" Src="~/sections/controls/samples/ModalDialogSample.ascx" %>

<asp:Content ContentPlaceHolderID="TopCPH" runat="server">
	<h1>ModalEditorExtender</h1>
	<p>	<p>Inline editing replacement - edit in ModalDialog</p>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainCPH" runat="server">
	
	<h2>ModalEditorExtender</h2>

	<p>Editor extender attaches to a "For" controls and is used as item editor instead of standard control edit mode. Currently, only GridViewExt and EnterpriseGridView are supported as target controls. EnterpriseGridView with AutoCrudOperations ensures object changes persistence.</p>
	<h3>Properties</h3>
	<ul>
		<li>For</li>
		<li>HeaderText, HeaderTemplate, ContentTemplate, FooterTemplate</li>
	</ul>

	<h3>Methods</h3>
	<ul>
		<li>StartEditor</li>
		<li>CloseEditor</li>
		<li>ExtractValues</li>
		<li>Save</li>
	</ul>

	<h3>Events</h3>
	<ul>
		<li>ItemCreated, ItemDataBound</li>
		<li>ItemSaving, ItemSaved</li>
		<li>EditorClosed</li>
		<li>GetEditedObject</li>
	</ul>
	
	<h3>Event propagation</h3>
	Events Save, OK and Cancel are handled.

	<h3>Skin support</h3>
	FooterTemplate should be set in skin. It should use buttons with command names OK, Save, Cancel which are handled. No codebehind for buttons required.

</asp:Content>
