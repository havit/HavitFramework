<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="CheckBoxValidatorTest.aspx.cs" Inherits="WebApplicationTest.HavitWebTests.CheckBoxValidatorTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
</head>
<body>
	<form id="form1" runat="server">
		<div>
			<asp:Label ID="StampLb" runat="server" /><br />
			<asp:CheckBox ID="TestCheckBox" runat="server" />
			<havit:CheckBoxValidator ControlToValidate="TestCheckBox" Text="Chyba" runat="server" />
			<asp:Button Text="Submit" runat="server" />
		</div>
		<br />
		<br />
		<br />
		<div>
			<%--			<asp:TextBox ID="TestTextBox" runat="server" />
			<asp:RequiredFieldValidator ControlToValidate="TestTextBox" Text="Chyba" runat="server" />--%>
		</div>
	</form>
</body>
</html>