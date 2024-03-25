namespace Havit.Ares;


/// Základní třída vrácená z volání ARES (podle názvu).   
public class EkonomickeSubjektyResult
{
	/// Počet nalezených subjektů v ARES.  Může jich být víc než vrácených. Max. počet co ARES vrátí je 1000. 
	public long PocetCelkem { get; set; }

	/// List všech nalezených subjektů z ARES 
	public List<EkonomickySubjektItem> Items { get; set; }

}

