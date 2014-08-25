<%@ Page Title="" Language="C#" MasterPageFile="~/Templates/Main.Master" %>

<%@ Register TagPrefix="uc" TagName="ValidatorSample" Src="~/sections/controls/samples/ValidatorSample.ascx" %>

<asp:Content ContentPlaceHolderID="TopCPH" runat="server">
	<h1>Validators</h1>
	<p>Extensions of ASP.NET WebForms Validators</p>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainCPH" runat="server">
	<h2>Mapping</h2>
	<p>
		Standard
		<asp:HyperLink NavigateUrl="~/sections/getting-started/getting-started#tagmapping" runat="server">Validators and ValidationSummary are remapped by tag mapping</asp:HyperLink>
		to the descendants in Havit.Web.Bootstrap.
	</p>
	<p>Custom legacy Validators from Havit.Web must be manualy remapped:</p>
	<ul>
		<li>CheckBoxValidator to Havit.Web.Bootstrap.UI.WebControls.Legacy.CheckBoxValidator</li>
		<li>NumericBoxValidator to Havit.Web.Bootstrap.UI.WebControls.Legacy.NumericBoxValidator</li>
	</ul>

	<h2>Properties</h2>
	<ul>
		<ul>
			<li>ShowToolTip (default True)</li>
			<li>ToolTipPosition (default ToolTipPosition.Right)</li>
			<li>ControlToValidateInvalidCssClass (default "validation-invalid") - class name added to the input control when validation fails</li>
			<li>ControlToValidateInvalidTooltipCssClass (default "validation-tooltip") - class name added to the validator tooltip</li>
			<li>Display (default value overriden to ValidatorDisplay.None) - validation text is not display in place of validator</li>
		</ul>
	</ul>

	<h2>Behavior</h2>
	<p>Extends validators with ControlToValidate property value by adding a class to the invalid input field (ControlToValidate) and displaying validation errors as a tooltip of ControlToValidate control.</p>
	<p>Tooltip text is first non empty string in Tooltip, Text, ErrorMessage. Tooltip is visible on control mouse over or when control has focus.</p>
	<p>EnableClientScript property must have true value to enable described behavior.</p>
	<p>Validation tooltips are displayed with container 'body' to enable use in <a href="modaldialog">modal dialog</a>. Otherwise tooltip cannot leave modal dialog and is hidden unded scrollbars.</p>

	<h2>Styling</h2>
	<p>Variables for customization are defined in validation.less file.</p>

	<h2>Sample - Required Field Validator with ValidationSummary</h2>
	<uc:ValidatorSample ID="ValidatorSampleUC" runat="server" />
	<uc:ShowControl ShowControlID="ValidatorSampleUC" runat="server" />

	<h2>Known Issues</h2>
	<h3>Validating user controls</h3>
	<p>Usually for validtion user control, it is used CustomValidator without setting ControlToValidate property. As a result, validator can't use tooltip or add class to a control with invalid value.</p>
	<p>To be able to set ControlToValidate to user control, this requirements must be met:</p>
	<ol>
		<li>User Control must have assigned <a href="http://msdn.microsoft.com/en-us/library/system.web.ui.validationpropertyattribute.aspx">ValidationProperyAttrubute</a> (required by .NET Framework)</li>
		<li>User Control must have assigned ValidationDisplayTargetAttribute which defines control to use as a target for setting css class and tooltip.</li>
	</ol>
	<p>Mainly, this applies to picker controls like SubjectPicker, RolePicker, etc. where nested TextBox ID is used for ValidationDisplayTargetAttribute.</p>

	<h3>Validator '...' should show tooltip but ControlToValidate is not specified</h3>
	<p>One of validator goal is to show tooltip on a control with invalid value. Tooltip is shown on the control determined by ControlToValidate property value. When ControlToValidate is not used (or when it cannot be used) this exception is thrown. To solve this issue it is recommended to explicitly express the wish not to show tooltip by setting ShowTooltip property to false.</p>

	<h3>SetFocusOnError issue</h3>
	<p>When SetFocusOnError is set to true, it does not work properly when used in UpdatePanel (second button with validation does not display tooltip).</p>
</asp:Content>
