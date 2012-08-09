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
		<asp:ScriptManager EnablePageMethods="true" runat="server" />
		<div>
			<asp:TextBox ID="SubjektTB" Width="150" runat="server" style="border: 1px solid black;" />
			<havit:AutoSuggestMenu
				ID="SubjektASM"
				TargetControlID="SubjektTB"
				OnGetSuggestions="GetSuggestions"
				runat="server"
			/>
		</div>
		<br /><br />
		<div>
		    <asp:Button ID="ShowDialogButton" text="Zobraz dialog" runat="server" />
		    <havit:AjaxModalDialog ID="TestDialog" Width="300" Height="200" runat="server">
		        <ContentTemplate>
		        
			        <asp:TextBox ID="DialogSubjektTB" Width="150" runat="server" style="border: 1px solid black;"  />
			        <havit:AutoSuggestMenu
				        ID="DialogSubjektASM"
				        TargetControlID="DialogSubjektTB"
				        OnGetSuggestions="GetSuggestions"
				        runat="server"
		            />
		            
		        </ContentTemplate>
		    </havit:AjaxModalDialog>
		</div>
	</form>
</body>
</html>
