using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using Havit.Web.UI;

[assembly: WebResource("Havit.Web.Bootstrap.UI.WebControls.WebUIValidationExtension.js", "application/javascript")]
namespace Havit.Web.Bootstrap.UI.WebControls
{
	/// <summary>
	/// Helper class for rendering Validator Extensions.
	/// </summary>
	internal static class ValidatorRenderExtender
	{
		#region Setup
		/// <summary>
		/// Sets up validator (used from constructor).
		/// Changes validator default values to:
		/// - Display = "None"
		/// - SetFocusOnError = true
		/// </summary>
		internal static void OnInit(BaseValidator validator)
		{			
			validator.Display = ValidatorDisplay.None;
			validator.SetFocusOnError = true;
		}
		#endregion

		#region OnPreRender
		/// <summary>
		/// Entends OnPreRender method.
		/// </summary>
		internal static void OnPreRender(BaseValidator validator)
		{
			// ensure requirements
			ClientScripts.BootstrapClientScriptHelper.RegisterBootstrapClientScript(validator.Page);

			// register Validators Extensions script
			ScriptManager.ScriptResourceMapping.EnsureScriptRegistrationForEmbeddedResource(validator, typeof(ValidatorRenderExtender), "Havit.Web.Bootstrap.UI.WebControls.WebUIValidationExtension.js");

			// register hookup script - in every request (must be included also in asynchronnous requests!)
			ScriptManager.RegisterStartupScript(validator, typeof(ValidatorRenderExtender), "StartUp", "$(function() { Havit_EnsureValidatorsExtensionsHookup(); });", true);
		}
		#endregion

		#region AddAttributesToRender
		/// <summary>
		/// Extends AddAttributesToRender method.
		/// </summary>
		internal static void AddAttributesToRender(IValidatorExtension validator, HtmlTextWriter writer)
		{
			if (validator.IsEnabled && validator.RenderUpLevel)
			{
				// control to value invalid css class has meaning only when there is a control to validate
				if (!String.IsNullOrEmpty(validator.ControlToValidate))
				{
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
						writer.AddAttribute("data-val-tt-position", validator.TooltipPosition.ToString().ToLower());
						writer.AddAttribute("data-val-tt-text", tooltip);
					}
				}
			}
		}
		#endregion
	}
}
