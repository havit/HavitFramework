<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DateTimeBoxTest.aspx.cs" Inherits="WebApplicationTest.DateTimeBoxTest" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
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
						<td><havit:DateTimeBox runat="server" /></td>
					</tr>
					<tr>
						<td>DateTime</td>
						<td><havit:DateTimeBox DateTimeMode="DateTime" runat="server" /></td>
					</tr>
					<tr>
						<td>Enabled=false</td>
						<td><havit:DateTimeBox runat="server" Enabled="false" /></td>
					</tr>
					<tr>
						<td>in disabled panel</td>
						<td>
							<asp:Panel Enabled="false" runat="server">
								<havit:DateTimeBox runat="server" />
							</asp:Panel>
						</td>
					</tr>
				</table>
				
				<asp:Button runat="server" text="Callback" />
			</ContentTemplate>
		</asp:UpdatePanel>
		
    </div>
    </form>
</body>
</html>
