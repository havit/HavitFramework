using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using Havit.Web.UI.WebControls.ControlsValues;

namespace Havit.Web.UI.WebControls.ControlsValues
{
	/// <summary>
	/// IPersisterControlExtender pro DropDownList.
	/// </summary>
	public class DropDownListPersisterControlExtender : IPersisterControlExtender
	{
		#region GetValue
		/// <summary>
		/// Získá hodnotu (stav) zadaného controlu.		
		/// </summary>
		public object GetValue(System.Web.UI.Control control)
		{
			return ((DropDownList)control).SelectedValue;
		} 
		#endregion

		#region GetValueType
		/// <summary>
		/// Získá typ hodnoty zadaného controlu.
		/// </summary>		
		public Type GetValueType()
		{			
			return typeof(String);
		} 
		#endregion

		#region SetValue
		/// <summary>
		/// Nastaví hodnotu do controlu.
		/// </summary>
		public void SetValue(System.Web.UI.Control control, object value)
		{
			string selectedValue = (string)value;
			DropDownList dropDownList = (DropDownList)control;

			ListItem item = dropDownList.Items.FindByValue(selectedValue);
			if (item != null)
			{
				dropDownList.ClearSelection();
				item.Selected = true;
			}
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
			if (control is DropDownList)
			{
				return 1;
			}
			return null;
		}
		#endregion

		#region PersistChildren
		/// <summary>
		/// Pokud je true, ControlsValuesPersister se pokusí uložit i hodnoty child controlů.
		/// Implicitně vrací false.
		/// </summary>
		public bool PersistChildren(Control control)
		{
			return false;
		}
		#endregion
	}
}
