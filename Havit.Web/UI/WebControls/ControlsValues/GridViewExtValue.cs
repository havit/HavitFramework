using Havit.Collections;

namespace Havit.Web.UI.WebControls.ControlsValues;

/// <summary>
/// Reprezentuje stav GridViewExt pro uložení persisterem.
/// </summary>
[Serializable]
public class GridViewExtValue
{
	/// <summary>
	/// Constructor.
	/// </summary>
	public GridViewExtValue()
	{
		AllowPaging = true; // default value (zpětná kompatibilita)
	}

	/// <summary>
	/// Indikuje povolení stránkovat.
	/// Nutné pro sledování v situaci, kdy uživatel klikne ve stránkování na zobrazení "vše".
	/// </summary>
	public bool AllowPaging
	{
		get;
		set;
	}

	/// <summary>
	/// Index zobrazené stránky dat.
	/// </summary>
	public int PageIndex
	{
		get;
		set;
	}

	/// <summary>
	/// Řazení.
	/// </summary>
	public SortItemCollection SortItems
	{
		get;
		set;
	}
}
