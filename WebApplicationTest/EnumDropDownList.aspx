<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EnumDropDownList.aspx.cs" Inherits="WebApplicationTest.EnumDropDownList_aspx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<havit:EnumDropDownList ID="TestDDL" EnumType="<%$ Expression: typeof(WebApplicationTest.TestEnum) %>" DataTextFormatString="$resources: TestEnum, {0}" Nullable="true" runat="server" />
		<asp:Label ID="TestLb" runat="server" />
		<asp:Button ID="TestBt" Text="Test" runat="server" />
    </div>
    </form>
</body>
</html>
