<%@ Page Language="C#" CodeBehind="AutoSuggestMenu.aspx.cs" StyleSheetTheme="WebTheme" Inherits="WebApplicationTest.HavitWebTests.AutoSuggestMenu_aspx" %>

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
						OnGetSuggestions="WebApplicationTest.HavitWebTests.AutoSuggestMenuService.GetSuggestions"
						Mode="ClearTextOnNoSelection"
						MessageOnClearText="Hodnota stornována, nedošlo ke spárování hodnoty s číselníkovou hodnotou."
						AutoPostBack="false"
						UsePageMethods="false"
						AutoRegisterStyleSheets="true"
						runat="server" />
					<asp:Button Text="Postback" runat="server" />
				</div>
			</ContentTemplate>
		</asp:UpdatePanel>

		<div>
			<textarea id="txtTrace" style="width: 100%; height: 600px;"></textarea>
		</div>

		<havit:GridViewExt ID="MyGridView" ShowFilter="True" runat="server">
			<Columns>
				<havit:TemplateFieldExt ID="MyTemplateField">
					<ItemTemplate>
						<asp:TextBox ID="Subjekt2TB" Width="150" runat="server" Style="border: 1px solid black;" />
						<havit:AutoSuggestMenu
							ID="Subjekt2ASM"
							TargetControlID="Subjekt2TB"
							ServicePath="~/HavitWebTests/AutoSuggestMenuService.asmx"
							KeyPressDelay="300"
							UsePaging="true"
							PageSize="10"
							MinSuggestChars="2"
							OnGetSuggestions="WebApplicationTest.HavitWebTests.AutoSuggestMenuService.GetSuggestions"
							Mode="ClearTextOnNoSelection"
							MessageOnClearText="Hodnota stornována, nedošlo ke spárování s číselníkovou hodnotou."
							AutoPostBack="true"
							UsePageMethods="false"
							runat="server" />

					</ItemTemplate>

				</havit:TemplateFieldExt>
			</Columns>
		</havit:GridViewExt>

		<asp:PlaceHolder Visible="False" runat="server">
			<asp:TextBox ID="Subjekt3TB" Width="150" runat="server" Style="border: 1px solid black;" />
			<havit:AutoSuggestMenu
				ID="Subjekt3ASM"
				TargetControlID="Subjekt3TB"
				ServicePath="~/HavitWebTests/AutoSuggestMenuService.asmx"
				KeyPressDelay="300"
				UsePaging="true"
				PageSize="10"
				MinSuggestChars="2"
				OnGetSuggestions="WebApplicationTest.HavitWebTests.AutoSuggestMenuService.GetSuggestions"
				Mode="ClearTextOnNoSelection"
				MessageOnClearText="Hodnota stornována, nedošlo ke spárování s číselníkovou hodnotou."
				AutoPostBack="true"
				UsePageMethods="false"
				runat="server" />
		</asp:PlaceHolder>

	</form>
</body>
</html>
