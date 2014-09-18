<%@ Page Title="" Language="C#" MasterPageFile="~/HavitWebBootstrapTests/Bootstrap.Master" AutoEventWireup="true" CodeBehind="DateTimeBoxTest.aspx.cs" Inherits="WebApplicationTest.HavitWebBootstrapTests.DateTimeBoxTest" %>

<asp:Content ContentPlaceHolderID="MainCPH" runat="server">

	<havit:DateTimeBox DateTimePickerElement="Image" runat="server"/>
	<br/>
	<br/>

	<div class="form-inline">
		<div class="form-group">
				<havit:DateTimeBox DateTimePickerElement="Image" ContainerRenderMode="BootstrapInputGroupAddOnOnLeft" runat="server"/>
				<havit:DateTimeBox DateTimePickerElement="Image" ContainerRenderMode="BootstrapInputGroupAddOnOnRight" runat="server"/>
		</div>
	</div>
</asp:Content>
