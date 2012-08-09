<%@ Page Language="C#" Trace="true" CodeBehind="GridViewExtTest.aspx.cs" Inherits="WebApplicationTest.GridViewExtTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<havit:GridViewExt ID="TestGV1" AllowPaging="true" PagerSettings-Position="bottom" PagerSettingsShowAllPagesButton="true" PagerSettingsAllPagesButtonText="Vše" PagerSettings-Mode="Numeric" PageSize="2000" AllowSorting="true" AutoSort="true" runat="server">
			<Columns>
				<havit:TemplateFieldExt SortExpression="Nazev" HeaderText="Název">
					<ItemTemplate>
						<%# Eval("Nazev") %>
					</ItemTemplate>
					<EditItemTemplate>
						Edit: <%# Eval("Nazev") %>
					</EditItemTemplate>
				</havit:TemplateFieldExt>
				<havit:GridViewCommandField ShowEditButton="true" ShowDeleteButton="true" />
			</Columns>
		</havit:GridViewExt>
		<havit:GridViewExt ID="TestGV2" AllowPaging="true" PagerSettings-Position="bottom" PagerSettingsShowAllPagesButton="true" PagerSettingsAllPagesButtonImageUrl="~/a/test.gif" PagerSettings-Mode="NumericFirstLast" PageSize="2" runat="server">
			<Columns>
				<havit:BoundFieldExt DataField="Nazev" />
				<havit:GridViewCommandField ShowEditButton="true" ShowDeleteButton="true" />
			</Columns>
		</havit:GridViewExt>
		<havit:GridViewExt ID="TestGV3" AllowPaging="true" PagerSettings-Position="bottom" PagerSettingsShowAllPagesButton="true" PagerSettings-Mode="NextPrevious" PageSize="2" runat="server">
			<Columns>
				<havit:BoundFieldExt DataField="Nazev" />
				<havit:GridViewCommandField ShowEditButton="true" ShowDeleteButton="true" />
			</Columns>
		</havit:GridViewExt>
		<havit:GridViewExt ID="TestGV4" AllowPaging="true" PagerSettings-Position="bottom" PagerSettingsShowAllPagesButton="true" PagerSettings-Mode="NextPreviousFirstLast" PageSize="2" runat="server">
			<Columns>
				<havit:BoundFieldExt DataField="Nazev" />
				<havit:GridViewCommandField ShowEditButton="true" ShowDeleteButton="true" />
			</Columns>
		</havit:GridViewExt>
    </div>
    </form>
</body>
</html>
