﻿using System.Web.UI.WebControls;
using System.Web.UI;

namespace Havit.Web.UI.WebControls.ControlsValues;

/// <summary>
/// IPersisterControlExtender pro RadioButtonList.
/// </summary>
internal class RadioButtonListPersisterControlExtender : IPersisterControlExtender
{
	public object GetValue(Control control)
	{
		return ((RadioButtonList)control).SelectedValue;
	}

	public Type GetValueType()
	{
		return typeof(String);
	}

	public void SetValue(Control control, object value)
	{
		RadioButtonList radioButtonList = (RadioButtonList)control;
		radioButtonList.SelectedValue = (string)value;
	}

	public int? GetPriority(System.Web.UI.Control control)
	{
		if (control is RadioButtonList)
		{
			return 1;
		}
		return null;
	}

	/// <summary>
	/// Pokud je true, ControlsValuesPersister se pokusí uložit i hodnoty child controlů.
	/// Implicitně vrací false.
	/// </summary>
	public bool PersistsChildren(Control control)
	{
		return false;
	}
}
