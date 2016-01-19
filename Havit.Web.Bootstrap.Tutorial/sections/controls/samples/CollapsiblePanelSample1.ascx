<style>
	.panel-heading {
		cursor: pointer;
	}
</style>
<bc:CollapsiblePanel runat="server">
	<HeaderTemplate>
		<h1 class="panel-title">Collapsible panel</h1>
	</HeaderTemplate>
	<ContentTemplate>
		<div class="panel-body">
			<pre>This context is in collapsible section...</pre>
		</div>
		<div class="panel-footer">
			Footer of collapsible panel
		</div>
	</ContentTemplate>
</bc:CollapsiblePanel>