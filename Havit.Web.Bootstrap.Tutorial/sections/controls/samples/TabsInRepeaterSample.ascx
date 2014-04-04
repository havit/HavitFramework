<%@ Control Language="C#" %>

<bc:TabContainer runat="server">
	
	<bc:TabPanel HeaderText="Static tab" runat="server">
		<ContentTemplate>
			First tab panel.			
		</ContentTemplate>
	</bc:TabPanel>
	
	<asp:Repeater ID="TabsRepeater" ItemType="System.Int32" runat="server">
		<ItemTemplate>			
			<bc:TabPanel HeaderText="<%# Item %>" runat="server">
				<ContentTemplate>
					Dynamic tab # <%# ((RepeaterItem)Container).DataItem %> content.					
				</ContentTemplate>
			</bc:TabPanel>
		</ItemTemplate>
	</asp:Repeater>
	
</bc:TabContainer>

<script runat="server">
	protected override void OnLoad(EventArgs e)
	{
		if (!Page.IsPostBack)
		{
			TabsRepeater.DataSource = Enumerable.Range(1, 5);
			TabsRepeater.DataBind();
		}
	}
</script>