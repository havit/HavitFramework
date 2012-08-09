<%@ Page Language="C#" Trace="true" CodeBehind="GridViewExtTest.aspx.cs" Inherits="WebApplicationTest.GridViewExtTest" StyleSheetTheme="Theme1" %>
<%@ Register TagPrefix="uc" TagName="GridViewExtTest_InnerGVControl" src="GridViewExtTest_InnerGVControl.ascx"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<havit:EnterpriseGridView ID="TestGV1" AllowInserting="True" AllowPaging="true" PageSize="5" PagerSettings-Position="Bottom" runat="server">
			<Columns>
				<havit:BoundFieldExt DataField="Nazev" SortExpression="Nazev" HeaderText="Název" />
				<havit:TemplateFieldExt SortExpression="Nazev" HeaderText="Název">
					<ItemTemplate>
						<%# Eval("Nazev") %>
					</ItemTemplate>
					<EditItemTemplate>						
						Edit: <%# Eval("Nazev") %>
					</EditItemTemplate>
				</havit:TemplateFieldExt>
				<havit:TemplateFieldExt HeaderText="Grid">
					<EditItemTemplate>
						<asp:Panel runat="server">
							<uc:GridViewExtTest_InnerGVControl runat="server"/>
						</asp:Panel>
					</EditItemTemplate>
				</havit:TemplateFieldExt>
				<havit:GridViewCommandField InsertText="Insert" ButtonType="Image" ShowInsertButton="true" ShowEditButton="true" ShowDeleteButton="true" />
			</Columns>
		</havit:EnterpriseGridView>

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
		<havit:GridViewExt ID="TestGV4" EmptyDataText="No records." AllowPaging="true" PagerSettings-Position="bottom" PagerSettingsShowAllPagesButton="true" PagerSettings-Mode="NextPreviousFirstLast" PageSize="2" runat="server">
			<Columns>
				<havit:BoundFieldExt DataField="Nazev" />
				<havit:GridViewCommandField ShowEditButton="true" ShowDeleteButton="true" />
			</Columns>
		</havit:GridViewExt>
		<havit:GridViewExt ID="TestGV5" EmptyDataText="$resources: Glossary, NoData" AllowPaging="true" PagerSettings-Position="bottom" PagerSettingsShowAllPagesButton="true" PagerSettings-Mode="NextPreviousFirstLast" PageSize="2" runat="server">
			<Columns>
				<havit:BoundFieldExt DataField="Nazev" />
				<havit:GridViewCommandField ShowEditButton="true" ShowDeleteButton="true" />
			</Columns>
		</havit:GridViewExt>
		<asp:Button ID="HideButton" Text="Hide" runat="server" />
		<asp:Button ID="SRDBButton" Text="SetRequiresDatabinding" runat="server" />
		<asp:Button ID="PostbackButton" Text="Postback" runat="server" />
    </div>
    </form>
</body>
</html>
