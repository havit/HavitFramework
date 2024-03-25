namespace Havit.Ares.Ares;

/// <summary>
/// Jeden Subjekt (sro, OSVČ, a.s., apod.) vrácená z ARESu
/// </summary>
public class EkonomickySubjektItem
{
	/// <summary>
	/// struktura vrácená z ARES (automaticky generovaná)
	/// </summary>
	public EkonomickySubjekt EkonomickySubjektAres { get; set; }

	/// <summary>
	/// vypočítané hodnoty z ARES
	/// </summary>
	public EkonomickySubjektExtension EkonomickySubjektExtension { get; set; }
}