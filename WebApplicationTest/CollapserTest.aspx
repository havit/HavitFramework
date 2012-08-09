<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CollapserTest.aspx.cs" Inherits="WebApplicationTest.CollapserTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
		<havit:Collapser ContentElement="CollapsiblePanel" runat="server">+/-</havit:Collapser>
		<asp:Panel ID="CollapsiblePanel" runat="server">
			Collapsible panel
		</asp:Panel>

		<asp:button runat="server" />
    </div>

    </form>
</body>
</html>
