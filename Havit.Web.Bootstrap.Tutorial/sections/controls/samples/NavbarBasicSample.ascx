<bc:Navbar runat="server">
	<BrandTemplate>
		Navbar DEMO
	</BrandTemplate>		
	<MenuItems>
		<bc:NavbarLinkItem ID="FirstNavbarMenuItem" Text="First ">
			<Items>
				<bc:NavbarHeaderItem Text="First section" />
				<bc:NavbarLinkItem ID="Item1LinkItem" Text="Item 1" Url="#"/>
				<bc:NavbarLinkItem Text="Item 2" Url="#"/>
				<bc:NavbarSeparatorItem />
				<bc:NavbarHeaderItem Text="Second section" />
				<bc:NavbarLinkItem Text="Item 3" Url="#" />
				<bc:NavbarLinkItem Text="Item 4" Url="#" />
			</Items>
		</bc:NavbarLinkItem>
	</MenuItems>
	<RightSectionTemplate>
		<li><a href="#">Username here</a></li>
	</RightSectionTemplate>
</bc:Navbar>

<script runat="server">
	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);
		Item1LinkItem.VisibleFunc = () => Havit.Web.Bootstrap.Tutorial.My_Page.IsAuthorizedToAccess();
	}
</script>