<%@ Page Title="" Language="C#" MasterPageFile="~/HavitWebBootstrapTests/Bootstrap.Master" AutoEventWireup="false" CodeBehind="ModalDialogTest.aspx.cs" Inherits="WebApplicationTest.HavitWebBootstrapTests.ModalDialogTest" %>
<%@ Register Src="~/HavitWebBootstrapTests/ModalDialogUserControlTest.ascx" TagPrefix="uc" TagName="ModalDialogUserControlTest" %>

<asp:Content ContentPlaceHolderID="MainCPH" runat="server">
	<uc:ModalDialogUserControlTest ID="ModalDialogUserControlTestUC" runat="server" />
	
	<asp:Button ID="OpenButton" Text="Open" runat="server" />
	<asp:Button ID="Button1" Text="Postback" runat="server" />	
</asp:Content>
