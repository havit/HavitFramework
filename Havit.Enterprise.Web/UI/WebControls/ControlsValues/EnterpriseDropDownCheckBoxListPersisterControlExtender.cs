using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Havit.Web.UI.WebControls.ControlsValues;
using Havit.Web.UI.WebControls;
using System.Web.UI;

namespace Havit.Web.UI.WebControls.ControlsValues
{
	/// <summary>
	/// IPersisterControlExtender pro EnterpriseCheckBoxList
	/// </summary>
	internal class EnterpriseDropDownCheckBoxListPersisterControlExtender : IPersisterControlExtender
	{
		#region GetValue
		public object GetValue(Control control)
		{
			return ((EnterpriseDropDownCheckBoxList)control).SelectedIds;			
		} 
		#endregion

		#region GetValueType
		public Type GetValueType()
		{
			return typeof(int[]);
		} 
		#endregion

		#region SetValue
		public void SetValue(Control control, object value)
		{			
			int[] selectedIDs = (int[])value;
			EnterpriseDropDownCheckBoxList enterpriseCheckBoxList = (EnterpriseDropDownCheckBoxList)control;
			enterpriseCheckBoxList.SelectExistingItems(selectedIDs);			
		} 
		#endregion

		#region GetPriority
		public int? GetPriority(Control control)
		{
			if (control is EnterpriseDropDownCheckBoxList)
			{
				return 1;
			}
			return null;
		} 
		#endregion		
	}
}
