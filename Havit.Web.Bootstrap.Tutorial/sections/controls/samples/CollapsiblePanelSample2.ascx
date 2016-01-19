<style>
	.panel-heading {
		cursor: pointer;
	}
</style>
<bc:CollapsiblePanel HeaderText="Collapsible panel" Collapsed="False" AutoPostBack="True" runat="server">
	<ContentTemplate>
		<div class="panel-body">
			<pre>This context is in collapsible section...</pre>
		</div>
		<div class="panel-footer">
			Footer of collapsible panel
		</div>
	</ContentTemplate>
</bc:CollapsiblePanel>