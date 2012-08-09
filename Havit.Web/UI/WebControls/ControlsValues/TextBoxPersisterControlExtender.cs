using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Havit.Web.UI.WebControls.ControlsValues
{
	/// <summary>
	/// IPersisterControlExtender pro NumericBox.
	/// </summary>
	public class TextBoxPersisterControlExtender : IPersisterControlExtender
	{
		#region GetValue
		public object GetValue(System.Web.UI.Control control)
		{
			return ((TextBox)control).Text;
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
			((TextBox)control).Text = (string)value;
		} 
		#endregion

		#region GetPriority
		public int? GetPriority(Control control)
		{
			if (control is TextBox)
			{
				return 1;
			}
			return null;
		} 
		#endregion
	}
}
