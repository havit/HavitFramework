using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using Havit.Web.UI.WebControls.ControlsValues;

namespace Havit.Web.UI.WebControls.ControlsValues
{
	/// <summary>
	/// IPersisterControlExtender pro DateTimeBox.	
	/// </summary>
	public class DateTimeBoxPersisterControlExtender: IPersisterControlExtender
	{
		#region GetValue
		public object GetValue(Control control)
		{
			DateTimeBox dateTimeBox = ((DateTimeBox)control);
			if (!dateTimeBox.IsValid)
			{
				return null;
			}
			return dateTimeBox.Value;
		} 
		#endregion

		#region GetValueType
		public Type GetValueType()
		{
			return typeof(DateTime?);
		} 
		#endregion

		#region SetValue
		public void SetValue(Control control, object value)
		{
			DateTimeBox dateTimeBox = ((DateTimeBox)control);
			DateTime? dateTimeValue = (DateTime?)value;
			dateTimeBox.Value = dateTimeValue;
		} 
		#endregion

		#region GetPriority
		public int? GetPriority(Control control)
		{
			if (control is DateTimeBox)
			{
				return 1;
			}
			return null;
		} 
		#endregion
	}
}
