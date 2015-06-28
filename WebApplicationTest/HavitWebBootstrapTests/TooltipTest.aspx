<%@ Page Title="" Language="C#" MasterPageFile="~/HavitWebBootstrapTests/Bootstrap.Master" AutoEventWireup="false" CodeBehind="TooltipTest.aspx.cs" Inherits="Havit.WebApplicationTest.HavitWebBootstrapTests.Validators" %>

<asp:Content ContentPlaceHolderID="MainCPH" runat="server">
	<br/>
	<br/>
	<br/>
	<br/>

	<div class="container">			
		<bc:Tooltip ToolTip="Nahoře" TooltipPosition="Top" runat="server">Nahoře</bc:Tooltip>
		<bc:Tooltip ToolTip="Dole" TooltipPosition="Bottom" runat="server">Dole</bc:Tooltip>
		<bc:Tooltip ToolTip="Vlevo" TooltipPosition="Left" runat="server">Vlevo</bc:Tooltip>
		<bc:Tooltip ToolTip="Vpravo" TooltipPosition="Right" runat="server">Vpravo</bc:Tooltip>
		<bc:Tooltip ToolTip="Default" runat="server">Default</bc:Tooltip>
	</div>

</asp:Content>
