using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Havit.Web.UI.WebControls.ControlsValues;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Havit.Web.UI.WebControls.ControlsValues
{
	/// <summary>
	/// IPersisterControlExtender pro RadioButtonList.
	/// </summary>
	internal class RadioButtonListPersisterControlExtender : IPersisterControlExtender
	{
		#region GetValue
		public object GetValue(Control control)
		{
			return ((RadioButtonList)control).SelectedValue;
		} 
		#endregion

		#region GetValueType
		public Type GetValueType()
		{
			return typeof(String);
		} 
		#endregion

		#region SetValue
		public void SetValue(Control control, object value)
		{
			RadioButtonList radioButtonList = (RadioButtonList)control;
			radioButtonList.SelectedValue = (string)value;
		} 
		#endregion

		#region GetPriority
		public int? GetPriority(System.Web.UI.Control control)
		{
			if (control is RadioButtonList)
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
