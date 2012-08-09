<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ScriptletTest3.aspx.cs" Inherits="WebApplicationTest.ScriptletTest3" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    	<havit:NumericBox ID="TestNumericBox" runat="server" />
		<havit:Scriptlet ID="Scriptlet1" runat="server">		
			<havit:ControlParameter ID="ControlParameter1" ControlName="TestNumericBox" runat="server" StartOnChange="true" />
			<havit:ClientScript ID="ClientScript1" startonload="true" runat="server">
				
			</havit:ClientScript>
		</havit:Scriptlet>

		<havit:Scriptlet ID="Scriptlet2" runat="server">		
			<havit:ControlParameter ID="ControlParameter2" ControlName="TestNumericBox" runat="server" StartOnChange="true" />
			<havit:ClientScript ID="ClientScript2" startonload="true" runat="server">
				alert(parameters.TestNumericBox.value);
			</havit:ClientScript>
		</havit:Scriptlet>
    </div>
    </form>
</body>
</html>
