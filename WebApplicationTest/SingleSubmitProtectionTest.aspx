<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SingleSubmitProtectionTest.aspx.cs" Inherits="WebApplicationTest.SingleSubmitProtectionTest" %>
<%@ Register Namespace="Havit.Web.UI.WebControls" TagPrefix="havit" Assembly="Havit.Web" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form runat="server">
    <div>
		<asp:ScriptManager runat="server" ScriptMode="Release" />
		<havit:SingleSubmitProtection runat="server" />
		
		<asp:Button ID="TestButton" Text="Test" runat="server" OnClientClick="<%$ Expression: SingleSubmitProtection.SetProcessingDisableJavaScript %>" />
		<asp:UpdatePanel runat="server" UpdateMode="Conditional">
		<Triggers>
		</Triggers>
		<ContentTemplate>
			<asp:Button Text="UpdatePanel" runat="server" />
		</ContentTemplate>
		</asp:UpdatePanel>
    </div>
    </form>

</body>
</html>
