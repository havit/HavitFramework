<%@ Page Language="C#" CodeBehind="TestPage.aspx.cs" Inherits="Havit.DsvCommerce.Web.TestPage" Title="Untitled Page" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
	<link rel="Stylesheet" href="havit.css" type="text/css" />
	<script src="havitScripts.js" language="javascript"></script>
</head>
<body>
    <form runat="server">
    <div>
       	<asp:ScriptManager runat="server" />

	<asp:Button runat="server" Text="Refresh" />

	<div>MyBasicModalDialog</div>

	<asp:Button runat="server" Text="Client open" OnClientClick='<%# MyBasicModalDialog.GetShowScript() + "return false;" %>' />
	<asp:Button runat="server" Text="Server open" ID="ServerOpen1Button" />

	<havit:BasicModalDialog ID="MyBasicModalDialog" runat="server" Width="200" Height="200">
		<ContentTemplate>
			<div>
		<asp:Button runat="server" Text="Client close" OnClientClick='<%# MyBasicModalDialog.GetHideScript() + "return false;" %>' />
		<asp:Button runat="server" Text="Server close" ID="ServerClose1Button" />
		<asp:Button runat="server" Text="Refresh" />
			<%= DateTime.Now %>
			Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.
			</div>
		</ContentTemplate>
		
	</havit:BasicModalDialog>

	<div>MyWebModalDialog</div>

	<asp:Button runat="server" Text="Client open" OnClientClick='<%# MyAjaxModalDialog.GetShowScript() + "return false;" %>' />
	<asp:Button runat="server" Text="Server open" ID="ServerOpen2Button" />

	<havit:AjaxModalDialog ID="MyAjaxModalDialog" runat="server" Width="200" Height="200">
		<ContentTemplate>
		<asp:Button runat="server" Text="Client close" OnClientClick='<%# MyAjaxModalDialog.GetHideScript() + "return false;" %>' />
		<asp:Button runat="server" Text="Server close" ID="ServerClose2Button" />
		<asp:Button runat="server" Text="Refresh" />
			<%= DateTime.Now %>
		</ContentTemplate>
		<Triggers>
			<asp:AsyncPostBackTrigger ControlID="ServerOpen2Button" />
		</Triggers>
	</havit:AjaxModalDialog>



    </div>
    </form>
</body>
</html>

