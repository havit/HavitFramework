﻿<%@ Page Title="" Language="C#" MasterPageFile="~/HavitWebBootstrapTests/Bootstrap.Master" AutoEventWireup="true" CodeBehind="NavbarTest.aspx.cs" Inherits="Havit.WebApplicationTest.HavitWebBootstrapTests.NavbarTest" %>

<asp:Content ContentPlaceHolderID="MainCPH" runat="server">
	<bc:Navbar ID="FirstNavbar" ToggleNavigationText="ToggleNavigationText" runat="server">
		<BrandTemplate>
			Navbar DEMO
		</BrandTemplate>		
		<MenuItems>
			<bc:NavbarLinkItem ID="FirstNavbarLinkItem" Text="Menu 1">
				<Items>
					<bc:NavbarHeaderItem Text="Header" />
					<bc:NavbarLinkItem Text="Text1 &amp; &quot; &lt;" Url="~/HavitWebBootstrapTests/TooltipTest.aspx"/>
					<bc:NavbarSeparatorItem />
					<bc:NavbarSeparatorItem />
					<bc:NavbarLinkItem Enabled="false" Text="Text2" Url="#" />
					<bc:NavbarSeparatorItem />
				</Items>
			</bc:NavbarLinkItem>
		</MenuItems>
		<RightSectionItems>
			<bc:NavbarLinkItem Text="Odhlásit" Url="~/" />
			<bc:NavbarTextItem Text="Přihlášený uživatel" />
		</RightSectionItems>
	</bc:Navbar>
	
	<asp:SiteMapDataSource ID="WebSiteMapDataSource" SiteMapProvider="WebSiteMapProvider" ShowStartingNode="false" runat="server" />
	<bc:Navbar
		ID="SecondNavbar"
		EnableViewState="false"
		DataSourceID="WebSiteMapDataSource"
		runat="server"
	/>

	<asp:Label ID="CountLabel" runat="server" />
	<asp:Button Text="Postback" runat="server" />
</asp:Content>
