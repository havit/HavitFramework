namespace Havit.Drawing;

/// <summary>
/// Režimy pro ImageExt.Resize()
/// </summary>
public enum ResizeMode
{
	/// <summary>
	/// Upraví rozměry obrázku tak, aby se jeho vnější obrys vešel do požadovaných rozměrů. Poměr stran zachová.
	/// </summary>
	PreserveAspectRatioFitBox = 1,

	/// <summary>
	/// Pokud je obrázek větší než požadované rozměry, je redukován tak, aby se jeho vnější rozměr vešel do požadovaného boxu.
	/// Poměr stran je zachován. Pokud je obrázek menší, není zvětšen, ale zůstává nezměněn.
	/// </summary>
	PreserveAspectRatioFitBoxReduceOnly = 2,

	/// <summary>
	/// Obrázek se upraví na přesně požadované rozměry. V případě potřeby je natáhnut, nemusí být zachován poměr stran.
	/// </summary>
	AdjustToBox = 3
}
