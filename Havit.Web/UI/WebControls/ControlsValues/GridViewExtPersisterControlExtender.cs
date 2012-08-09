using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using Havit.Web.UI.WebControls.ControlsValues;
using Havit.Goran.WebBase.UI.WebControls.ControlsValues;
using Havit.Collections;
using System.Xml.Serialization;

namespace Havit.Web.UI.WebControls.ControlsValues
{
	/// <summary>
	/// IPersisterControlExtender pro GridViewExt.	
	/// </summary>	
	class GridViewExtPersisterControlExtender : IPersisterControlExtender
	{
		#region GetValue
		public object GetValue(Control control)
		{
			GridViewExtValue gridViewExtValue = new GridViewExtValue();
			gridViewExtValue.SortItems = ((GridViewExt)control).SortExpressions.SortItems;
			gridViewExtValue.PageIndex = ((GridViewExt)control).PageIndex;

			return gridViewExtValue;
		} 
		#endregion

		#region GetValueType
		public Type GetValueType()
		{
			return typeof(GridViewExtValue);
		} 
		#endregion

		#region SetValue
		public void SetValue(System.Web.UI.Control control, object value)
		{
			GridViewExt gridView = (GridViewExt)control;			
			GridViewExtValue gridViewExtValue = (GridViewExtValue)value;

			gridView.PageIndex = gridViewExtValue.PageIndex;
			gridView.SortExpressions.ClearSelection();

			foreach (SortItem sortItem in gridViewExtValue.SortItems)
			{
				gridView.SortExpressions.SortItems.Add(sortItem);
			}			
		}
		#endregion

		#region GetPriority
		public int? GetPriority(Control control)
		{
			if (control is GridViewExt)
			{
				return 1;
			}
			return null;
		}
		#endregion

	}	
}
