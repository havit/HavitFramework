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
	internal class EnterpriseCheckBoxDropDownListPersisterControlExtender : IPersisterControlExtender
	{
		#region GetValue
		public object GetValue(Control control)
		{
			return ((EnterpriseCheckBoxDropDownList)control).SelectedIds;			
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
			EnterpriseCheckBoxDropDownList enterpriseCheckBoxList = (EnterpriseCheckBoxDropDownList)control;
			enterpriseCheckBoxList.SelectExistingItems(selectedIDs);			
		} 
		#endregion

		#region GetPriority
		public int? GetPriority(Control control)
		{
			if (control is EnterpriseCheckBoxDropDownList)
			{
				return 1;
			}
			return null;
		}
		#endregion

		#region PersistChilds
		/// <summary>
		/// Pokud je true, ControlsValuesPersister rekursivně projde i child controly.
		/// Implicitně vrací false.
		/// </summary>
		public bool PersistChildren(Control control)
		{
			return false;
		}
		#endregion
	}
}
