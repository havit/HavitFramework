<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="SmtpTraceListenerTest.aspx.cs" Inherits="WebApplicationTest.SystemWebTests.SmtpTraceListenerTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<asp:Button ID="ExceptionCodeButton" Text="Exception in code" runat="server" />
		<asp:Button ID="ExceptionBackgroundThreadButton" Text="Exception in background thread" runat="server" />
		<asp:Button ID="ExceptionTaskButton" Text="Exception in task" runat="server" />
    </div>
    </form>
</body>
</html>
