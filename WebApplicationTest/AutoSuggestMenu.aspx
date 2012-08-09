<%@ Page Language="C#" Codebehind="AutoSuggestMenu.aspx.cs" Inherits="WebApplicationTest.AutoSuggestMenu_aspx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Untitled Page</title>
</head>
<body>
	<form id="form1" runat="server">
		<asp:ScriptManager EnablePageMethods="true" runat="server" />
		<div>
			<asp:TextBox ID="SubjektTB" Width="150" runat="server" />
			<havit:AutoSuggestMenu
				ID="SubjektASM"
				TargetControlID="SubjektTB"
				OnGetSuggestions="GetSuggestions"
				runat="server"
			/>
		</div>
	</form>
</body>
</html>
