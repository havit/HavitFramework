using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Havit.Web.UI.WebControls.ControlsValues;
using Havit.Web.UI.WebControls;
using System.Web.UI;

namespace Havit.Web.UI.WebControls.ControlsValues;

/// <summary>
/// IPersisterControlExtender pro EnterpriseCheckBoxList
/// </summary>
internal class EnterpriseCheckBoxDropDownListPersisterControlExtender : IPersisterControlExtender
{
	public object GetValue(Control control)
	{
		return ((EnterpriseCheckBoxDropDownList)control).SelectedIds;			
	}

	public Type GetValueType()
	{
		return typeof(int[]);
	}

	public void SetValue(Control control, object value)
	{			
		int[] selectedIDs = (int[])value;
		EnterpriseCheckBoxDropDownList enterpriseCheckBoxList = (EnterpriseCheckBoxDropDownList)control;
		enterpriseCheckBoxList.SelectExistingItems(selectedIDs);			
	}

	public int? GetPriority(Control control)
	{
		if (control is EnterpriseCheckBoxDropDownList)
		{
			return 1;
		}
		return null;
	}

	/// <summary>
	/// Pokud je true, ControlsValuesPersister rekursivně projde i child controly.
	/// Implicitně vrací false.
	/// </summary>
	public bool PersistsChildren(Control control)
	{
		return false;
	}
}
