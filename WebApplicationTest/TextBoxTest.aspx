<%@ Page Language="C#" Codebehind="TextBoxTest.aspx.cs" Inherits="WebApplicationTest.TextBoxTest_aspx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Untitled Page</title>
	<link rel="Stylesheet" href="havit.css" type="text/css" />
	<script src="havitScripts.js" language="javascript"></script>
</head>
<body>
	<form id="form1" runat="server">
		<asp:ScriptManager EnablePageMethods="true" ID="ScriptManager1" runat="server"/>
			<asp:TextBox ID="TextBox1" Width="150" runat="server" style="border: 1px solid black;" /><asp:Label ID="Label1" runat="server"/><asp:Button ID="PostbackButton" Text="Postback" runat="server" /><br/>
			<asp:TextBox ID="TextBox2" Width="150" AutoPostBack="True" runat="server" style="border: 1px solid black;" /><asp:Label ID="Label2" runat="server"/> 
			<havit:NumericBox AutoPostBack="True" ID="NB" runat="server" />
			<asp:TextBox TextMode="MultiLine" MaxLength="25" Text="Multiline maxlegth" runat="server" />
		</div>
	</form>
</body>
</html>
