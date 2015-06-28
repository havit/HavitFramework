<%@ Page Title="" Language="C#" MasterPageFile="~/HavitWebBootstrapTests/Bootstrap.Master" AutoEventWireup="false" CodeBehind="SwitchButtonTest.aspx.cs" Inherits="Havit.WebApplicationTest.HavitWebBootstrapTests.SwitchButtonTest" %>
<asp:Content ContentPlaceHolderID="MainCPH" runat="server">
	<style>		
		.no.active { background: #ffc0cb; }		
		.yes.active { background: lightgreen; }
	</style>
	<div>
		<bc:SwitchButton ID="FirstSwitchButton" YesText="Ano" NoText="Ne" runat="server" />
		<br/>
		<asp:Label ID="FirstStateLabel" runat="server" />
		<br/>
		<asp:Button Text="Postback" runat="server" />
	</div>

	<asp:UpdatePanel UpdateMode="Conditional" runat="server">
		<ContentTemplate>
			<bc:SwitchButton ID="SecondSwitchButton" AutoPostBack="true" runat="server" />			
			<asp:Label ID="SecondStateLabel" runat="server" />
		</ContentTemplate>
	</asp:UpdatePanel>

</asp:Content>
