<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="MenuTest.aspx.cs" Inherits="Havit.WebApplicationTest.SystemWebTests.MenuTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
		<asp:Menu runat="server" Orientation="Horizontal" DisappearAfter="1000">
			<Items>
				<asp:MenuItem Text="Tests">
				</asp:MenuItem>
				<asp:MenuItem Text="Scriptlet">
					<asp:MenuItem Text="ScriptletTest" NavigateUrl="~/ScriptletTest.aspx">
						<asp:MenuItem Text="ScriptletTest" NavigateUrl="~/bScriptletTest.aspx" />
						<asp:MenuItem Text="ScriptletTest2" NavigateUrl="~/bScriptletTest2.aspx" />
					</asp:MenuItem>
					<asp:MenuItem Text="ScriptletTest2" NavigateUrl="~/ScriptletTest2.aspx" />
					<asp:MenuItem Text="ScriptletAjaxTest">
						<asp:MenuItem Text="ScriptletTest" NavigateUrl="~/aScriptletTest.aspx" />
						<asp:MenuItem Text="ScriptletTest2" NavigateUrl="~/aScriptletTest2.aspx" />
					</asp:MenuItem>
				</asp:MenuItem>
				<asp:MenuItem Text="Scriptlet">
					<asp:MenuItem Text="ScriptletTest" NavigateUrl="~/xScriptletTest.aspx" />
					<asp:MenuItem Text="ScriptletTest2" NavigateUrl="~/xScriptletTest2.aspx" />
					<asp:MenuItem Text="ScriptletAjaxTest" NavigateUrl="~/xscriptletajaxtest.aspx" />
				</asp:MenuItem>
				<asp:MenuItem Text="Scriptlet">
					<asp:MenuItem Text="ScriptletTest" NavigateUrl="~/yScriptletTest.aspx" />
					<asp:MenuItem Text="ScriptletTest2" NavigateUrl="~/yScriptletTest2.aspx" />
					<asp:MenuItem Text="ScriptletAjaxTest" NavigateUrl="~/yscriptletajaxtest.aspx" />
				</asp:MenuItem>
				
			</Items>
		</asp:Menu>
		
    </div>    
    </form>
</body>
</html>
