using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Havit.Diagnostics.Contracts;
using Havit.Web.UI;

[assembly: WebResource("Havit.Web.Bootstrap.UI.WebControls.WebUIValidationExtension.js", "application/javascript")]
namespace Havit.Web.Bootstrap.UI.WebControls
{
	/// <summary>
	/// Helper class for rendering Validator Extensions.
	/// </summary>
	internal static class ValidatorRenderExtender
	{
		#region DefaultControlToValidateInvalidCssClass (const)
		/// <summary>
		/// Default CssClass for control with failed validation (validators sets this css class).
		/// </summary>
		internal const string DefaultControlToValidateInvalidCssClass = "validation-invalid";
		#endregion

		#region Setup
		/// <summary>
		/// Sets up validator (used from constructor).
		/// Changes validator default values to:
		/// - Display = "None"
		/// </summary>
		internal static void Setup(BaseValidator validator)
		{			
			validator.Display = ValidatorDisplay.None;
			validator.SetFocusOnError = false;
		}
		#endregion

		#region OnPreRender
		/// <summary>
		/// Entends OnPreRender method.
		/// </summary>
		internal static void OnPreRender(IValidatorExtension validator)
		{
			BaseValidator baseValidator = (BaseValidator)validator;

			if (baseValidator.Enabled && validator.ShowTooltip && String.IsNullOrEmpty(validator.ControlToValidate) && HttpContext.Current.IsDebuggingEnabled)
			{
				throw new HttpException(String.Format("Validator '{0}' should show tooltip but ControlToValidate is not specified.", baseValidator.ID));
			}

			// ensure requirements
			ClientScripts.BootstrapClientScriptHelper.RegisterBootstrapClientScript(baseValidator.Page);

			// register Validators Extensions script
			ScriptManager.ScriptResourceMapping.EnsureScriptRegistration(baseValidator.Page, ClientScripts.BootstrapClientScriptHelper.WebUIValidationExtensionScriptResourceMappingName);

			// register hookup script - in every request (must be included also in asynchronnous requests!)
			ScriptManager.RegisterStartupScript(baseValidator, typeof(ValidatorRenderExtender), "StartUp", "$(function() { Havit_Validation_StartUp(); });", true);
		}
		#endregion

		#region AddAttributesToRender
		/// <summary>
		/// Extends AddAttributesToRender method.
		/// </summary>
		internal static void AddAttributesToRender(IValidatorExtension validator, HtmlTextWriter writer)
		{
			// control to value invalid css class has meaning only when there is a control to validate
			if (!String.IsNullOrEmpty(validator.ControlToValidate))
			{
				Control controlToValidate = ((Control)validator).NamingContainer.FindControl(validator.ControlToValidate);
				// no check needed - ControlToValidate already checked

				ValidationDisplayTargetAttribute validationDisplayTargetAttribute = controlToValidate.GetType().GetCustomAttributes(typeof(ValidationDisplayTargetAttribute), true).Cast<ValidationDisplayTargetAttribute>().FirstOrDefault();
				if (validationDisplayTargetAttribute != null)
				{
					Control validationDisplayTarget = controlToValidate.FindControl(validationDisplayTargetAttribute.DisplayTargetControl);
					if (validationDisplayTarget == null)
					{
						throw new HttpException(String.Format("Control '{0}' defined in ValidationDisplayTargetAttribute not found in control '{1}'.",
							validationDisplayTargetAttribute.DisplayTargetControl,
							controlToValidate.ID));
					}

					writer.AddAttribute("data-val-validationdisplaytarget", validationDisplayTarget.ClientID);
				}

				// ensure rendering control to value invalid css class
				if (!String.IsNullOrEmpty(validator.ControlToValidateInvalidCssClass))
				{
					writer.AddAttribute("data-val-ctvclass", validator.ControlToValidateInvalidCssClass); // controltovalidate css class
				}

				// ensure rendering tooltip data
				if (validator.ShowTooltip)
				{
					string tooltip = validator.ToolTip;
					if (String.IsNullOrEmpty(tooltip))
					{
						tooltip = validator.Text;
					}
					if (String.IsNullOrEmpty(tooltip))
					{
						tooltip = validator.ErrorMessage;
					}

					if (!String.IsNullOrEmpty(tooltip))
					{
						writer.AddAttribute("data-val-tt-position", validator.TooltipPosition.ToString().ToLower());
						writer.AddAttribute("data-val-tt-text", tooltip);
					}
				}
			}
		}
		#endregion
	}
}
