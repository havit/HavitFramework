using System.ComponentModel;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// Argumenty události RowClick controlu GridViewExt.
/// </summary>
public class GridViewRowClickEventArgs : CancelEventArgs
{
	/// <summary>
	/// Index řádku, na kterém došlo k rozkliknutí.
	/// </summary>
	public int RowIndex { get; internal set; }
}
