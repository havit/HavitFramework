<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="SingleSubmitProtectionTest.aspx.cs" Inherits="WebApplicationTest.HavitWebTests.SingleSubmitProtectionTest" %>
<%@ Register Namespace="Havit.Web.UI.WebControls" TagPrefix="havit" Assembly="Havit.Web" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form runat="server">
    <div>
		<asp:ScriptManager runat="server" />
		<havit:SingleSubmitProtection runat="server" />
		
		<asp:UpdatePanel UpdateMode="Conditional" runat="server">
		<Triggers>
		</Triggers>
		<ContentTemplate>
			<asp:Button ID="TestButton" Text="Test" runat="server" />
		</ContentTemplate>
		</asp:UpdatePanel>
    </div>
    </form>

</body>
</html>
