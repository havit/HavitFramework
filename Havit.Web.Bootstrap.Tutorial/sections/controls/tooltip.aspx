<%@ Page Title="" Language="C#" MasterPageFile="~/Templates/Main.Master" %>
<%@ Register TagPrefix="uc" TagName="TooltipSample" Src="~/sections/controls/samples/TooltipSample.ascx" %>

<asp:Content ContentPlaceHolderID="TopCPH" runat="server">
	<h1>ToolTip</h1>
	<p>ASP.NET wrapper for Bootstrap Tooltip</p>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainCPH" runat="server">
	<h2>Appearance</h2>
	<p>Displays text in ToolTip property as a <a href="http://getbootstrap.com/javascript/#tooltips">Bootstrap ToolTip</a>.</p>

	<h2>Properties</h2>
	<ul>
		<li>ToolTip</li>
		<li>Position (default ToolTipPosition.Top)</li>
	</ul>

	<h2>Sample</h2>	
	<uc:TooltipSample ID="TooltipSampleUC" runat="server" />
	<uc:ShowControl ShowControlID="TooltipSampleUC" runat="server" />
</asp:Content>
