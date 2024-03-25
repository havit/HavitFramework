using Havit.Ares.FinancniSprava;

namespace Havit.Ares.Ares;

/// <summary>
/// Zpracovane vysledky ze dvou volani ARES + PlatceDph (z MFCR). 
/// </summary>
public class AresDphResponse
{
	/// <summary>
	/// EkonomickySubjektItem - nalezena data v ARES. Null pokud nenalezeno. 
	/// </summary>
	public EkonomickySubjektItem EkonomickySubjektItem { get; set; }

	/// <summary>
	/// PlatceDphElement - nalezena data ve WebService MFCR - PlatceDph. Null pokud nenalezeno. 
	/// </summary>
	public PlatceDphResult PlatceDphElement { get; set; }
}
