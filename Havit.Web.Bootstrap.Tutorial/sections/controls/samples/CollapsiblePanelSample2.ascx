<style>
	.panel-heading {
		cursor: pointer;
	}
</style>


<script runat="server">
protected void Example2CollapsiblePanel_CollapsedStateChanged(object sender, EventArgs eventArgs)
{
	Example2CollapsiblePanelStateLb.Text = String.Format("Panel state: {0}", Example2CollapsiblePanel.Collapsed ? "collapsed" : "expanded");
}
</script>

<asp:UpdatePanel runat="server">
<ContentTemplate>

	<asp:Label ID="Example2CollapsiblePanelStateLb" Text="Panel state: expanded" runat="server" />

	<bc:CollapsiblePanel ID="Example2CollapsiblePanel" HeaderText="Collapsible panel" Collapsed="False" AutoPostBack="True" OnCollapsedStateChanged="Example2CollapsiblePanel_CollapsedStateChanged" runat="server">
		<ContentTemplate>
			<pre>This context is in collapsible section...</pre>
		</ContentTemplate>
	</bc:CollapsiblePanel>

</ContentTemplate>
</asp:UpdatePanel>