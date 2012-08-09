using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using Havit.Web.UI.WebControls.ControlsValues;

namespace Havit.Web.UI.WebControls.ControlsValues
{
	/// <summary>
	/// IPersisterControlExtender pro NumericBox.
	/// </summary>
	public class NumericBoxPersisterControlExtender: IPersisterControlExtender
	{
		#region GetValue
		public object GetValue(Control control)
		{
			NumericBox numericBox = ((NumericBox)control);
			if (!numericBox.IsValid)
			{
				return null;
			}
			return numericBox.Value;
		}
		#endregion

		#region GetValueType
		public Type GetValueType()
		{
			return typeof(Decimal);
		} 
		#endregion

		#region SetValue
		public void SetValue(System.Web.UI.Control control, object value)
		{
			NumericBox numericBox = ((NumericBox)control);
			decimal? decimalValue = (decimal?)value;
			numericBox.Value = decimalValue;
		} 
		#endregion

		#region GetPriority
		public int? GetPriority(Control control)
		{
			if (control is NumericBox)
			{
				return 1;
			}
			return null;
		} 
		#endregion
	}
}
