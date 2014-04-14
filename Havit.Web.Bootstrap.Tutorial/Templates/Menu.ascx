<%@ Control Language="C#" CodeBehind="Menu.ascx.cs" Inherits="Havit.Web.Bootstrap.Tutorial.Templates.Menu" %>
<%@ Import Namespace="Microsoft.AspNet.FriendlyUrls" %>

<header class="navbar navbar-static-top" id="top" role="banner">
	<div class="container">
		<div class="navbar-header">
			<button class="navbar-toggle" type="button" data-toggle="collapse" data-target=".bs-navbar-collapse">
				<span class="sr-only">Toggle navigation</span>
				<span class="icon-bar"></span>
				<span class="icon-bar"></span>
				<span class="icon-bar"></span>
			</button>
			<asp:HyperLink NavigateUrl="~/" CssClass="navbar-brand" runat="server">Havit.Web.Bootstrap</asp:HyperLink>
		</div>
		<nav class="collapse navbar-collapse" role="navigation">
			<ul class="nav navbar-nav">
				<li>
					<asp:HyperLink NavigateUrl="~/sections/getting-started/getting-started" runat="server">Getting started</asp:HyperLink>
				</li>
				<li class="dropdown">
					<a href="#">Controls <b class="caret"></b></a>
					<ul class="dropdown-menu">
						<li><asp:HyperLink NavigateUrl="~/sections/controls/checkboxlist_radiobuttonlist" runat="server">CheckBoxList and RadioButtonList</asp:HyperLink></li>
						<li><asp:HyperLink NavigateUrl="~/sections/controls/modaldialog" runat="server">ModalDialog</asp:HyperLink></li>
						<li><asp:HyperLink NavigateUrl="~/sections/controls/tabs" runat="server">Tabs</asp:HyperLink></li>
						<li><asp:HyperLink NavigateUrl="~/sections/controls/tooltip" runat="server">Tooltip</asp:HyperLink></li>
						<li><asp:HyperLink NavigateUrl="~/sections/controls/switchbutton" runat="server">SwitchButton</asp:HyperLink></li>
						<li><asp:HyperLink NavigateUrl="~/sections/controls/validators" runat="server">Validators</asp:HyperLink></li>
						<li><asp:HyperLink NavigateUrl="~/sections/controls/validation-summary" runat="server">Validation Summary</asp:HyperLink></li>
						<li class="divider"></li>
						<li><asp:HyperLink NavigateUrl="~/sections/controls/_backlog" runat="server">Backlog</asp:HyperLink></li>
					</ul>
				</li>
				<li><asp:HyperLink NavigateUrl="~/sections/less/less" runat="server">LESS</asp:HyperLink></li>
			</ul>
		</nav>
	</div>
</header>
