using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Argumenty události GridViewDataFiltering.
	/// </summary>
	public class GridViewDataFilteringEventArgs : EventArgs
	{
		#region Data
		/// <summary>
		/// Data, která jsou bindována na grid.
		/// </summary>
		public IEnumerable Data { get; set; }
		#endregion

		#region FilterRow
		/// <summary>
		/// Filtrační řádek gridu.
		/// </summary>
		public GridViewRow FilterRow { get; private set; }
		#endregion

		#region FilterGridViewDataEventArgs
		/// <summary>
		/// Konstruktor.
		/// </summary>
		public GridViewDataFilteringEventArgs(IEnumerable data, GridViewRow filterRow)
		{
			this.Data = data;
			this.FilterRow = filterRow;
		}
		#endregion
	}
}
