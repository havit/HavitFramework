using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Havit.Web.UI.WebControls.ControlsValues;
using Havit.Web.UI.WebControls;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Havit.Web.UI.WebControls.ControlsValues
{
	/// <summary>
	/// IPersisterControlExtender pro EnumDropDownList.
	/// </summary>
	internal class EnumDropDownListPersisterControlExtender : IPersisterControlExtender
	{
		public object GetValue(Control control)
		{
			return ((EnumDropDownList)control).SelectedEnumValue;
		}

		public Type GetValueType()
		{
			return typeof(String);
		}

		public void SetValue(Control control, object value)
		{
			EnumDropDownList enumDropDownList = (EnumDropDownList)control;
			
			enumDropDownList.ClearSelection();
			if (value != null)
			{
				enumDropDownList.SelectedEnumValue = Enum.Parse(enumDropDownList.EnumType, value.ToString());		
			}
		}

		public int? GetPriority(System.Web.UI.Control control)
		{
			if (control is EnumDropDownList)
			{
				return 2;
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
}
