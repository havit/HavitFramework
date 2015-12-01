<%@ Page Title="" Language="C#" MasterPageFile="~/HavitWebBootstrapTests/Bootstrap.Master" StylesheetTheme="BootstrapTheme" CodeBehind="ButtonTest.aspx.cs" Inherits="Havit.WebApplicationTest.HavitWebBootstrapTests.ButtonTest" %>

<asp:Content ContentPlaceHolderID="MainCPH" runat="server">
	<bc:Button CssClass="btn btn-default btn-lg" SkinID="Save" runat="server">abc</bc:Button>
	<bc:Button CssClass="btn btn-default btn-lg" SkinID="Save" IconPosition="Right" runat="server">abc</bc:Button>
	<bc:Button CssClass="btn btn-default btn-lg" SkinID="Save" runat="server"/>
</asp:Content>
