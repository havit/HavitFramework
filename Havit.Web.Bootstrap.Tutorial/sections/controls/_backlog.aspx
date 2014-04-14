<%@ Page Title="" Language="C#" MasterPageFile="~/Templates/Main.Master" %>
<%@ Register TagPrefix="uc" TagName="ValidatorSample" Src="~/sections/controls/samples/ValidatorSample.ascx" %>

<asp:Content ContentPlaceHolderID="TopCPH" runat="server">
	<h1>Backlog</h1>
	<p>For future development</p>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainCPH" runat="server">
	<h2>Controls</h2>
	<ul>
		<li>Menu (MenuAdapter?)</li>
		<li>NumericBox</li>
		<li>NumericBoxValidator</li>
		<li>DateTimeBox</li>
		<li>DateTimeBoxValidator</li>
		<li>DateRangeBox</li>
		<li>DateRangeBoxValidator</li>
		<li>GridView pagination</li>
		<li>...</li>
	</ul>
	<h2>Scripts</h2>
	<ul>
		<li>Do not embed in assembly, distribute in NuGet</li>
		<li>...</li>
	</ul>
</asp:Content>
