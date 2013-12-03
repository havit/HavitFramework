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
							<td>
								<havit:NumericBox ID="Test1NumericBox" MaxLength="4" runat="server" />								
								<havit:NumericBoxValidator Text="error" ControlToValidate="Test1NumericBox" runat="server" />
							</td>
						</tr>
						<tr>
							<td>AllowNegativeNumber</td>
							<td>
								<havit:NumericBox ID="Test2NumericBox" AllowNegativeNumber="true" runat="server" />
								<havit:NumericBoxValidator Text="error" ControlToValidate="Test2NumericBox" runat="server" />
							</td>
						</tr>
						<tr>
							<td>Decimals=2</td>
							<td>
								<havit:NumericBox ID="Test3NumericBox" Decimals="2" runat="server" />
								<havit:NumericBoxValidator Text="error" ControlToValidate="Test3NumericBox" runat="server" />
							</td>
						</tr>
						<tr>
							<td>AllowNegativeNumber, Decimals=2</td>
							<td>
								<havit:NumericBox ID="Test4NumericBox" AllowNegativeNumber="true" Decimals="2" runat="server" />
								<havit:NumericBoxValidator Text="error" ControlToValidate="Test4NumericBox" runat="server" />
							</td>
						</tr>
					</table>
					<asp:Button ID="CallBackButton" CausesValidation="true" runat="server" text="Callback" />
				</ContentTemplate>
			</asp:UpdatePanel>
			<havit:FormViewExt ID="MyFormView" DefaultMode="Edit" ItemType="WebApplicationTest.NumericBoxTest.DataClass" runat="server">
				<EditItemTemplate>					
					<div>Integer Value: <havit:NumericBox ID="NB1" ValueAsInt="<%# BindItem.IntegerValue %>" runat="server" /></div>
					<div>Nullable Integer Value: <havit:NumericBox ID="NB2" Value="<%# BindItem.NullableIntegerValue %>" runat="server" /></div>
				</EditItemTemplate>
			</havit:FormViewExt>
			<asp:Button ID="ExtractButton" Text="Extract" runat="server" />
		</div>
	</form>
</body>
</html>
