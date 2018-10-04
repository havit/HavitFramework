<%@ Page Title="" Language="C#" MasterPageFile="~/Templates/Main.Master" %>
<%@ Register TagPrefix="uc" TagName="ValidatorSample" Src="~/sections/controls/samples/ValidatorSample.ascx" %>

<asp:Content ContentPlaceHolderID="TopCPH" runat="server">
	<h1>ValidationSummary</h1>
	<p>Extensions of ASP.NET WebForms Validation Summary</p>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainCPH" runat="server">
	<h2>Mapping</h2>
	<p>Standard <asp:HyperLink NavigateUrl="~/sections/getting-started/getting-started#tagmapping" runat="server">Validators and ValidationSummary are remapped by tag mapping</asp:HyperLink> to the descendants in Havit.Web.Bootstrap.</p>

	<h2>Properties</h2>
	<ul>
		<ul>
			<li>ShowToastr (default True)</li>
			<li>CssClass (default value overriden to "alert alert-danger" for case when ShowSummary is displayed)</li>
			<li>DisplayMode (default value overriden to ValidationSummaryDisplayMode.List for case when ShowSummary is displayed)</li>
			<li>ShowSummary (default value overriden to False)</li>
		</ul>
	</ul>

	<h2>Behavior</h2>
	<p>When ShowToastr is True, ValidationSummary displays validation error as toastr error.</p>
	<p>When ShowSummary is True, ValidationSummary is rendered as <a href="http://getbootstrap.com/components/#alerts">Bootstrap Alert</a> (with danger style).</p>
	<p>When ShowMessage is True, message box with CustomValidators errors could be displayed (standard ASP.NET version does not show CustomValidators' errors in message box).</p>
	<br/>

	<h2>Sample - Required Field Validator with ValidationSummary</h2>	
	<uc:ValidatorSample ID="ValidatorSampleUC" runat="server" />
	<uc:ShowControl ShowControlID="ValidatorSampleUC" runat="server" />

</asp:Content>
