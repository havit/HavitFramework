using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Validátor, který øeší povinnost zaškrtnutí (eventuelnì odškrtnutí - viz vlastnost ValidCheckedState) CheckBoxu.
	/// </summary>
    public class CheckBoxValidator : BaseValidator
	{
		#region ValidCheckedState
		/// <summary>
		/// Udává validní hodnotu - zaškrtnutý [default] nebo odškrtnutý.
		/// </summary>
		public bool ValidCheckedState
    	{
    		get
    		{
    			return (bool)(ViewState["ValidCheckedState"] ?? true);
    		}
    		set
    		{
    			ViewState["ValidCheckedState"] = value;
    		}
    	}
    	#endregion

		#region CheckBoxToValidate
		/// <summary>
		/// CheckBox, který se bude validovat, získán z ControlToValidate.
		/// </summary>
		protected CheckBox CheckBoxToValidate
    	{
    		get
    		{
    			if (_checkBoxToValidate == null)
    			{
    				_checkBoxToValidate = FindControl(this.ControlToValidate) as CheckBox;
    			}
                
    			return _checkBoxToValidate;
    		}
    	}
    	private CheckBox _checkBoxToValidate = null;
    	#endregion

		#region ControlPropertiesValid
		/// <summary>
		/// Zkontroluje, zda je validátor nastaven správnì, jinak vyhodí vyjímku.
		/// </summary>
		protected override bool ControlPropertiesValid()
    	{
    		if (String.IsNullOrEmpty(ControlToValidate))
    		{
    			throw new HttpException(string.Format("Vlastnost ControlToValidate controlu '{0}' nesmí být prázdná.", this.ID));
    		}

    		if (this.CheckBoxToValidate == null)
    		{
    			throw new HttpException(string.Format("CheckBoxValidator mùže validovat pouze controly typu CheckBox."));
    		}

    		return true;
    	}
    	#endregion

		#region EvaluateIsValid
		/// <summary>
		/// Vyhodnotí validátor
		/// </summary>
		protected override bool EvaluateIsValid()
    	{
    		return CheckBoxToValidate.Checked == ValidCheckedState;
    	}
    	#endregion

		#region AddAttributesToRender
		/// <summary>
		/// Pøidá renderované atributy potøebné pro klientskou validaci
		/// </summary>
		protected override void AddAttributesToRender(HtmlTextWriter writer)
    	{
    		base.AddAttributesToRender(writer);

			if (EnableClientScript)
			{
				writer.AddAttribute("evaluationfunction", "CheckBoxValidatorEvaluateIsValid", false);
				writer.AddAttribute("validCheckedState", ValidCheckedState ? "true" : "false", false);
			}
    	}
    	#endregion

		#region OnPreRender
		protected override void OnPreRender(EventArgs e)
    	{
    		base.OnPreRender(e);

    		if (EnableClientScript)
    		{
    			ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "Havit.Web.UI.WebControls.CheckBoxValidator", validationScript, true);
    		}
    	}
    	private const string validationScript = @"
				<script type=""text/javascript"">
				function CheckBoxValidatorEvaluateIsValid(val)
				{
					var control = document.getElementById(val.controltovalidate);
					var validCheckedState = Boolean(val.validCheckedState == 'true');

					return control.checked == validCheckedState;
				}
				</script>
				";
		#endregion
	}
}
