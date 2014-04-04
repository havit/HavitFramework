<%@ Page Title="" Language="C#" MasterPageFile="~/Templates/Main.Master" CodeBehind="Show.aspx.cs" Inherits="Havit.Web.Bootstrap.Tutorial.Section.Samples.ShowControlPage" %>
<asp:Content ContentPlaceHolderID="TopCPH" runat="server">
	<h1><asp:Literal ID="TitleLiteral" runat="server" /></h1>
	<p>Source code</p>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainCPH" runat="server">
	<pre><code><asp:Literal ID="ContentLiteral" runat="server" /></code></pre>
</asp:Content>
