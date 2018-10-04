<%@ Page Title="" Language="C#" MasterPageFile="~/Templates/Main.Master" %>
<%@ Register TagPrefix="uc" TagName="TabsSimpleSample" Src="~/sections/controls/samples/TabsSimpleSample.ascx" %>
<%@ Register TagPrefix="uc" TagName="TabsInRepeaterSample" Src="~/sections/controls/samples/TabsInRepeaterSample.ascx" %>
<%@ Register TagPrefix="uc" TagName="TabsAppearanceSample" Src="~/sections/controls/samples/TabsAppearanceSample.ascx" %>

<asp:Content ContentPlaceHolderID="TopCPH" runat="server">
	<h1>Tabs</h1>
	<p>ASP.NET control for Bootstrap Tabs</p>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainCPH" runat="server">
	<h2>Appearance</h2>
	ASP.NET Control for displaying <a href="http://getbootstrap.com/components/#nav">Bootstrap Tabs &amp; Pills</a>.
	Tabs can be generated dynamicaly, ie. by Repeater control.

	<h3>TabContainer Properties, Events</h3>
	<ul>
		<li>TabMode - <a href="http://getbootstrap.com/components/#nav">Bootstrap Tabs &amp; Pills</a></li>
		<li>Justified - see <a href="http://getbootstrap.com/components/#nav">Bootstrap Tabs &amp; Pills</a></li>
		<%--<li>AutoPostBack</li>--%>
		<li>UseAnimations (default True)</li>
		<li>ActiveTabPanel</li>
		<li>ActiveTabPanelChanged (event)</li>
	</ul>
	
	<h3>TabPanel Properties</h3>
	<ul>
		<li>Active</li>
		<li>HeaderText, HeaderTemplate</li>
		<li>ContentTemplate</li>
		<li>Enabled</li>
	</ul>
	
	<h2>Sample #1 - Simple Tabs</h2>
	<uc:TabsSimpleSample id="TabsSimpleSampleUC" runat="server" />
	<uc:ShowControl ShowControlID="TabsSimpleSampleUC" runat="server" />

	<h2>Sample #2 - Tabs in Repeater</h2>
	<uc:TabsInRepeaterSample id="TabsInRepeaterSampleUC" runat="server" />
	<uc:ShowControl ShowControlID="TabsInRepeaterSampleUC" runat="server" />

	<h2>Sample #3 - Appearance</h2>
	<uc:TabsAppearanceSample id="TabsAppearanceSampleUC" runat="server" />
	<uc:ShowControl ShowControlID="TabsAppearanceSampleUC" runat="server" />

</asp:Content>
