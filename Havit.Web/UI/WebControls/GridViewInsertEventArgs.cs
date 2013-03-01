using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Parametry události vložení záznamu do GridView.
	/// </summary>
	public class GridViewInsertEventArgs : CancelEventArgs
	{
		#region RowIndex
		/// <summary>
		/// Vrací index řádku GridView, v kterém se odehrává Insert.
		/// </summary>
		public int RowIndex
		{
			get
			{
				return this._rowIndex;
			}
		}
		private int _rowIndex;
		#endregion

		#region Constructor
		/// <summary>
		/// Vytvoří instanci.
		/// </summary>
		/// <param name="rowIndex">index řádku GridView, v kterém se odehrává Insert</param>
		public GridViewInsertEventArgs(int rowIndex)
			: base(false)
		{
			this._rowIndex = rowIndex;
		}
		#endregion

	}
}
