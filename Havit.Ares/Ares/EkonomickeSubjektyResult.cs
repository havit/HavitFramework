namespace Havit.Ares.Ares;

/// <summary>
/// Základní třída vrácená z volání ARES (podle názvu).   
/// </summary>
public class EkonomickeSubjektyResult
{
	/// <summary>
	/// Počet nalezených subjektů v ARES.  Může jich být víc než vrácených. Max. počet co ARES vrátí je 1000. 
	/// </summary>
	public long PocetCelkem { get; set; }

	/// <summary>
	/// List všech nalezených subjektů z ARES 
	/// </summary>
	public List<EkonomickySubjektItem> Items { get; set; }

}

