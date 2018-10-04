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
		<Triggers>
			<asp:PostBackTrigger ControlID="SecondSwitchButton" runat="server" />
		</Triggers>
		<ContentTemplate>
			<bc:SwitchButton ID="SecondSwitchButton" AutoPostBack="true" CausesValidation="true" ValidationGroup="Some" runat="server" />
			<asp:TextBox ID="SomeTextBox" runat="server" />
			<asp:RequiredFieldValidator ControlToValidate="SomeTextBox" ValidationGroup="Some" Text="*" runat="server" />
			<asp:Label ID="SecondStateLabel" runat="server" />
		</ContentTemplate>
	</asp:UpdatePanel>

</asp:Content>
