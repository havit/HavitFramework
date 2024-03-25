namespace Havit.Ares;

/// Jeden Subjekt (sro, OSVČ, a.s., apod.) vrácená z ARESu
public class EkonomickySubjektItem
{
	/// struktura vrácená z ARES (automaticky generovaná)
	public EkonomickySubjekt EkonomickySubjektAres { get; set; }
	/// vypočítané hodnoty z ARES
	public EkonomickySubjektExtension EkonomickySubjektExtension { get; set; }
}