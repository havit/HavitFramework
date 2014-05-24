<%@ Page Language="C#" MasterPageFile="Bootstrap.Master" CodeBehind="GridViewExtTest.aspx.cs" Inherits="WebApplicationTest.HavitWebBootstrapTests.GridViewExtTest" %>

<asp:Content ContentPlaceHolderID="MainCPH" runat="server">

	<havit:EnterpriseGridView ID="MainGV" CssClass="table table-bordered table-condensed table-hover table-striped table-responsive" ItemType="Havit.BusinessLayerTest.Subjekt" AllowPaging="true" PageSize="10" PagerRenderMode="BootstrapPagination" PagerSettings-Mode="NumericFirstLast" PagerSettings-PreviousPageText="&lsaquo;" PagerSettings-FirstPageText="&laquo;" PagerSettings-NextPageText="&rsaquo;" PagerSettings-LastPageText="&raquo;" PagerSettingsShowAllPagesButton="true" PagerSettingsAllPagesButtonText="vše" runat="server">
		<Columns>
			<havit:BoundFieldExt DataField="Nazev" SortExpression="Nazev" HeaderText="Název" />
			<havit:GridViewCommandField ButtonType="Link" ShowCancelButton="true" ShowDeleteButton="true" ShowInsertButton="true" ShowEditButton="true" />
		</Columns>
	</havit:EnterpriseGridView>

</asp:Content>
