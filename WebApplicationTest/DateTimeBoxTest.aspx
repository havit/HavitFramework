<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DateTimeBoxTest.aspx.cs" Inherits="WebApplicationTest.DateTimeBoxTest" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<havit:DateTimeBox runat="server" />
		
		<havit:DateTimeBox runat="server" Enabled="false" />
		
		<asp:Panel Enabled="false" runat="server">
		<havit:DateTimeBox runat="server" />
		</asp:Panel>
    </div>
    </form>
</body>
</html>
