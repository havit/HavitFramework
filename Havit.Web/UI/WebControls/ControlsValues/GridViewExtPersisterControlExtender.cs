using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using Havit.Web.UI.WebControls.ControlsValues;
using Havit.Collections;
using System.Xml.Serialization;

namespace Havit.Web.UI.WebControls.ControlsValues
{
	/// <summary>
	/// IPersisterControlExtender pro GridViewExt.	
	/// </summary>	
	internal class GridViewExtPersisterControlExtender : IPersisterControlExtender
	{
		public object GetValue(Control control)
		{
			GridViewExt gridView = ((GridViewExt)control);
			GridViewExtValue gridViewExtValue = new GridViewExtValue();
			gridViewExtValue.SortItems = gridView.SortExpressions.SortItems;
			gridViewExtValue.PageIndex = gridView.PageIndex;
			gridViewExtValue.AllowPaging = gridView.AllowPaging;

			return gridViewExtValue;
		}

		public Type GetValueType()
		{
			return typeof(GridViewExtValue);
		}

		public void SetValue(System.Web.UI.Control control, object value)
		{
			GridViewExt gridView = (GridViewExt)control;			
			GridViewExtValue gridViewExtValue = (GridViewExtValue)value;

			gridView.AllowPaging = gridViewExtValue.AllowPaging;
			gridView.PageIndex = gridViewExtValue.PageIndex;
			gridView.SortExpressions.ClearSelection();

			foreach (SortItem sortItem in gridViewExtValue.SortItems)
			{
				gridView.SortExpressions.SortItems.Add(sortItem);
			}			
		}

		public int? GetPriority(Control control)
		{
			if (control is GridViewExt)
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
}
