<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="ScriptletTest.aspx.cs" Inherits="WebApplicationTest.ScriptletTest" %>
<%@ Register Namespace="Havit.Web.UI.WebControls" TagPrefix="havit" Assembly="Havit.Web" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<havit:NumericBox ID="TestNumericBox" runat="server" />
		<havit:Scriptlet runat="server">		
			<havit:ControlParameter ControlName="TestNumericBox" runat="server" StartOnChange="true" />
			<havit:ClientScript startonload="true" runat="server">
				alert(parameters.TestNumericBox.value);
			</havit:ClientScript>
		</havit:Scriptlet>
    </div>
    </form>
</body>
</html>
