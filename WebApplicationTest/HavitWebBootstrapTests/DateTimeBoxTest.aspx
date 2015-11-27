<%@ Page Title="" Language="C#" MasterPageFile="~/HavitWebBootstrapTests/Bootstrap.Master" AutoEventWireup="true" CodeBehind="DateTimeBoxTest.aspx.cs" Inherits="Havit.WebApplicationTest.HavitWebBootstrapTests.DateTimeBoxTest" %>

<asp:Content ContentPlaceHolderID="MainCPH" runat="server">

	<havit:DateTimeBox DateTimePickerElement="Image" runat="server"/>
	<br/>
	<br/>

	<div class="form-inline">
		<div class="form-group">
				<havit:DateTimeBox DateTimePickerElement="Image" ContainerRenderMode="BootstrapInputGroupButtonOnLeft" runat="server"/>
				<havit:DateTimeBox DateTimePickerElement="Image" ContainerRenderMode="BootstrapInputGroupButtonOnRight" runat="server"/>
		</div>
	</div>
	
	<div class="form-inline">
		<div class="form-group">
			<havit:DateTimeBox DateTimePickerElement="Image" ContainerRenderMode="BootstrapInputGroupButtonOnLeft" AddOnText="AddOn &amp; Text" runat="server"/>
			<havit:DateTimeBox DateTimePickerElement="Image" ContainerRenderMode="BootstrapInputGroupButtonOnRight" AddOnText="AddOn &amp; Text" runat="server"/>
		</div>
	</div>
</asp:Content>
