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
<%--		<asp:UpdatePanel runat="server">
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
		</asp:UpdatePanel>--%>
		
		
		<table>
		    <tr>
		        <td>AutoPostBack</td>
		        <td><havit:DateTimeBox ID="AutoPostBackDateTimeBox" CausesValidation="true" AutoPostBack="true" runat="server" /></td>
		    </tr>
		    <tr>
		        <td>OnInit</td>
		        <td><asp:Label ID="AutoPostBackOnInitValueLabel" runat="server" /></td>		        
		    </tr>
		    <tr>
		        <td>
		            Serverový validátor
		            <asp:CustomValidator ID="AutoPostBackDateTimeBoxValidator" runat="server" />
		        </td>
		        <td><asp:Label ID="AutoPostBackServerValidatorValueLabel" runat="server" /></td>		        
		    </tr>
		    <tr>
		        <td>OnLoad</td>
		        <td><asp:Label ID="AutoPostBackOnLoadValueLabel" runat="server" /></td>		        
		    </tr>		    		    
		    <tr>
		        <td>OnLoadComplete</td>
		        <td><asp:Label ID="AutoPostBackOnLoadCompleteValueLabel" runat="server" /></td>		        
		    </tr>
		    <tr>
		        <td>Changed</td>
		        <td><asp:Label ID="ChangedLabel" runat="server" /></td>		        
		    </tr>
		    
		</table>
    </div>
    </form>
</body>
</html>
