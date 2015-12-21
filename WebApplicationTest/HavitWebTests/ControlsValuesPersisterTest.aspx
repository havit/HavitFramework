<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="ControlsValuesPersisterTest.aspx.cs" Inherits="Havit.WebApplicationTest.HavitWebTests.ControlsValuesPersisterTest" %>
<%@ Import Namespace="Havit.BusinessLayerTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<havit:ControlsValuesPersister ID="MainControlsValuesPersister" runat="server">

			<div>TextBox: <asp:TextBox ID="TestTextBox" runat="server" /></div>
			<div>NumericBox:<havit:NumericBox ID="TestNB" runat="server" /></div>
			<div>
				CheckBoxList:
				<asp:CheckBoxList ID="TestChBL" runat="server">
					<asp:ListItem Value="0" Text="0" />
					<asp:ListItem Value="1" Text="1" />
					<asp:ListItem Value="2" Text="2" />
					<asp:ListItem Value="3" Text="3" />
					<asp:ListItem Value="4" Text="4" />
					<asp:ListItem Value="5" Text="5" />
				</asp:CheckBoxList>
			</div>

			<div>
				ListBox:
				<asp:ListBox ID="TestLB" runat="server">
					<asp:ListItem Value="0" Text="0" />
					<asp:ListItem Value="1" Text="1" />
					<asp:ListItem Value="2" Text="2" />
					<asp:ListItem Value="3" Text="3" />
					<asp:ListItem Value="4" Text="4" />
					<asp:ListItem Value="5" Text="5" />
				</asp:ListBox>
			</div>

			<div>
				RadioButtonList:
				<asp:RadioButtonList ID="TestRBL" runat="server">
					<asp:ListItem Value="0" Text="0" />
					<asp:ListItem Value="1" Text="1" />
					<asp:ListItem Value="2" Text="2" />
					<asp:ListItem Value="3" Text="3" />
					<asp:ListItem Value="4" Text="4" />
					<asp:ListItem Value="5" Text="5" />
				</asp:RadioButtonList>
			</div>
		
			<div>
				GridViewExt
				<havit:GridViewExt ID="TestGVE" AllowPaging="true" PagerSettings-Position="bottom" PagerSettingsShowAllPagesButton="true" PagerSettingsAllPagesButtonImageUrl="~/a/test.gif" PagerSettings-Mode="NumericFirstLast" PageSize="3" runat="server">
					<Columns>
						<havit:TemplateFieldExt SortExpression="Nazev" HeaderText="Název">
							<ItemTemplate>
								<asp:TextBox ID="NazevTextBox" Text="" runat="server" />
							</ItemTemplate>							
						</havit:TemplateFieldExt>
						<havit:GridViewCommandField ShowEditButton="true" ShowDeleteButton="true" />
					</Columns>
				</havit:GridViewExt>
			</div>
		</havit:ControlsValuesPersister>

		<asp:Button Text="Save" ID="SaveButton" runat="server" />
		<asp:Button Text="Clear" ID="ClearButton" runat="server" />
		<asp:Button Text="Load" ID="LoadButton" runat="server" />
    </div>

    </form>
</body>
</html>
