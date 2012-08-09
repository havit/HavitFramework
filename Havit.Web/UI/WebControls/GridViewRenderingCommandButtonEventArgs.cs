using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Argumenty události <see cref="GridViewExt.RowCustomizingCommandButton"/>.
	/// </summary>
	public class GridViewRowCustomizingCommandButtonEventArgs : EventArgs
	{
		#region CommandName
		/// <summary>
		/// Vrátí CommandName tlaèítka.
		/// </summary>
		public string CommandName
		{
			get
			{
				return _commandName;
			}
		}
		private string _commandName;
		#endregion

		#region Visible
		/// <summary>
		/// Vlastnost Visible tlaèítka.
		/// </summary>
		public bool Visible
		{
			get
			{
				return _visible;
			}
			set
			{
				_visible = value;
			}
		}
		private bool _visible;
		#endregion

		#region Enabled
		/// <summary>
		/// Vlastnosti Enabled tlaèítka.
		/// </summary>
		public bool Enabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				_enabled = value;
			}
		}
		private bool _enabled;
		#endregion

		#region RowIndex
		/// <summary>
		/// Index øádku, kterého se událost týká.
		/// </summary>
		public int RowIndex
		{
			get
			{
				return _rowIndex;
			}
		}
		private int _rowIndex;
		#endregion

		#region DataItem
		/// <summary>
		/// DataItem øádku, kterého se událost týká.
		/// </summary>
		public object DataItem
		{
			get
			{
				return _dataItem;
			}
		}
		private object _dataItem;
		#endregion

		#region GridViewRowCustomizingCommandButtonEventArgs
		/// <summary>
		/// Vytvoøí instanci tøídy <see cref="GridViewRowCustomizingCommandButtonEventArgs"/>.
		/// </summary>
		/// <param name="commandName">CommandName obsluhovaného buttonu</param>
		public GridViewRowCustomizingCommandButtonEventArgs(string commandName, int rowIndex, object dataItem)
		{
			this._commandName = commandName;
			this._rowIndex = rowIndex;
			this._dataItem = dataItem;
		}
		#endregion
	}
}
