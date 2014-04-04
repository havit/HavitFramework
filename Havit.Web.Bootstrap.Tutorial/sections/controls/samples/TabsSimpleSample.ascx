<%@ Control Language="C#" %>

<bc:TabContainer runat="server">
	
	<bc:TabPanel HeaderText="First" runat="server">
		<ContentTemplate>
			First tab panel.			
		</ContentTemplate>
	</bc:TabPanel>

	<bc:TabPanel runat="server">
		<HeaderTemplate>Second<sup>2</sup></HeaderTemplate>
		<ContentTemplate>
			Second tab panel.
		</ContentTemplate>
	</bc:TabPanel>
	
	<bc:TabPanel HeaderText="Third" Enabled="false" runat="server">
		<ContentTemplate>
			Third tab panel.
		</ContentTemplate>		
	</bc:TabPanel>

</bc:TabContainer>