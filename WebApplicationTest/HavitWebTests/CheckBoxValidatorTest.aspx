<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="CheckBoxValidatorTest.aspx.cs" Inherits="WebApplicationTest.HavitWebTests.CheckBoxValidatorTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<asp:CheckBox ID="TestCheckBox" runat="server" />
		<havit:CheckBoxValidator ControlToValidate="TestCheckBox" Text="Chyba" runat="server" />
		<asp:Button Text="Submit" runat="server" />
    </div>
    </form>
</body>
</html>
