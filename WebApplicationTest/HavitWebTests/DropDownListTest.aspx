<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DropDownListTest.aspx.cs" Inherits="WebApplicationTest.HavitWebTests.DropDownListTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<havit:GridViewExt ID="TestGridView" runat="server">
			<Columns>
				<havit:TemplateFieldExt>
					<ItemTemplate>
						<havit:EnterpriseDropDownList
							ID="RoleDDL"
							ItemObjectInfo="<%$ Expression: Havit.BusinessLayerTest.Role.ObjectInfo %>"
							SelectedObject="<%# Havit.BusinessLayerTest.Role.GetObject(-1) %>"
							DataTextField="Symbol"
							AutoDataBind="false"
							AutoSort="false"
							runat="server" />
					</ItemTemplate>
				</havit:TemplateFieldExt>
			</Columns>
		</havit:GridViewExt>
		<asp:Button ID="PostbackButton" Text="Postback" runat="server" />
    </div>
    </form>
</body>
</html>
