<h3>Tabs Justified</h3>
<p>See what happens when windows resized.</p>
<bc:TabContainer TabMode="Tabs" Justified="true" runat="server">
	<bc:TabPanel HeaderText="First" runat="server">
		<ContentTemplate>First tab panel.</ContentTemplate>
	</bc:TabPanel>
	<bc:TabPanel HeaderText="Second" runat="server">		
		<ContentTemplate>Second tab panel.</ContentTemplate>
	</bc:TabPanel>
	<bc:TabPanel HeaderText="Third" runat="server">
		<ContentTemplate>Third tab panel.</ContentTemplate>
	</bc:TabPanel>	
</bc:TabContainer>

<h3>Pills</h3>
<bc:TabContainer TabMode="Pills" runat="server">
	<bc:TabPanel HeaderText="First" runat="server">
		<ContentTemplate>First tab panel.</ContentTemplate>
	</bc:TabPanel>
	<bc:TabPanel HeaderText="Second" runat="server">		
		<ContentTemplate>Second tab panel.</ContentTemplate>
	</bc:TabPanel>
	<bc:TabPanel HeaderText="Third" runat="server">
		<ContentTemplate>Third tab panel.</ContentTemplate>
	</bc:TabPanel>	
</bc:TabContainer>

<h3>Pills Stacked</h3>
<bc:TabContainer TabMode="PillsStacked" runat="server">
	<bc:TabPanel HeaderText="First" runat="server">
		<ContentTemplate>First tab panel.</ContentTemplate>
	</bc:TabPanel>
	<bc:TabPanel HeaderText="Second" runat="server">		
		<ContentTemplate>Second tab panel.</ContentTemplate>
	</bc:TabPanel>
	<bc:TabPanel HeaderText="Third" runat="server">
		<ContentTemplate>Third tab panel.</ContentTemplate>
	</bc:TabPanel>	
</bc:TabContainer>
