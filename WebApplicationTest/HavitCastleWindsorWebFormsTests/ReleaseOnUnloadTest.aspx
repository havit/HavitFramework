<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReleaseOnUnloadTest.aspx.cs" Inherits="Havit.WebApplicationTest.HavitCastleWindsorWebFormsTests.ReleaseOnUnloadTest" %>

<%@ Register Src="~/HavitCastleWindsorWebFormsTests/ReleaseOnUnloadControlTest.ascx" TagPrefix="uc1" TagName="ReleaseOnUnloadControlTest" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<uc1:ReleaseOnUnloadControlTest id="ReleaseOnUnloadControlTestUC" runat="server"  />
	    Page: <div><asp:Label ID="MyLabel" runat="server"/></div>
    </div>
    </form>
</body>
</html>
