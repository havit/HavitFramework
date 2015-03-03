using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Web.UI.WebControls.ControlsValues
{
	/// <summary>
	/// IPersisterControlExtender pro AutoCompleteTextBox.
	/// </summary>
	public class AutoCompleteTextBoxControlExtender : IPersisterControlExtender
	{
		#region GetValue
		/// <summary>
		/// Získá hodnotu (stav) zadaného controlu.
		/// </summary>
		public object GetValue(System.Web.UI.Control control)
		{
			AutoCompleteTextBox box = (AutoCompleteTextBox)control;
			return new AutoCompleteTextBoxValues()
			{
				SelectedText = box.SelectedText,
				SelectedValue = box.SelectedValue
			};
		}
		#endregion

		#region GetValueType
		/// <summary>
		/// Získá typ hodnoty zadaného controlu.
		/// </summary>
		public Type GetValueType()
		{
			return typeof(Tuple<string, string>);
		}
		#endregion

		#region SetValue
		/// <summary>
		/// Nastaví hodnotu do controlu.
		/// </summary>
		public void SetValue(System.Web.UI.Control control, object value)
		{
			AutoCompleteTextBox box = (AutoCompleteTextBox)control;
			AutoCompleteTextBoxValues data = (AutoCompleteTextBoxValues)value;
			box.SelectedValue = data.SelectedValue;
			box.SelectedText = data.SelectedText;
		}
		#endregion

		#region GetPriority
		/// <summary>
		/// Vrací prioritu se kterou je tento IPersisterControlExtender použitelný
		/// pro zpracování daného controlu.
		/// Slouží k řešení situací, kdy potřebujeme předka a potomka zpracovávat jinak
		/// (např. DropDownList a EnterpriseDropDownList - registrací na typ DDL
		/// se automaticky chytneme i na EDDL, proto v extenderu pro EDDL použijeme
		/// vyšší prioritu).
		/// Vyšší priorita vyhravá.Není-li control extender použitelný, nechť vrací null.
		/// </summary>
		public int? GetPriority(System.Web.UI.Control control)
		{
			if (control is AutoCompleteTextBox)
			{
				return 1;
			}
			return null;
		}
		#endregion
	}
}