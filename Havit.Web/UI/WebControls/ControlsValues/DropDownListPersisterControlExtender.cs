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
		public object GetValue(System.Web.UI.Control control)
		{
			return ((DropDownList)control).SelectedValue;
		} 
		#endregion

		#region GetValueType
		public Type GetValueType()
		{			
			return typeof(String);
		} 
		#endregion

		#region SetValue
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
		public int? GetPriority(Control control)
		{
			if (control is DropDownList)
			{
				return 1;
			}
			return null;
		} 
		#endregion
	}
}
