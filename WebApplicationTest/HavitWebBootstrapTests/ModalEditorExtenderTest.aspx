<%@ Page Language="C#" MasterPageFile="Bootstrap.Master" CodeBehind="GridViewExtTest.aspx.cs" Inherits="WebApplicationTest.HavitWebBootstrapTests.GridViewExtTest" StyleSheetTheme="BootstrapTheme" %>

<asp:Content ContentPlaceHolderID="MainCPH" runat="server">
	
	<asp:UpdatePanel UpdateMode="Conditional" runat="server">
		<ContentTemplate>

			<havit:EnterpriseGridView ID="MainGV" AllowInserting="true" AutoCrudOperations="true" AllowPaging="true" PageSize="5" runat="server">
				<Columns>
					<havit:BoundFieldExt DataField="Nazev" SortExpression="Nazev" HeaderText="Název" />
					<havit:GridViewCommandField ShowEditButton="true" ShowInsertButton="true"  />
				</Columns>
			</havit:EnterpriseGridView>	
		</ContentTemplate>
	</asp:UpdatePanel>
	
	<bc:ModalEditorExtender ID="ModalEditorExtender" TargetControlID="MainGV" ItemType="Havit.BusinessLayerTest.Subjekt" runat="server">
		<HeaderTemplate>
			Subjekt <%# Item.Nazev %>
		</HeaderTemplate>
		<ContentTemplate>
			<asp:TextBox ID="NazevTB" Text="<%# BindItem.Nazev %>" runat="server" />
		</ContentTemplate>
		<FooterTemplate>
			<bc:Button CommandName="OK" Text="OK" runat="server" />
			<bc:Button CommandName="Save" Text="Save" runat="server" />
			<bc:Button CommandName="Cancel" Text="Cancel" runat="server" />
			<bc:Button CommandName="Next" Text="next" runat="server" />
			<bc:Button CommandName="Previous" Text="previous" runat="server" />
		</FooterTemplate>
	</bc:ModalEditorExtender>

</asp:Content>
