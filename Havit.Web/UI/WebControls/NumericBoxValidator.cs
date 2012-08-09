using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Havit.DsvCommerce.WebBase.UI.WebControls
{
	/// <summary>
	/// Validátor hodnoty controlu NumericBox.
	/// </summary>
	public class NumericBoxValidator: BaseValidator
	{
		#region MinValue
		/// <summary>
		/// Minimální hodnota, která je považována za platnou hodnotu.
		/// </summary>
		public decimal? MinValue
		{
			get { return (decimal?)ViewState["MinValue"]; }
			set { ViewState["MinValue"] = value; }
		}
		#endregion

		#region MaxValue
		/// <summary>
		/// Maximální hodnota, která je považována za platnou hodnotu.
		/// </summary>
		public decimal? MaxValue
		{
			get { return (decimal?)ViewState["MaxValue"]; }
			set { ViewState["MaxValue"] = value; }
		}
		#endregion

		#region EvaluateIsValid (overriden)
		/// <summary>
		/// Testuje platnost èísla.
		/// </summary>
		protected override bool EvaluateIsValid()
		{			
			Control control = FindControl(ControlToValidate);

			if (control == null)
			{
				throw new ArgumentException("ControlToValidate nebyl nalezen.", "ControlToValidate");
			}

			if (!(control is NumericBox))
			{
				throw new ArgumentException("ControlToValidate není NumericBox.", "ControlToValidate");
			}

			NumericBox numericBox = (NumericBox)control;

			// prázdná hodnota je OK
			if (numericBox.NumberText == String.Empty)
			{
				return true;
			}

			// zeptáme se, zda je èíslo vùbec èíslem a tudíž, jestli smime šáhnout na vlastnost Value		
			if (!numericBox.IsValid)
			{
				return false;
			}

			decimal? numericValue = numericBox.Value;

			if (numericValue == null)
			{
				return true;
			}

			// otestujeme záporná èísla
			if (!numericBox.AllowNegativeNumber && (numericValue < 0))
			{
				return false;
			}

			// testujeme minimální hodnotu
			if ((MinValue != null) && (numericValue < MinValue.Value))
			{
				return false;
			}

			// testujeme maximální hodnotu
			if ((MaxValue != null) && (numericValue > MaxValue.Value))
			{
				return false;
			}

			return true;
		}
		#endregion
	}
}
