<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HealthMonitoringTest.aspx.cs" Inherits="Havit.WebApplicationTest.SystemWebTests.HealthMonitoringTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager runat="server" />
    <havit:AjaxHealthMonitoring runat="server"/>
    <div>	    
		<asp:Button ID="DoExceptionButton" Text="Do Exception" runat="server"/>
		<asp:UpdatePanel runat="server">
			<ContentTemplate>
				<asp:Button ID="DoException2Button" Text="Do Exception in async postback" runat="server"/>
			</ContentTemplate>
		</asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
