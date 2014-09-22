<%@ Page Title="" Language="C#" MasterPageFile="~/Templates/Main.Master" %>
<%@ Register Src="~/sections/controls/samples/DateTimeBoxExtensionsSample.ascx" TagPrefix="uc" TagName="DateTimeBoxExtensionsSample" %>

<asp:Content ContentPlaceHolderID="TopCPH" runat="server">
	<h1>DateTimeBox extensions</h1>
	<p>DateTimeBox extensions to enable rendering and use in bootstrap based application</p>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainCPH" runat="server">
	
	<div class="alert-danger">
		<blockquote>
			<p>This is not a new control but only only an extension of <a href="http://hfw.havit.local/Documentation/?topic=html/25dd52ed-c6e9-ca7f-fce8-5ee8630257f4.htm">Havit.Web.UI.Controls.DateTimeBox</a>.</p>
			<p>New DateBox or DateTimeBox will be based on the <a href="http://eonasdan.github.io/bootstrap-datetimepicker/">Bootstrap datetimepicker</a>.</p>
		</blockquote>
	</div>

	<h2>Properties</h2>
	<ul>
		<li>ContainerRenderMode (default: DateTimeBoxContainerRenderMode.Standard) - posible values: Standard, BootstrapInputGroupAddOnOnLeft, BootstrapInputGroupAddOnOnRight<br/>
			When used BootstrapInputGroupAddOnOnLeft or BootstrapInputGroupAddOnOnRight classes input-group and form-control are rendered automatically.
		</li>
	</ul>

	<h2>Rendered HTML</h2>
	It is intended to render bootstrap input group with addon.
	<pre>&lt;div class="input-group"&gt;
	&lt;input type="text" class="form-control" /&gt;
	&lt;span class="input-group-btn"&gt;
		&lt;button class="btn btn-default" type="button"&gt;
			...(icon)...
		&lt;/button&gt;
	&lt;/span&gt;
&lt;/div&gt;</pre>

	<h2>Sample</h2>
	<uc:DateTimeBoxExtensionsSample ID="DateTimeBoxExtensionsSampleUC" runat="server" />
	<br/>
	<uc:ShowControl ShowControlID="DateTimeBoxExtensionsSampleUC" runat="server" />
</asp:Content>
