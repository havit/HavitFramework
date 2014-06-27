<%@ Page Title="" Language="C#" MasterPageFile="~/Templates/Main.Master" %>
<%@ Register TagPrefix="uc" TagName="ModalDialogUserControlBaseSample" Src="~/sections/controls/samples/ModalDialogUserControlBaseSample.ascx" %>
<%@ Register TagPrefix="uc" TagName="ModalDialogSample" Src="~/sections/controls/samples/ModalDialogSample.ascx" %>

<asp:Content ContentPlaceHolderID="TopCPH" runat="server">
	<h1>ModalDialog/UserControlBase</h1>
	<p>	<p>ASP.NET control for Bootstrap Modals</p>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainCPH" runat="server">
	
	<h2>ModalDialogUserControlBase</h2>
	<p>ModalDialogUserControlBase should be used as base class of ASCX which behaves like modal dialog. This ASCX should contain exactly one ModalDialog.</p>
	<h3>Properties, Methods</h3>
	<ul>
		<li>Show, Hide</li>
		<li>DialogVisible</li>
	</ul>
	
	<h2>ModalDialog</h2>
	<p>Control which handles dialog appereance &amp; behavior. Dialog is set maximum height to be able to scroll the content of dialog without page scrollbar.</p>
	<p>Supports dialog in dialog (ie. record editing in modal via <a href="modaleditorextender">ModalEditorExtender</a> with &quot;something&quot; picker with dialog (see known issues).</p>
	<h3>Properties</h3>
	<ul>
		<li>HeaderTemplate, ContentTemplate, FooterTemplate</li>
		<li>HeaderText</li>
		<li>CssClass, HeaderCssClass, ContentCssClass, FooterCssClass</li>
		<li>ShowCloseButton (default True)</li>
		<li>CloseOnEscapeKey (default True)</li>
		<li>Width</li>
		<li>UseAnimations (default True)</li>
		<li>Triggers</li>
		<li>DragMode (default ModalDialogDragMode.IfAvailable)</li>
	</ul>
	
	<h4>DragMode</h4>
	<ul>
		<li>Required - Requires script resource definition "jquery.ui.combined" to register jQuery UI to enable dragging.</li>
		<li>IfAvailable - If script resource definition "jquery.ui.combined" exists or if jQuery UI is registered by any other way, dragging is enabled.</li>
		<li>No - No dragging enabled.</li>
	</ul>
	
	<h3>Known issues</h3>
	<ol>
		<li>Chrome displays <strong>nested</strong> modals inside parent modals. Add style <code>overflow: visible</code> to <code>.modal</code> css class to solve this issue.</li>
		<li>Dialog size is recalculated <strong>after</strong> dialog is shown. It causes a small flicker when dialog showing.</li>
	</ol>

	<h2>Sample</h2>
	<p><uc:ModalDialogUserControlBaseSample ID="ModalDialogUserControlBaseSampleUC" runat="server" /></p>
	<p><uc:ShowControl Text="Show ModalDialogUserControlBase Sample Code" Title="ModalDialogUserControlBase Sample" ShowControlID="ModalDialogUserControlBaseSampleUC" runat="server" /></p>
	<p><p><uc:ShowControl Text="Show ModalDialog Sample Code" Title="ModalDialog Sample" ShowControlID="ModalDialogSampleUC" runat="server" /></p>
	<uc:ModalDialogSample Visible="False" ID="ModalDialogSampleUC" runat="server" />
</asp:Content>
