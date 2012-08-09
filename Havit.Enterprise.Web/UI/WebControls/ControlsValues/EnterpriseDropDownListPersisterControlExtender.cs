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
		public object GetValue(System.Web.UI.Control control)
		{
			BusinessObjectBase selectedObject = ((EnterpriseDropDownList)control).SelectedObject;
			return (selectedObject == null) ? null : (int?)selectedObject.ID;
		}
		#endregion

		#region GetValueType
		public Type GetValueType()
		{
			return typeof(int);
		} 
		#endregion

		#region SetValue
		public void SetValue(System.Web.UI.Control control, object value)
		{
			int? valueToSet = (int?)value;
			EnterpriseDropDownList enterpriseDropDownList = (EnterpriseDropDownList)control;
			enterpriseDropDownList.SelectObjectIfPresent(valueToSet);
		}
		#endregion

		#region GetPriority
		public int? GetPriority(Control control)
		{
			if (control is EnterpriseDropDownList)
			{
				return 2;
			}
			return null;
		}
		#endregion
	}
}
