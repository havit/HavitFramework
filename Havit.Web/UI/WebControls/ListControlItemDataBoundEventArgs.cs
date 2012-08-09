using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace Havit.Web.UI.WebControls
{
	public class ListControlItemDataBoundEventArgs : EventArgs
	{
		/// <summary>
		/// Prvek, kterého se událost týká.
		/// </summary>
		public ListItem Item
		{
			get
			{
				return this._item;
			}
		}
		private ListItem _item;

		/// <summary>
		/// Data, na jejich základì prvek vzniknul.
		/// </summary>
		public object DataItem
		{
			get
			{
				return _dataItem;
			}
			set
			{
				_dataItem = value;
			}
		}
		private object _dataItem;

		/// <summary>
		/// Vytvoøí instanci.
		/// </summary>
		/// <param name="item">Prvek, kterého se událost týká.</param>
		public ListControlItemDataBoundEventArgs(ListItem item, object dataItem)
		{
			this._item = item;
			this._dataItem = dataItem;
		}
	}
}
