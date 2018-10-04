using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.Web.UI.WebControls.ControlsValues
{
	/// <summary>
	/// IPersisterControlExtender pro CheckBox.
	/// RadioButton dědí z CheckBoxu, proto je v důsledku použito i pro zpracování hodnoty RadioButtonu.
	/// </summary>
	public class CheckBoxPersisterControlExtender : IPersisterControlExtender
	{
		#region GetValue
		/// <summary>
		/// Získá hodnotu (stav) zadaného controlu.		
		/// </summary>
		public object GetValue(Control control)
		{
			return ((CheckBox)control).Checked;
		} 
		#endregion

		#region GetValueType
		/// <summary>
		/// Získá typ hodnoty zadaného controlu.
		/// </summary>		
		public Type GetValueType()
		{
			return typeof(bool);
		} 
		#endregion

		#region SetValue
		/// <summary>
		/// Nastaví hodnotu do controlu.
		/// </summary>
		public void SetValue(Control control, object value)
		{
			((CheckBox)control).Checked = (bool)value;
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
		public int? GetPriority(Control control)
		{
			if (control is CheckBox)
			{
				return 1;
			}
			return null;
		}
		#endregion

		#region PersistsChildren
		/// <summary>
		/// Pokud je true, ControlsValuesPersister se pokusí uložit i hodnoty child controlů.
		/// Implicitně vrací false.
		/// </summary>
		public bool PersistsChildren(Control control)
		{
			return false;
		}
		#endregion
	}
}
