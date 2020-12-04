<!DOCTYPE html>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AutoCompleteTextBox.aspx.cs" Inherits="Havit.WebApplicationTest.HavitWebTests.AutoCompleteTextBox" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<link href="../Content/site.css" rel="stylesheet" runat="server" />
	<link rel="Stylesheet" href="/Content/Havit.Web.ClientContent/autocompletetextbox.css" type="text/css" />
</head>

<body>
	<form id="MainForm" runat="server">
		<div>

			<asp:ScriptManager EnablePartialRendering="true" runat="server">
				<Scripts>
					<asp:ScriptReference Name="jquery" />
					<asp:ScriptReference Name="jquery.ui.combined" />
					<%--Framework Scripts--%>
					<asp:ScriptReference Name="WebForms.js" Assembly="System.Web" Path="~/Scripts/WebForms/WebForms.js" />
					<asp:ScriptReference Name="WebUIValidation.js" Assembly="System.Web" Path="~/Scripts/WebForms/WebUIValidation.js" />
					<asp:ScriptReference Name="MenuStandards.js" Assembly="System.Web" Path="~/Scripts/WebForms/MenuStandards.js" />
					<asp:ScriptReference Name="GridView.js" Assembly="System.Web" Path="~/Scripts/WebForms/GridView.js" />
					<asp:ScriptReference Name="DetailsView.js" Assembly="System.Web" Path="~/Scripts/WebForms/DetailsView.js" />
					<asp:ScriptReference Name="TreeView.js" Assembly="System.Web" Path="~/Scripts/WebForms/TreeView.js" />
					<asp:ScriptReference Name="WebParts.js" Assembly="System.Web" Path="~/Scripts/WebForms/WebParts.js" />
					<asp:ScriptReference Name="Focus.js" Assembly="System.Web" Path="~/Scripts/WebForms/Focus.js" />
					<%--Site Scripts--%>
					<asp:ScriptReference Name="bootstrap" />
					<asp:ScriptReference Name="toastr" />
				</Scripts>
			</asp:ScriptManager>

			<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
				<ContentTemplate>
					<havit:AutoCompleteTextBox
						ID="TesterACTB"
						ServiceUrl="~/Services/AutoCompleteTextBoxService.svc/GetSuggestionsContext"
						UseClientCache="false"
						Orientation="Auto"
						Context="lorem ipsum dolor sit amet"
						AllowInvalidSelection="true"
						Nullable="true"
						ShowNoSuggestionNotice="True"
						PlaceHolderText="Sem napište text"
						NoSuggestionNotice="Nejsou data!"
						AutoPostBack="true"
						runat="server" />

					<%--OnClientSelectScript="test();"--%>
					<script type="text/javascript">
						function test() {
							var element = $("#test");
							var date = new Date();
							element.text(element.text() + date.getSeconds() + "*");
						}
					</script>

					<span id="test"></span>

					<asp:Button ID="ConfimBt" Text="Potvrdit" runat="server" />
					<asp:Label ID="PostbackValueLabel" runat="server" /><br />
					<asp:Label ID="PostbackTextLabel" runat="server" /><br />
					<asp:TextBox runat="server" /><br />
					<asp:Button ID="HideBt" Text="Skrýt" runat="server" />
					<asp:Button ID="ShowBt" Text="Zobrazit" runat="server" />
				</ContentTemplate>
			</asp:UpdatePanel>
			<br />
			<havit:AutoCompleteTextBox
				ID="AutoCompleteTextBox1"
				ServiceUrl="~/Services/AutoCompleteTextBoxService.svc/GetSuggestionsContext"
				UseClientCache="false"
				Orientation="Auto"
				Context="lorem ipsum dolor sit amet"
				AllowInvalidSelection="true"
				Nullable="true"
				ShowNoSuggestionNotice="True"
				PlaceHolderText="Sem napište text"
				NoSuggestionNotice="Nejsou data!"
				AutoPostBack="true"
				runat="server" />
			<br />
			<br />
			<button type="button" class="btn btn-primary" data-toggle="modal" data-target="#myModal">
				Modal dialog test
			</button>
		</div>

		<div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
			<div class="modal-dialog" role="document">
				<div class="modal-content">
					<div class="modal-header">
						<button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
						<h4 class="modal-title" id="myModalLabel">Modal test</h4>
					</div>
					<div class="modal-body">
						<havit:AutoCompleteTextBox
							ID="AutoCompleteTextBox2"
							ServiceUrl="~/Services/AutoCompleteTextBoxService.svc/GetSuggestionsContext"
							UseClientCache="false"
							Orientation="Auto"
							Context="lorem ipsum dolor sit amet"
							AllowInvalidSelection="true"
							Nullable="true"
							ShowNoSuggestionNotice="True"
							PlaceHolderText="Sem napište text"
							NoSuggestionNotice="Nejsou data!"
							runat="server" />
					</div>
					<div class="modal-footer">
						<button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
					</div>
				</div>
			</div>
		</div>
	</form>
</body>
</html>
