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
		public object GetValue(Control control)
		{
			return ((CheckBox)control).Checked;
		} 
		#endregion

		#region GetValueType
		public Type GetValueType()
		{
			return typeof(bool);
		} 
		#endregion

		#region SetValue
		public void SetValue(Control control, object value)
		{
			((CheckBox)control).Checked = (bool)value;
		} 
		#endregion

		#region GetPriority
		public int? GetPriority(Control control)
		{
			if (control is CheckBox)
			{
				return 1;
			}
			return null;
		} 
		#endregion
	}
}
