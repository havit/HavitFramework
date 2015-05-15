using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Havit.Web.UI;
using Havit.Web.UI.ClientScripts;

namespace Havit.Web.Bootstrap.UI.WebControls
{
	/// <summary>
	/// Extended ValidationSummary.
	/// </summary>
	public class ValidationSummary : System.Web.UI.WebControls.ValidationSummary
	{
		#region ShowToastr
		/// <summary>
		/// Indicates whether to show error messages in toastr.
		/// </summary>
		public bool ShowToastr
		{
			get { return (bool)(ViewState["ShowToastr"] ?? true); }
			set { ViewState["ShowToastr"] = value; }
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes ValidationSummary.
		/// </summary>
		public ValidationSummary()
		{
			this.ShowSummary = false;
			this.DisplayMode = ValidationSummaryDisplayMode.List;
			this.CssClass = "alert alert-danger";
		}
		#endregion

		#region OnPreRender
		/// <summary>
		/// Registers jquery and toastr scripts and script for displaying error message in toastr when ShowToastr is True.
		/// Registers message box (alert) script with error messages when ShowMessageBox is True.
		/// </summary>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			if (ShowToastr)
			{
				ScriptManager.ScriptResourceMapping.EnsureScriptRegistration(this.Page, "jquery");
				ScriptManager.ScriptResourceMapping.EnsureScriptRegistration(this.Page, "toastr");
				RegisterShowToastrScripts();
			}

			if (ShowMessageBox)
			{
				RegisterShowMessageBoxScripts();
			}
		}
		#endregion

		#region AddAttributesToRender
		/// <summary>
		/// Adds attribute "data-showtoastr" when ShowToastr is True.
		/// </summary>
		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			base.AddAttributesToRender(writer);

			if (ShowToastr)
			{
				writer.AddAttribute("data-showtoastr", "True");
			}
		}
		#endregion

		#region RegisterShowMessageBoxScripts
		/// <summary>
		/// Registers script for display validation error on client side page load.
		/// </summary>
		protected void RegisterShowMessageBoxScripts()
		{
			string[] errors = GetValidationErrorMessages();

			if (errors.Length > 0)
			{
				errors = errors.Select(item => item.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\"", "\\\"")).ToArray();
				string message = String.Join("\\r\\n", errors);
				string script = String.Format("$(function() {{ window.setTimeout(function() {{ alert('{0}'); }}, 1); }});", message);
				ScriptManager.RegisterStartupScript(this.Page, typeof(ValidationSummary), this.ClientID + "MessageBox", script, true);
			}

		}
		#endregion

		#region RegisterShowToastrScripts
		/// <summary>
		/// Registers script for display validation error on client side page load.
		/// Cannot be calculated on client sides because validators with has EnableClientScript set to false are not supported on the client side.
		/// </summary>
		protected void RegisterShowToastrScripts()
		{
			string[] errors = GetValidationErrorMessages();

			if (errors.Length > 0)
			{
				errors = errors.Select(item => item.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\"", "\\\"")).ToArray();
				string message = String.Join("<br />", errors);
				string script = String.Format("$(function() {{ Havit_ValidationSummary_ShowToastrError('{0}'); }});", message);
				ScriptManager.RegisterStartupScript(this.Page, typeof(ValidationSummary), this.ClientID + "Toastr", script, true);
			}
		}
		#endregion

		#region GetValidationErrorMessages
		/// <summary>
		/// Returns ErrorMessages for display in validation summary.
		/// Only messages of validators with failed validation (IsValid = false) are returned.
		/// </summary>
		protected string[] GetValidationErrorMessages()
		{
			List<string> lines = new List<string>();
			ValidatorCollection validators = Page.GetValidators(this.ValidationGroup);
			foreach (System.Web.UI.WebControls.BaseValidator validator in validators)
			{
				if (!validator.IsValid && !String.IsNullOrEmpty(validator.ErrorMessage))
				{
					lines.Add(validator.ErrorMessage);
				}
			}
			return lines.ToArray();
		}
		#endregion
	}
}
