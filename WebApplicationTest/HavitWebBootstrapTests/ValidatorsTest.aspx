<%@ Page Title="" Language="C#" MasterPageFile="~/HavitWebBootstrapTests/Bootstrap.Master" AutoEventWireup="false" CodeBehind="ValidatorsTest.aspx.cs" Inherits="WebApplicationTest.HavitWebBootstrapTests.ValidatorsTest" %>

<asp:Content ContentPlaceHolderID="MainCPH" runat="server">
	<style>
		.invalid + .tooltip > .tooltip-inner {
			border: 1px solid red;
			background: whitesmoke;
			color: black;
		}

		body {
			margin-left: 2em;
		}

		.invalid {
			border: 1px solid red;
		}
	</style>

	<asp:Panel runat="server">

		<h1>Validation Group A</h1>

		<bc:ValidationSummary ValidationGroup="A" runat="server" />

		1.
		<asp:TextBox ID="TB1" runat="server" />
		<bc:RequiredFieldValidator ControlToValidate="TB1" Text="Zadejte hodnotu do <i>prvního</i> texboxu (1)." ErrorMessage="Zadejte hodnotu do <i>prvního</i> texboxu (1)." ValidationGroup="A" runat="server" />
		<bc:RequiredFieldValidator ControlToValidate="TB1" Text="Zadejte hodnotu do <i>prvního</i> textboxu (2)." ErrorMessage="Zadejte hodnotu do <i>prvního</i> textboxu (2)." ValidationGroup="A" runat="server" />
		<bc:RegularExpressionValidator ControlToValidate="TB1" ValidationExpression="^\d{3,5}$" ErrorMessage="Zadejte tři až pět číslic." ValidationGroup="A" runat="server" />
		<br />

		2.
		<asp:TextBox ID="TB2" runat="server" />
		<bc:RequiredFieldValidator ControlToValidate="TB2" ErrorMessage="Zadejte hodnotu do druhého textboxu." ValidationGroup="A" runat="server" />
		<br />
		<br />

		<asp:Button Text="Postback" ValidationGroup="A" runat="server" />
	</asp:Panel>

	<asp:UpdatePanel runat="server">
		<ContentTemplate>

			<h1>Validation Group B with UpdatePanel</h1>

			<bc:ValidationSummary ValidationGroup="B" runat="server" />

			3.
			<asp:TextBox ID="TB3" runat="server" />
			<bc:RequiredFieldValidator ControlToValidate="TB3" ErrorMessage="Zadejte hodnotu do třetího textboxu." ValidationGroup="B" runat="server" />
			<br />

			4.
			<asp:TextBox ID="TB4" runat="server" />
			<bc:RequiredFieldValidator ControlToValidate="TB4" ErrorMessage="Zadejte hodnotu do čtvrtého textboxu." ValidationGroup="B" runat="server" />
			<bc:CustomValidator ControlToValidate="TB4" ErrorMessage="Hodnota druhého textboxu musí začínat písmenem A." ValidationGroup="B" OnServerValidate="CustomValidator_ServerValidate" TooltipPosition="Top" runat="server" />
			<br />
			<br />

			<asp:Button Text="Postback" ValidationGroup="B" runat="server" />

			<br />
			<br />

			<h1>Validation Group C</h1>
			<bc:ValidationSummary ValidationGroup="C" ShowMessageBox="True" runat="server" />
			<asp:TextBox ID="TB5" runat="server" />
			<bc:RequiredFieldValidator ControlToValidate="TB5" ErrorMessage="Zadejte hodnotu do pátého textboxu. \zpetne lomitko 'apostrof" ValidationGroup="C" EnableClientScript="false" runat="server" />
			<asp:Button Text="Postback" ValidationGroup="C" runat="server" />

		</ContentTemplate>
	</asp:UpdatePanel>

	<br />
	<br />

	<bc:Tooltip ToolTip="Tento tooltip by měl mít standardní barvu." runat="server">Normální tooltip.</bc:Tooltip>
</asp:Content>
