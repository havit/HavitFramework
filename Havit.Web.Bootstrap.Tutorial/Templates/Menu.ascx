﻿<asp:SiteMapDataSource ID="MainSiteMapDataSource" ShowStartingNode="false" SiteMapProvider="MainMenu" runat="server"/>
<bc:Navbar DataSourceID="MainSiteMapDataSource" CssClass="navbar navbar-static-top" ShowCaret="true" ContainerMode="Container" runat="server">
	<BrandTemplate>Havit.Web.Bootstrap</BrandTemplate>
</bc:Navbar>
