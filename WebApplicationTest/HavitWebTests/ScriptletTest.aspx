<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="ScriptletTest.aspx.cs" Inherits="Havit.WebApplicationTest.HavitWebTests.ScriptletTest" %>
<%@ Register Namespace="Havit.Web.UI.WebControls" TagPrefix="havit" Assembly="Havit.Web" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<havit:DateTimeBox ID="TestDateTimeBox" runat="server" />
		<asp:LinkButton ID="MyLinkButton" Text="LB" runat="server" OnClientClick="return false;" />
		<havit:Scriptlet runat="server">		
			<havit:ControlParameter ControlName="TestDateTimeBox" runat="server" StartOnChange="true" />
			<havit:ControlParameter ControlName="MyLinkButton" runat="server" StartOnChange="true" />
			<havit:ClientScript runat="server">
				alert(parameters.TestDateTimeBox.value);
			</havit:ClientScript>
		</havit:Scriptlet>
    </div>
    </form>
</body>
</html>
