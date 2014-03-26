using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.Web.Bootstrap.UI.WebControls
{
	/// <summary>
	/// Extended ValidationSummary.
	/// </summary>
	public class ValidationSummary : System.Web.UI.WebControls.ValidationSummary
	{
		#region OnInit
		/// <summary>
		/// Initializes ValidationSummary.
		/// </summary>
		protected override void OnInit(EventArgs e)
		{
			this.DisplayMode = ValidationSummaryDisplayMode.List;
			this.CssClass = "alert alert-danger";

			base.OnInit(e);
		}
		#endregion

		#region OnPreRender
		/// <summary>
		/// PreRender method.
		/// </summary>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			if (ShowMessageBox)
			{
				RegisterShowMessageBoxScripts();
			}
		}
		#endregion

		#region RegisterShowMessageBoxScripts
		/// <summary>
		/// Registers script for display validation error on client side page load.
		/// </summary>
		protected void RegisterShowMessageBoxScripts()
		{
			List<string> lines = new List<string>();
			ValidatorCollection validators = Page.GetValidators(this.ValidationGroup);
			foreach (BaseValidator validator in validators)
			{
				if (!validator.IsValid)
				{
					lines.Add(validator.ErrorMessage.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\"", "\\\""));
				}
			}

			if (lines.Count > 0)
			{
				string message = String.Join("\\r\\n", lines);
				string script = String.Format("$(function() {{ alert('{0}'); }});", message);
				ScriptManager.RegisterStartupScript(this, typeof(ValidationSummary), this.ClientID + "MessageBox", script, true);
			}

		}
		#endregion
	}
}
