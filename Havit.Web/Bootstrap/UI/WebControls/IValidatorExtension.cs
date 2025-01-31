namespace Havit.Web.Bootstrap.UI.WebControls;

internal interface IValidatorExtension
{
	string ControlToValidate { get; }
	string ControlToValidateInvalidCssClass { get; }
	string ControlToValidateInvalidToolTipCssClass { get; }
	string ToolTip { get; }
	string Text { get; }
	string ErrorMessage { get; }
	bool ShowToolTip { get; }
	ToolTipPosition ToolTipPosition { get; }
}
