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
		<li>CollapsedStateChanged (event) - Occurs when collaption state is changed.</li>
	</ul>

	<h2>Rendered HTML</h2>
	<pre>&lt;div class=&quot;panel panel-default&quot;&gt;
  &lt;div data-target=&quot;#{this.ClientID}&quot; data-toggle=&quot;collapse&quot; class=&quot;panel-heading&quot;&gt;
    [Panel heading]
  &lt;/div&gt;
  &lt;div id=&quot;#{this.ClientID}&quot; class=&quot;collapse panel-collapse&quot;&gt;
    &lt;div class=&quot;panel-body&quot;&gt;
      [Panel content]
    &lt;/div&gt;
  &lt;/div&gt;
&lt;/div&gt;</pre>

	<h2>Sample 1</h2>	
	<uc:CollapsiblePanelSample1 ID="CollapsiblePanelSample1UC" runat="server" />
	<uc:ShowControl ShowControlID="CollapsiblePanelSample1UC" runat="server" />

	<h2>Sample 2</h2>	
	<uc:CollapsiblePanelSample2 ID="CollapsiblePanelSample2UC" runat="server" />
	<uc:ShowControl ShowControlID="CollapsiblePanelSample2UC" runat="server" />
</asp:Content>
