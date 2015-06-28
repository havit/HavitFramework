<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="ScriptletTest2.aspx.cs" Inherits="Havit.WebApplicationTest.HavitWebTests.ScriptletTest2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        	<asp:ScriptManager runat="server" />
					<asp:CheckBox ID="TestCheckBox" runat="server" />
					<havit:Scriptlet runat="server">		
						<havit:ControlParameter ControlName="TestCheckBox" runat="server" StartOnChange="true" />
						<havit:ClientScript startonload="true" StartOnAjaxCallback="true" runat="server">
							alert(parameters.TestCheckBox.checked);
						</havit:ClientScript>
					</havit:Scriptlet>		

    </div>
    </form>
</body>
</html>
