<%@ Page Language="C#" Codebehind="AutoSuggestMenu.aspx.cs" Inherits="WebApplicationTest.AutoSuggestMenu_aspx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Untitled Page</title>
	<link rel="Stylesheet" href="havit.css" type="text/css" />
	<script src="havitScripts.js" language="javascript"></script>
</head>
<body>
	<form id="form1" runat="server">
		<asp:ScriptManager EnablePageMethods="true" ID="ScriptManager1" AllowCustomErrorsRedirect="false" runat="server" />
		<div>
			<asp:Label ID="TimestampLabel" runat="server" />
		</div>
		<div>
			<asp:Label ID="SubjektLabel" runat="server" />
		</div>
		<div>
			<asp:TextBox ID="SubjektTB" Width="150" runat="server" style="border: 1px solid black;" />
			<havit:AutoSuggestMenu
				ID="SubjektASM"
				TargetControlID="SubjektTB"
				ServicePath="~/AutoSuggestMenuService.asmx"
				KeyPressDelay="300"	
				UsePaging="true"
				PageSize="10"	
				MinSuggestChars="2"							
				OnGetSuggestions="WebApplicationTest.AutoSuggestMenuService.GetSuggestions"
				Mode="ClearTextOnNoSelection"
				AutoPostBack="true"
				UsePageMethods="false"
				runat="server"
			/>
			<asp:Button Text="Postback" runat="server" />
		</div>
		<br />
		<div>
			<textarea id="txtTrace" style="width: 100%; height: 600px;"></textarea>
		</div>
	</form>
</body>
</html>
