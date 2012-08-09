<%@ Page Language="C#" AutoEventWireup="true" Codebehind="NumericBoxTest.aspx.cs"
	Inherits="WebApplicationTest.NumericBoxTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Untitled Page</title>
</head>
<body>
	<form id="form1" runat="server">
		<div>
			<asp:ScriptManager runat="server" />
			<asp:UpdatePanel runat="server">
				<ContentTemplate>
					<table>
						<tr>
							<td></td>
							<td><havit:NumericBox runat="server" /></td>
						</tr>
						<tr>
							<td>AllowNegativeNumber</td>
							<td><havit:NumericBox AllowNegativeNumber="true" runat="server" /></td>
						</tr>
						<tr>
							<td>Decimals=2</td>
							<td><havit:NumericBox Decimals="2" runat="server" /></td>
						</tr>
						<tr>
							<td>AllowNegativeNumber, Decimals=2</td>
							<td><havit:NumericBox AllowNegativeNumber="true" Decimals="2" runat="server" /></td>
						</tr>
					</table>
					<asp:Button runat="server" text="Callback" />
				</ContentTemplate>
			</asp:UpdatePanel>
		</div>
	</form>
</body>
</html>
