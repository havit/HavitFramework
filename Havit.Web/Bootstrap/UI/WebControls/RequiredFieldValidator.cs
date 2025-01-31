using System.ComponentModel;
using System.Web.UI;

namespace Havit.Web.Bootstrap.UI.WebControls;

/// <summary>
/// Extended RequiredFieldValidator.
/// </summary>
public class RequiredFieldValidator : System.Web.UI.WebControls.RequiredFieldValidator, IValidatorExtension
{
	/// <summary>
	/// Shows ToolTip (or Text if ToolTip not set) as a Bootstrap ToolTip at ControlToValidate when validation fails.
	/// </summary>
	public bool ShowToolTip
	{
		get
		{
			return (bool)(ViewState["ShowToolTip"] ?? true);
		}
		set
		{
			ViewState["ShowToolTip"] = value;
		}
	}

	/// <summary>
	/// ToolTip position.
	/// </summary>
	[DefaultValue(ToolTipPosition.Right)]
	public ToolTipPosition ToolTipPosition
	{
		get
		{
			return (ToolTipPosition)(ViewState["ToolTipPosition"] ?? ToolTipPosition.Right);
		}
		set
		{
			ViewState["ToolTipPosition"] = value;
		}
	}

	/// <summary>
	/// CssClass name which is added to ControlToValidate when validation fails.
	/// </summary>
	[DefaultValue(ValidatorRenderExtender.DefaultControlToValidateInvalidCssClass)]
	public string ControlToValidateInvalidCssClass
	{
		get
		{
			return (string)(ViewState["ControlToValidateInvalidCssClass"] ?? ValidatorRenderExtender.DefaultControlToValidateInvalidCssClass);
		}
		set
		{
			ViewState["ControlToValidateInvalidCssClass"] = value;
		}
	}

	/// <summary>
	/// CssClass name which is added to a validation tooltip.
	/// </summary>
	[DefaultValue(ValidatorRenderExtender.DefaultControlToValidateInvalidToolTipCssClass)]
	public string ControlToValidateInvalidToolTipCssClass
	{
		get
		{
			return (string)(ViewState["ControlToValidateInvalidToolTipCssClass"] ?? ValidatorRenderExtender.DefaultControlToValidateInvalidToolTipCssClass);
		}
		set
		{
			ViewState["ControlToValidateInvalidToolTipCssClass"] = value;
		}
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	public RequiredFieldValidator()
	{
		ValidatorRenderExtender.Setup(this);
	}

	/// <summary>
	/// Adds the attributes of this control to the output stream for rendering on the client.
	/// </summary>
	protected override void AddAttributesToRender(HtmlTextWriter writer)
	{
		base.AddAttributesToRender(writer);
		ValidatorRenderExtender.AddAttributesToRender(this, writer);
	}

	/// <summary>
	/// Checks the client brower and configures the validator for compatibility prior to rendering.
	/// </summary>
	protected override void OnPreRender(EventArgs e)
	{
		base.OnPreRender(e);
		ValidatorRenderExtender.OnPreRender(this);
	}
}
