﻿using System.Web.UI.WebControls;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// Argumenty události oznamující nabidnování hodnoty do seznamového prvku (DropDownList, CheckBoxList, EnumDropDownList, apod.)
/// </summary>
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
	private readonly ListItem _item;

	/// <summary>
	/// Data, na jejich základě prvek vzniknul.
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
	/// Vytvoří instanci.
	/// </summary>
	public ListControlItemDataBoundEventArgs(ListItem item, object dataItem)
	{
		this._item = item;
		this._dataItem = dataItem;
	}
}
