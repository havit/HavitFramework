using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using Havit.Web.UI.WebControls.ControlsValues;
using Havit.Business;

namespace Havit.Web.UI.WebControls.ControlsValues
{
	/// <summary>
	/// IPersisterControlExtender pro EnterpriseDropDownList.
	/// </summary>
	public class EnterpriseDropDownListPersisterControlExtender : IPersisterControlExtender
	{
		#region GetValue
		/// <summary>
		/// Získá hodnotu (stav) zadaného controlu.		
		/// </summary>
		public object GetValue(System.Web.UI.Control control)
		{
			BusinessObjectBase selectedObject = ((EnterpriseDropDownList)control).SelectedObject;
			return (selectedObject == null) ? null : (int?)selectedObject.ID;
		}
		#endregion

		#region GetValueType
		/// <summary>
		/// Získá typ hodnoty zadaného controlu.
		/// </summary>		
		public Type GetValueType()
		{
			return typeof(int);
		} 
		#endregion

		#region SetValue
		/// <summary>
		/// Nastaví hodnotu do controlu.
		/// </summary>
		public void SetValue(System.Web.UI.Control control, object value)
		{
			int? valueToSet = (int?)value;
			EnterpriseDropDownList enterpriseDropDownList = (EnterpriseDropDownList)control;
			enterpriseDropDownList.SelectObjectIfPresent(valueToSet);
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
			if (control is EnterpriseDropDownList)
			{
				return 2;
			}
			return null;
		}
		#endregion

		#region PersistChilds
		/// <summary>
		/// Pokud je true, ControlsValuesPersister rekursivně projde i child controly.
		/// Implicitně vrací false.
		/// </summary>
		public bool PersistChilds(Control control)
		{
			return false;
		}
		#endregion
	}
}
