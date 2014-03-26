<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="EnterpriseDropDownListTest.aspx.cs" Inherits="WebApplicationTest.HavitEnterpriseWebTests.EnterpriseDropDownListTest" %>
<%@ Import Namespace="Havit.BusinessLayerTest" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<havit:EnterpriseDropDownList 
			ItemObjectInfo="<%$ Expression: Role.ObjectInfo %>" 
			AutoDataBind="false"
			AutoPostBack="true"
			DataTextField="Symbol"		
			ID="AutoPostBackEDDL"
			runat="server" 
		/> Vybrano: <asp:Label ID="AutoPostBackResultLabel" runat="server" /><br />
		<havit:EnterpriseDropDownList 
			SortDirection="Descending"
			ItemObjectInfo="<%$ Expression: Role.ObjectInfo %>" 
			AutoDataBind="true"
			DataTextField="Symbol"		
			runat="server"
		/>
	
	<table>
	<tr>
	<td>
		<havit:EnterpriseGridView runat="server" AutoDataBind="false" ID="Test1GV">
			<Columns>
				<havit:BoundFieldExt DataField="Nazev" />
				<havit:TemplateFieldExt>
					<ItemTemplate>
						<havit:EnterpriseDropDownList 
							ID="TestDDL"
							AutoDataBind="true"
							DataTextField="Nazev"
							ItemObjectInfo="<%$ Expression: Subjekt.ObjectInfo %>"
							runat="server"
						/>
					</ItemTemplate>
				</havit:TemplateFieldExt>
			</Columns>
		</havit:EnterpriseGridView>
	</td>
	<td>
		<havit:EnterpriseGridView runat="server" AutoDataBind="true" ID="Test2GV">
			<Columns>
				<havit:BoundFieldExt DataField="Nazev" />
				<havit:TemplateFieldExt>
					<ItemTemplate>
						<havit:EnterpriseDropDownList 
							ID="TestDDL"
							AutoDataBind="true"
							DataTextField="Nazev"
							ItemObjectInfo="<%$ Expression: Subjekt.ObjectInfo %>"
							runat="server"
						/>
					</ItemTemplate>
				</havit:TemplateFieldExt>
			</Columns>
		</havit:EnterpriseGridView>
	</td>
	</tr>
	</table>
		
		<asp:Repeater runat="server" ID="TestRepeater">
			<ItemTemplate>
				<havit:EnterpriseDropDownList 
					ID="TestDDL"
					AutoDataBind="true"
					DataTextField="Nazev"
					ItemObjectInfo="<%$ Expression: Subjekt.ObjectInfo %>"
					SelectedObject="<%# Subjekt.GetObject(9) %>"
					runat="server"
				/>			
			</ItemTemplate>
		</asp:Repeater>
    </div>
    </form>
</body>
</html>
