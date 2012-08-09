using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Havit.Web.UI.WebControls
{
	public class GridViewInsertEventArgs : CancelEventArgs
	{
		/// <summary>
		/// Vrací index øádku GridView, v kterém se odehrává Insert.
		/// </summary>
		public int RowIndex
		{
			get
			{
				return this._rowIndex;
			}
		}
		private int _rowIndex;

		/// <summary>
		/// Vytvoøí instanci.
		/// </summary>
		/// <param name="rowIndex">index øádku GridView, v kterém se odehrává Insert</param>
		public GridViewInsertEventArgs(int rowIndex)
			: base(false)
		{
			this._rowIndex = rowIndex;
		}

	}
}
