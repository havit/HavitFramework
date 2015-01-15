<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="ScriptletTest4.aspx.cs" Inherits="WebApplicationTest.HavitWebTests.ScriptletTest4" %>
<%@ Register Namespace="Havit.Web.UI.WebControls" TagPrefix="havit" Assembly="Havit.Web" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
       <asp:ScriptManager runat="server"/>
    <div>
        <asp:UpdatePanel runat="server">
            <ContentTemplate>
	            <div>
	                <asp:Button ID="ShowButton" Text="Show" runat="server" />
					<asp:Button ID="HideButton" Text="Hide" Visible="false" runat="server" />
					<asp:Button Text="PostBack" runat="server" />
				</div>
                <asp:Panel ID="MyPanel" Visible="false" runat="server">
	                Scriptlet here...
                    <havit:Scriptlet runat="server">
			            <havit:ClientScript StartOnLoad="true" StartOnAjaxCallback="true" runat="server">
				            alert('scriptlet loaded');
			            </havit:ClientScript>
                    </havit:Scriptlet>
                </asp:Panel>                
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
