<%@ Page Language="C#" CodeBehind="AutoSuggestMenu.aspx.cs" StylesheetTheme="WebTheme" Inherits="Havit.WebApplicationTest.HavitWebTests.AutoSuggestMenu_aspx" %>

<%@ Register TagPrefix="uc" TagName="AutoSuggestMenuControl" Src="~/HavitWebTests/AutoSuggestMenuControl.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Untitled Page</title>
	<link rel="Stylesheet" href="havit.css" type="text/css" />
	<script src="havitScripts.js" language="javascript"></script>
</head>
<body>
	<form id="form1" runat="server">
		<asp:ScriptManager EnablePageMethods="true" ID="ScriptManager1" runat="server" />
		<asp:UpdatePanel runat="server">
			<ContentTemplate>
				<div>
					<asp:Label ID="TimestampLabel" runat="server" />
				</div>
				<div>
					<asp:Label ID="AsyncLabel" runat="server" />
				</div>

				<div>
					<asp:Label ID="SubjektLabel" runat="server" />
				</div>
				<div>
					<asp:TextBox ID="SubjektTB" Width="150" runat="server" Style="border: 1px solid black;" />
					<havit:AutoSuggestMenu
						ID="SubjektASM"
						TargetControlID="SubjektTB"
						ServicePath="~/HavitWebTests/AutoSuggestMenuService.asmx"
						KeyPressDelay="300"
						UsePaging="true"
						PageSize="10"
						MinSuggestChars="2"
						OnGetSuggestions="Havit.WebApplicationTest.HavitWebTests.AutoSuggestMenuService.GetSuggestions"
						Mode="ClearTextOnNoSelection"
						MessageOnClearText="Hodnota stornována, nedošlo ke spárování hodnoty s číselníkovou hodnotou."
						AutoPostBack="true"
						UsePageMethods="false"
						AutoRegisterStyleSheets="true"
						runat="server" />
					<asp:Button Text="Postback" runat="server" />
				</div>

				<asp:Repeater ID="MyRepeater" runat="server">
					<ItemTemplate>
						<uc:AutoSuggestMenuControl ID="AutoSuggestMenuControl" runat="server" />
					</ItemTemplate>
				</asp:Repeater>

				<havit:GridViewExt ID="MyGridView" ShowFilter="True" runat="server">
					<Columns>
						<havit:TemplateFieldExt ID="MyTemplateField">
							<ItemTemplate>
								<uc:AutoSuggestMenuControl ID="AutoSuggestMenuControl" runat="server" />
							</ItemTemplate>
						</havit:TemplateFieldExt>
					</Columns>
				</havit:GridViewExt>
			</ContentTemplate>
		</asp:UpdatePanel>

		<div>
			<textarea id="txtTrace" style="width: 100%; height: 600px;"></textarea>
		</div>

	</form>
</body>
</html>
