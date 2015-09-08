<div class="form-horizontal">
	<div class="form-group">
		<label class="col-sm-2 control-label">From date</label>
		<span class="col-sm-10">
			<havit:DateTimeBox ContainerRenderMode="BootstrapInputGroupButtonOnLeft" runat="server" />
		</span>
	</div>
	<div class="form-group">
		<label class="col-sm-2 control-label">To date</label>
		<span class="col-sm-10">
			<havit:DateTimeBox ContainerRenderMode="BootstrapInputGroupButtonOnRight" runat="server" />
		</span>
	</div>
	<div class="form-group">
		<label class="col-sm-2 control-label">Date Range</label>
		<span class="col-sm-5">
			<havit:DateTimeBox AddOnText="From" ContainerRenderMode="BootstrapInputGroupButtonOnRight" runat="server" />
		</span>
		<span class="col-sm-5">
			<havit:DateTimeBox AddOnText="To" ContainerRenderMode="BootstrapInputGroupButtonOnRight" runat="server" />
		</span>
	</div>
</div>
