<%@ Page Title="" Language="C#" MasterPageFile="~/Templates/Main.Master" %>
<%@ Register TagPrefix="uc" TagName="ValidatorSample" Src="~/sections/controls/samples/ValidatorSample.ascx" %>

<asp:Content ContentPlaceHolderID="TopCPH" runat="server">
	<h1>Validators</h1>
	<p>Extensions of ASP.NET WebForms Validators</p>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainCPH" runat="server">
	<h2>Mapping</h2>
	<p>Standard <asp:HyperLink NavigateUrl="~/sections/getting-started/getting-started#tagmapping" runat="server">Validators and ValidationSummary are remapped by tag mapping</asp:HyperLink> to the descendants in Havit.Web.Bootstrap.</p>
	<p>Custom legacy Validators from Havit.Web must be manualy remapped:</p>
	<ul>
		<li>CheckBoxValidator to Havit.Web.Bootstrap.UI.WebControls.Legacy.CheckBoxValidator</li>
		<li>NumericBoxValidator to Havit.Web.Bootstrap.UI.WebControls.Legacy.NumericBoxValidator</li>
	</ul>

	<h2>Properties</h2>
		<ul>
			<ul>
				<li>ShowTooltip (default True)</li>
				<li>TooltipPosition (default TooltipPosition.Right)</li>
				<li>ControlToValidateInvalidCssClass (default "invalid") - class name added to the input control when validation fails</li>
				<li>Display (default value overriden to ValidatorDisplay.None) - validation text is not display in place of validator</li>
				<li>SetFocusOnError (default value overriden to True)</li>
			</ul>
		</ul>
	<h2>Behavior</h2>
	<p>Extends validators with ControlToValidate property value by adding a class to the invalid input field (ControlToValidate) and displaying validation errors as a tooltip of ControlToValidate control.</p>
	<p>Tooltip text is first non empty string in Tooltip, Text, ErrorMessage. Tooltip is visible on control mouse over or when control has focus.</p>
	<br/>
	<p>ValidationSummary is rendered as <a href="http://getbootstrap.com/components/#alerts">Bootstrap Alert</a> (with danger style).</p>

	<h2>Styling</h2>
	<p>Variables for customization are defined in validation.less file.</p>

	<h2>Sample - Required Field Validator with ValidationSummary</h2>	
	<uc:ValidatorSample ID="ValidatorSampleUC" runat="server" />
	<uc:ShowControl Title="Validator sample" ShowControlID="ValidatorSampleUC" runat="server" />

</asp:Content>
