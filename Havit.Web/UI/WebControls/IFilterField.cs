using System.Web.UI.WebControls;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// Označuje field, který lze použít pro filtrování.
/// </summary>
public interface IFilterField
{
	/// <summary>
	/// Styl pro buňku filtru.
	/// Pokud FilterStyle není inicializován, inicializuje jej (lazy inicialization).
	/// </summary>
	TableItemStyle FilterStyle { get; }

	/// <summary>
	/// Styl pro buňku filtru.
	/// Pouze pro interní použití - tato vlastnost může vrátit null, pokud dosud FilterStyle není inicializován.
	/// </summary>
	TableItemStyle FilterStyleInternal { get; }

	/// <summary>
	/// Inicializuje buňku filtru.
	/// </summary>
	void InitializeFilterCell(DataControlFieldCell cell);
}
