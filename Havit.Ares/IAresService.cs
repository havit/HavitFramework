
namespace Havit.Ares;

/// <summary>
/// Rozhraní service pro volání ARES služeb.
/// </summary>
public interface IAresService
{
	/// <summary>
	/// Vyhledání seznamu ekonomických subjektů ARES dle Ico.
	/// </summary>
	/// <param name="ico"></param>
	/// <returns>EkonomickySubjektItem. Vrací jeden subjekt nebo null - pokud nenalezeno </returns>
	EkonomickySubjektItem GetEkonomickeSubjektyDleIco(string ico);
	/// <summary>
	/// Vyhledání seznamu ekonomických subjektů ARES dle Ico. Asynchronní varianta.
	/// </summary>
	/// <param name="ico"></param>
	/// <param name="cancellationToken"></param>
	/// <returns>EkonomickySubjektItem. Vrací jeden subjekt nebo null - pokud nenalezeno </returns>
	Task<EkonomickySubjektItem> GetEkonomickeSubjektyDleIcoAsync(string ico, CancellationToken cancellationToken = default);

	/// <summary>
	/// Vyhledání seznamu ekonomických subjektů ARES dle Názvu (Obchodní jméno).
	/// </summary>
	/// <param name="obchodniJmeno"> ARES Hledá dle slov. Tj. hledání třeba dle ERB a ERBAN vrátí jiné výsledky</param>
	/// <param name="maxResults">Implicitně 100. ARES vrací maximálně 1000 subjektů - jinak chyba</param>
	/// <returns>EkonomickeSubjektyResult. Vrací List Subjektů. Nebo null </returns>
	EkonomickeSubjektyResult GetEkonomickeSubjektyDleObchodnihoJmena(string obchodniJmeno, int maxResults = AresService.DefaultMaxResults);

	/// <summary>
	/// Vyhledání seznamu ekonomických subjektů ARES dle Názvu (Obchodní jméno). Asynchronní varianta
	/// </summary>
	/// <param name="obchodniJmeno"> ARES Hledá dle slov. Tj. hledání třeba dle ERB a ERBAN vrátí jiné výsledky</param>
	/// <param name="maxResults">Implicitně 100. ARES vrací maximálně 1000 subjektů - jinak chyba</param>
	/// <param name="cancellationToken"></param>
	/// <returns>EkonomickeSubjektyResult. Vrací List Subjektů. Nebo null </returns>
	Task<EkonomickeSubjektyResult> GetEkonomickeSubjektyDleObchodnihoJmenaAsync(string obchodniJmeno, int maxResults = AresService.DefaultMaxResults, CancellationToken cancellationToken = default);

}