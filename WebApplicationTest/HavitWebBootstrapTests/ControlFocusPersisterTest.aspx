<%@ Page Language="C#" MasterPageFile="~/HavitWebBootstrapTests/Bootstrap.Master"  CodeBehind="ControlFocusPersisterTest.aspx.cs" Inherits="Havit.WebApplicationTest.HavitWebBootstrapTests.ControlFocusPersisterTest" %>

<asp:Content runat="server" ContentPlaceHolderID="MainCPH">
	<havit:ControlFocusPersister runat="server"/>
	<p><asp:TextBox ID="TextBox1" AutoPostBack="True" runat="server" /></p>
	<p><asp:TextBox ID="TextBox2" AutoPostBack="True" runat="server" /></p>
	<p><asp:TextBox ID="TextBox3" AutoPostBack="True" runat="server" /></p>
	<p><asp:TextBox ID="TextBox4" AutoPostBack="True" runat="server" /></p>
	<asp:UpdatePanel runat="server">
		<ContentTemplate>
			<p><asp:TextBox ID="TextBox5" AutoPostBack="True" runat="server" /></p>
			<p><asp:TextBox ID="TextBox6" AutoPostBack="True" runat="server" /></p>
			<p><asp:TextBox ID="TextBox7" AutoPostBack="True" runat="server" /></p>
			<p><asp:TextBox ID="TextBox8" AutoPostBack="True" runat="server" /></p>
		</ContentTemplate>
	</asp:UpdatePanel>
</asp:Content>
