<%@ Page Title="" Language="C#" MasterPageFile="~/Templates/Main.Master" %>
<%@ Register Src="~/sections/controls/samples/CollapsiblePanelSample1.ascx" TagPrefix="uc" TagName="CollapsiblePanelSample1" %>
<%@ Register Src="~/sections/controls/samples/CollapsiblePanelSample2.ascx" TagPrefix="uc" TagName="CollapsiblePanelSample2" %>

<asp:Content ContentPlaceHolderID="TopCPH" runat="server">
	<h1>CollapsiblePanel</h1>
	<p>The CollapsiblePanel is a very flexible extender that allows you to easily add collapsible sections to your web page.</p>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainCPH" runat="server">
	<h2>Appearance</h2>
	<p>Displays collapsible panel.</p>

	<h2>Properties, Events</h2>
	<ul>
		<li>Collapsed - Specifies that the object is collapsed or expanded.</li>
		<li>AutoPostBack - Gets or sets a value that indicates whether an automatic postback to the server occurs when user click to panel.</li>
		<li>HeaderText - The text to show in the control when the panel is collapsed.</li>
	</ul>

	<h2>Sample 1 - collapsible panel with 'HeaderTemplate'</h2>	
	<uc:CollapsiblePanelSample1 ID="CollapsiblePanelSample1UC" runat="server" />
	<uc:ShowControl ShowControlID="CollapsiblePanelSample1UC" runat="server" />

	<h2>Sample 2 - collapsible panel without 'HeaderTemplate'</h2>	
	<uc:CollapsiblePanelSample2 ID="CollapsiblePanelSample2UC" runat="server" />
	<uc:ShowControl ShowControlID="CollapsiblePanelSample2UC" runat="server" />
</asp:Content>
