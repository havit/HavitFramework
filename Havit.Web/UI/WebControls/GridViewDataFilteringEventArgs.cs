using System.Collections;
using System.Web.UI.WebControls;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// Argumenty události GridViewDataFiltering.
/// </summary>
public class GridViewDataFilteringEventArgs : EventArgs
{
	/// <summary>
	/// Data, která jsou bindována na grid.
	/// </summary>
	public IEnumerable Data { get; set; }

	/// <summary>
	/// Filtrační řádek gridu.
	/// </summary>
	public GridViewRow FilterRow { get; private set; }

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public GridViewDataFilteringEventArgs(IEnumerable data, GridViewRow filterRow)
	{
		this.Data = data;
		this.FilterRow = filterRow;
	}
}
