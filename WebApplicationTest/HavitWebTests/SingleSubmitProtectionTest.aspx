<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="SingleSubmitProtectionTest.aspx.cs" Inherits="Havit.WebApplicationTest.HavitWebTests.SingleSubmitProtectionTest" %>
<%@ OutputCache Duration="300" VaryByParam="*" %>

<%@ Register Namespace="Havit.Web.UI.WebControls" TagPrefix="havit" Assembly="Havit.Web" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title></title>
</head>
<body>
	<form runat="server">
		<div>
			<asp:ScriptManager runat="server">
				<Scripts>
					<asp:ScriptReference Name="jquery" />
					<%--Framework Scripts--%>
					<asp:ScriptReference Name="WebForms.js" Assembly="System.Web" Path="~/Scripts/WebForms/WebForms.js" />
					<asp:ScriptReference Name="WebUIValidation.js" Assembly="System.Web" Path="~/Scripts/WebForms/WebUIValidation.js" />
					<asp:ScriptReference Name="MenuStandards.js" Assembly="System.Web" Path="~/Scripts/WebForms/MenuStandards.js" />
					<asp:ScriptReference Name="GridView.js" Assembly="System.Web" Path="~/Scripts/WebForms/GridView.js" />
					<asp:ScriptReference Name="DetailsView.js" Assembly="System.Web" Path="~/Scripts/WebForms/DetailsView.js" />
					<asp:ScriptReference Name="TreeView.js" Assembly="System.Web" Path="~/Scripts/WebForms/TreeView.js" />
					<asp:ScriptReference Name="WebParts.js" Assembly="System.Web" Path="~/Scripts/WebForms/WebParts.js" />
					<asp:ScriptReference Name="Focus.js" Assembly="System.Web" Path="~/Scripts/WebForms/Focus.js" />
					<%--Site Scripts--%>
				</Scripts>
			</asp:ScriptManager>
			<havit:SingleSubmitProtection runat="server" />

            <asp:LinkButton ID="Link1Button" Text="Go to another page" runat="server" />
			<asp:UpdatePanel UpdateMode="Conditional" runat="server">
				<Triggers>
				</Triggers>
				<ContentTemplate>
					<asp:Button ID="TestButton" Text="Test" runat="server" />
					<asp:Button ID="Test2Button" Text="Not protected by single submit protectection" OnClientClick="<%$ Expression: SingleSubmitProtection.SetProcessingDisableJavaScript %>" runat="server" />
                    <asp:LinkButton ID="Link2Button" Text="Go to another page (async)" runat="server" />
				</ContentTemplate>
			</asp:UpdatePanel>
		</div>
	</form>
</body>
</html>
