<%@ Page Title="" Language="C#" MasterPageFile="~/HavitWebBootstrapTests/Bootstrap.Master" AutoEventWireup="true" CodeBehind="AutoCompleteTextBox.aspx.cs" Inherits="WebApplicationTest.HavitWebBootstrapTests.AutoCompleteTextBox" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainCPH" runat="server">

	<script src="/HavitWebTests/jquery.autocomplete.min.js"></script>

	<style type="text/css">
		.container {
			max-width: 730px;
		}

		.autocomplete-suggestions {
			border: 1px solid #999;
			background: #FFF;
			overflow: auto;
		}

		.autocomplete-suggestion {
			padding: 2px 5px;
			white-space: nowrap;
			overflow: hidden;
			cursor: pointer;
		}

		.autocomplete-selected {
			background: #F0F0F0;
		}

		.autocomplete-suggestions strong {
			font-weight: normal;
			color: #3399FF;
		}

		.autocomplete-group {
			padding: 2px 5px;
		}

			.autocomplete-group strong {
				display: block;
				border-bottom: 1px solid #000;
			}
	</style>

	<div class="container">
		<div class="row">
			<div class="col-xs-5">
				<asp:Label Text="Pokus našeptávače" CssClass="control-label" runat="server" />
			</div>
			<div class="col-xs-5">
				<havit:AutoCompleteTextBox
					ID="Test1ACTB"
					ServiceUrl="/HavitWebTests/AutoCompleteTextBoxService.asmx/GetSuggestions"
					AutoPostBack="false"
					TextBoxStyle-CssClass="form-control" MaxHeight="100"
					runat="server" />
			</div>
			<div class="col-xs-2">
				<bc:Button ID="ButtonBt" CssClass="btn btn-default" Text="Text" runat="server" />
			</div>
		</div>
	</div>
</asp:Content>