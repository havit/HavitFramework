<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DateTimeBoxTest.aspx.cs" Inherits="WebApplicationTest.DateTimeBoxTest" UnobtrusiveValidationMode="None" %>

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
								<havit:DateTimeBox ID="PrvniDTB" AutoPostBack="true" ValidationGroup="VG1" runat="server" />
								<asp:CompareValidator ControlToValidate="PrvniDTB" ValidationGroup="VG1" Operator="DataTypeCheck" Type="Date" runat="server" Text="error2" />
							</td>
						</tr>
						<tr>
							<td>DateTime</td>
							<td>
								<havit:DateTimeBox ID="DrubyDateTimeBox" FirstDayOfWeek="Sunday" KeyBlockingClientScriptEnabled="false" runat="server" />
								<asp:Button ID="VycistitButton" Text="Vycistit a schovat" runat="server" />
								<asp:Button ID="ZobrazitButton" Text="Zobrazit" runat="server" />
							</td>
						</tr>
						<tr>
							<td>Enabled=false</td>
							<td>
								<havit:DateTimeBox runat="server" Enabled="false" />
							</td>
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

					<asp:Button runat="server" Text="Callback" />
				</ContentTemplate>
			</asp:UpdatePanel>


			<table>
				<tr>
					<td>AutoPostBack</td>
					<td>
						<havit:DateTimeBox ID="AutoPostBackDateBox" CausesValidation="false" AutoPostBack="true" runat="server" />
						<br />
						<havit:DateTimeBox ID="AutoPostBackDateTimeBox" DateTimeMode="DateTime" CausesValidation="true" AutoPostBack="true" runat="server" />

					</td>
				</tr>
				<tr>
					<td>OnInit</td>
					<td>
						<asp:Label ID="AutoPostBackOnInitValueLabel" runat="server" /></td>
				</tr>
				<tr>
					<td>Serverový validátor
					<asp:CustomValidator ID="AutoPostBackDateTimeBoxValidator" runat="server" />
					</td>
					<td>
						<asp:Label ID="AutoPostBackServerValidatorValueLabel" runat="server" /></td>
				</tr>
				<tr>
					<td>OnLoad</td>
					<td>
						<asp:Label ID="AutoPostBackOnLoadValueLabel" runat="server" /></td>
				</tr>
				<tr>
					<td>OnLoadComplete</td>
					<td>
						<asp:Label ID="AutoPostBackOnLoadCompleteValueLabel" runat="server" /></td>
				</tr>
				<tr>
					<td>Changed</td>
					<td>
						<asp:Label ID="ChangedLabel" runat="server" /></td>
				</tr>

			</table>
		</div>

		<havit:DateTimeBox ID="SecondDateTimeBox" DateTimeMode="DateTime" CausesValidation="true" runat="server" />
		<asp:CustomValidator ID="SecondDateTimeBoxValidator" ControlToValidate="SecondDateTimeBox" runat="server" />
		<asp:Button ID="PostBackButton" Text="Postback" runat="server" />

		<asp:UpdatePanel UpdateMode="Conditional" runat="server">
			<ContentTemplate>
				<havit:GridViewExt ID="TestGV" runat="server" AutoDataBind="true">
					<Columns>
						<havit:TemplateFieldExt>
							<ItemTemplate>
								<asp:UpdatePanel runat="server">
									<ContentTemplate>
										edit...
									</ContentTemplate>
								</asp:UpdatePanel>
							</ItemTemplate>
							<EditItemTemplate>
								<havit:DateTimeBox ID="NestedDateTimeBox" runat="server" />
							</EditItemTemplate>
						</havit:TemplateFieldExt>
						<havit:TemplateFieldExt ID="TestGVField">
							<ItemTemplate>
								<havit:DateTimeBox ID="NestedDateTimeBox2" runat="server" />
							</ItemTemplate>
						</havit:TemplateFieldExt>
						<havit:GridViewCommandField ShowEditButton="true" ShowCancelButton="true" />
					</Columns>
				</havit:GridViewExt>
			</ContentTemplate>
		</asp:UpdatePanel>
		<asp:DropDownList runat="server" />
		
		<asp:FormView DefaultMode="Edit" runat="server">
			<EditItemTemplate>
				
			</EditItemTemplate>
		</asp:FormView>
	</form>
</body>
</html>
