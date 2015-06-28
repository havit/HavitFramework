<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ReleaseOnUnloadControlTest.ascx.cs" Inherits="Havit.WebApplicationTest.HavitCastleWindsorWebFormsTests.ReleaseOnUnloadControlTest" %>
<%@ Register TagPrefix="havit" TagName="NestedUserControlWithDepedency" Src="~/HavitCastleWindsorWebFormsTests/NestedUserControlWithDepedency.ascx" %>
<div>
	Control:
	<asp:Label ID="MyLabel" runat="server" />
</div>
Nested:
<havit:NestedUserControlWithDepedency ID="Test" runat="server" />