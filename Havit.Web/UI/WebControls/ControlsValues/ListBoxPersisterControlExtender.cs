using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Havit.Web.UI.WebControls.ControlsValues;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Havit.Web.UI.WebControls.ControlsValues;

/// <summary>
/// IPersisterControlExtender pro ListBox.
/// </summary>
internal class ListBoxPersisterControlExtender : IPersisterControlExtender
{
	public object GetValue(Control control)
	{
		return ((ListBox)control).Items.Cast<ListItem>().Where(item => item.Selected).Select(item => item.Value).ToArray();
	}

	public Type GetValueType()
	{
		return typeof(String[]);
	}

	public void SetValue(Control control, object value)
	{
		ListBox listBox = (ListBox)control;
		string[] values = (string[])value;

		listBox.ClearSelection();
		foreach (string item in values)
		{
			ListItem listItem = listBox.Items.FindByValue(item);
			if (listItem != null)
			{
				listItem.Selected = true;
			}
		}
	}

	public int? GetPriority(System.Web.UI.Control control)
	{
		if (control is ListBox)
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
