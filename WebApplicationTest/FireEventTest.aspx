<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FireEventTest.aspx.cs" Inherits="WebApplicationTest.FireEventTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<input type="text" id="MyTextBox" onchange="alert('onchanged');"/>
		<button id="MyButton" onclick="if (document.getElementById('MyTextBox').fireEvent) { document.getElementById('MyTextBox').fireEvent('onchange'); } else { alert('not supported'); }">test it</button>
    </div>
    </form>
</body>
</html>
