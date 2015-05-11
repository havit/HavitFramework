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
		/// Vrátí CommandName tlačítka.
		/// </summary>
		public string CommandName { get; private set; }
		#endregion

		#region Visible
		/// <summary>
		/// Vlastnost Visible tlačítka.
		/// </summary>
		public bool Visible { get; set; }
		#endregion

		#region Enabled
		/// <summary>
		/// Vlastnosti Enabled tlačítka.
		/// </summary>
		public bool Enabled { get; set; }
		#endregion

		#region RowIndex
		/// <summary>
		/// Index řádku, kterého se událost týká.
		/// </summary>
		public int RowIndex { get; private set; }
		#endregion

		#region DataItem
		/// <summary>
		/// DataItem řádku, kterého se událost týká.
		/// </summary>
		public object DataItem { get; private set; }
		#endregion

		#region GridViewRowCustomizingCommandButtonEventArgs
		/// <summary>
		/// Vytvoří instanci třídy <see cref="GridViewRowCustomizingCommandButtonEventArgs"/>.
		/// </summary>
		/// <param name="commandName">CommandName obsluhovaného buttonu.</param>
		/// <param name="rowIndex">Index řádku.</param>
		/// <param name="dataItem">Datový objekt řádku.</param>
		public GridViewRowCustomizingCommandButtonEventArgs(string commandName, int rowIndex, object dataItem)
		{
			this.CommandName = commandName;
			this.RowIndex = rowIndex;
			this.DataItem = dataItem;
		}
		#endregion
	}
}
