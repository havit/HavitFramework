<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FilePageStatePersisterTest.aspx.cs" Inherits="Havit.WebApplicationTest.HavitWebTests.FilePageStatePersisterTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
			<asp:Button Text="Postback" runat="server" />
			<asp:Button Text="Deleted old anonymous files" ID="DeleteOldAnonymousFilesButton" runat="server" />
        </div>
    </form>
</body>
</html>
