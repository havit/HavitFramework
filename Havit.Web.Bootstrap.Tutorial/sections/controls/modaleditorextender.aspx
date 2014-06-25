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
	
	<div class="alert-info">
		<blockquote>
			<small>
				ModalEditorExtender inherits from DataBoundControl class. It ensures functionality of strong type databinding with Intellisence in Visual Studio. But there is no support for properties and methods of DataBoundControls and its ancesors.
				I.e. properties BackColor, CssClass, Height, etc. have no meaning and does not have any impact to rendered code. Unused members are hidden for intellisence by EditorBrowsableAttributes and DesignerSerializationVisibilityAttributes.
			</small>
		</blockquote>
	</div>

	<h3>Properties</h3>
	<ul>
		<li>TargetControlID</li>
		<li>HeaderText, HeaderTemplate, ContentTemplate, FooterTemplate</li>
		<li>ValidationGroup</li>
		<li>next properties from nested <a href="modaldialog">ModalDialog</a> control</li>
	</ul>

	<h3>Methods</h3>
	<ul>
		<li>StartEditor</li>
		<li>CloseEditor</li>
		<li>ExtractValues</li>
		<li>Save</li>
		<li>NavigateNext, NavigatePrevious</li>
	</ul>

	<h3>Events</h3>
	<ul>
		<li>ItemCreated, ItemDataBound</li>
		<li>ItemSaving, ItemSaved</li>
		<li>PreviousNavigating, PreviousNavigated, NextNavigating, NextNavigated</li>
		<li>EditorClosed</li>
		<li>GetSavedObject</li>
	</ul>
	
	<h3>Event propagation</h3>
	<p>Events Save, OK, Cancel, Next, Previous are handled.</p>

	<h3>Skin support</h3>
	<p>FooterTemplate should be set in skin. It should use buttons with command names OK, Save, Cancel which are handled (No codebehind for buttons required). Be careful when setting ValidationGroup.</p>

	<uc:ShowControl Title="Show demo code" Filename="~/Sections/Controls/Samples/ModalEditorExtenderSample.txt" runat="server" />

</asp:Content>
