namespace Havit.Web.UI.WebControls;

/// <summary>
/// Argumenty události <see cref="GridViewExt.RowCustomizingCommandButton"/>.
/// </summary>
public class GridViewRowCustomizingCommandButtonEventArgs : EventArgs
{
	/// <summary>
	/// Vrátí CommandName tlačítka.
	/// </summary>
	public string CommandName { get; private set; }

	/// <summary>
	/// Vlastnost Visible tlačítka.
	/// </summary>
	public bool Visible { get; set; }

	/// <summary>
	/// Vlastnosti Enabled tlačítka.
	/// </summary>
	public bool Enabled { get; set; }

	/// <summary>
	/// Index řádku, kterého se událost týká.
	/// </summary>
	public int RowIndex { get; private set; }

	/// <summary>
	/// DataItem řádku, kterého se událost týká.
	/// </summary>
	public object DataItem { get; private set; }

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
}
