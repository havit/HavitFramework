namespace Havit.Ares;

/// <summary>
/// Rozhraní service pro volání ARES služeb.
/// </summary>
public interface IAresService
{
	/// <summary>
	/// Vyhledání seznamu ekonomických subjektů ARES dle Ico.
	/// </summary>
	EkonomickySubjektItem GetEkonomickeSubjektyDleIco(string ico);

	/// <summary>
	/// Vyhledání seznamu ekonomických subjektů ARES dle Ico. Asynchronní varianta.
	/// </summary>
	Task<EkonomickySubjektItem> GetEkonomickeSubjektyDleIcoAsync(string ico, CancellationToken cancellationToken = default);

	/// <summary>
	/// Vyhledání seznamu ekonomických subjektů ARES dle Názvu (Obchodní jméno).
	/// </summary>
	/// <param name="obchodniJmeno"> ARES Hledá dle slov. Tj. hledání třeba dle ERB a ERBAN vrátí jiné výsledky</param>
	/// <param name="maxResults">Implicitně 100. ARES vrací maximálně 1000 subjektů - jinak chyba</param>
	EkonomickeSubjektyResult GetEkonomickeSubjektyDleObchodnihoJmena(string obchodniJmeno, int maxResults = AresService.DefaultMaxResults);

	/// <summary>
	/// Vyhledání seznamu ekonomických subjektů ARES dle Názvu (Obchodní jméno). Asynchronní varianta
	/// </summary>
	/// <param name="obchodniJmeno"> ARES Hledá dle slov. Tj. hledání třeba dle ERB a ERBAN vrátí jiné výsledky</param>
	/// <param name="maxResults">Implicitně 100. ARES vrací maximálně 1000 subjektů - jinak chyba</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	Task<EkonomickeSubjektyResult> GetEkonomickeSubjektyDleObchodnihoJmenaAsync(string obchodniJmeno, int maxResults = AresService.DefaultMaxResults, CancellationToken cancellationToken = default);

}