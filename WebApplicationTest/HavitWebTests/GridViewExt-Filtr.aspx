<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GridViewExt-Filtr.aspx.cs" Inherits="WebApplicationTest.HavitWebTests.GridViewExt_Filtr" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<havit:GridViewExt ID="MainGridView" ShowHeaderWhenEmpty="True" AllowPaging="True" PageSize="10" PagerSettings-Position="TopAndBottom" ItemType="Havit.BusinessLayerTest.Subjekt" ShowFilter="True" HeaderStyle-ForeColor="red" FilterStyle-BackColor="blue"  runat="server">
			<Columns>
				<%--<havit:BoundFieldExt HeaderText="Název" DataField="Nazev" FilterMode="TextBox"/>				--%>
				<havit:TemplateFieldExt HeaderText="Název">
					<ItemTemplate>
						<%# Item.Nazev %>
					</ItemTemplate>
					<FilterTemplate>
						filtr: <havit:AutoFilterDropDownList DataTextField="Nazev.Length" runat="server" />
						<havit:AutoFilterTextBox DataFilterField="Nazev" Text="6" runat="server" />
						
					</FilterTemplate>
				</havit:TemplateFieldExt>
			</Columns>
		</havit:GridViewExt>
    </div>
    </form>
</body>
</html>
