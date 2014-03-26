<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="GridViewExtTest_InnerGVControl.ascx.cs" Inherits="WebApplicationTest.HavitWebTests.GridViewExtTest_InnerGVControl" %>

<havit:EnterpriseGridView ID="InnerGV" AllowInserting="True" runat="server">
	<Columns>
		<havit:BoundFieldExt DataField="Nazev" />
		<havit:GridViewCommandField ButtonType="Image" ShowInsertButton="True" InsertText="Insert" ShowEditButton="true" ShowDeleteButton="true" ValidationGroup="InnerGVValidationGroup" />
	</Columns>
</havit:EnterpriseGridView>
