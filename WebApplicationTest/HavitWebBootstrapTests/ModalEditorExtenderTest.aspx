<%@ Page Language="C#" MasterPageFile="Bootstrap.Master" CodeBehind="GridViewExtTest.aspx.cs" Inherits="Havit.WebApplicationTest.HavitWebBootstrapTests.GridViewExtTest" StyleSheetTheme="BootstrapTheme" %>

<asp:Content ContentPlaceHolderID="MainCPH" runat="server">
	<style>
		.edited {
			background-color: yellow;
		}
		.green {
			background-color: greenyellow;
		}
	</style>
	
	<asp:UpdatePanel UpdateMode="Conditional" runat="server">
		<ContentTemplate>
			<havit:EnterpriseGridView ID="MainGV" Enabled="true" EditorExtenderEditCssClass="edited" AllowInserting="true" AutoCrudOperations="true" AllowPaging="true" PageSize="5" AutoSort="true" DefaultSortExpression="Nazev" MessengerInsertedMessage="Inserted." MessengerUpdatedMessage="Updated." MessengerDeletedMessage="Deleted." PagerRenderMode="BootstrapPagination" PagerSettings-Mode="NumericFirstLast" runat="server">
				<Columns>
					<havit:BoundFieldExt DataField="Nazev" SortExpression="Nazev" HeaderText="Název" />
					<havit:GridViewCommandField ShowEditButton="true" ShowInsertButton="true" ShowDeleteButton="true" HeaderStyle-CssClass="headercssclass" HeaderNewCssClass="green"  />
				</Columns>
			</havit:EnterpriseGridView>	
		</ContentTemplate>
	</asp:UpdatePanel>
	
	<bc:ModalEditorExtender ID="ModalEditorExtender" TargetControlID="MainGV" ItemType="Havit.BusinessLayerTest.Subjekt" ValidationGroup="" runat="server">
		<HeaderTemplate>
			Subjekt <%# Item.Nazev %>
		</HeaderTemplate>
		<ContentTemplate>
			<asp:TextBox ID="NazevTB" Text="<%# BindItem.Nazev %>" runat="server" />
			<bc:Button CommandName="OK" Text="OK" runat="server" />
		</ContentTemplate>
		<FooterTemplate>
			<bc:Button CommandName="OK" Text="OK" runat="server" />
			<bc:Button CommandName="Save" Text="Save" runat="server" />
			<bc:Button CommandName="Cancel" Text="Cancel" runat="server" />
			<bc:Button CommandName="New" Text="New" runat="server" />
			<bc:Button CommandName="Next" Text="Next" runat="server" />
			<bc:Button CommandName="Previous" Text="Previous" runat="server" />
		</FooterTemplate>
	</bc:ModalEditorExtender>

</asp:Content>
