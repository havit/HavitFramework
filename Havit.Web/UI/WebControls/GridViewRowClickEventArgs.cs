using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Argumenty události RowClick controlu GridViewExt.
	/// </summary>
	public class GridViewRowClickEventArgs: CancelEventArgs
	{
		/// <summary>
		/// Index řádku, na kterém došlo k rozkliknutí.
		/// </summary>
		public int RowIndex { get; internal set; }
	}
}
