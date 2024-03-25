namespace Havit.Ares.FinancniSprava;

/// <summary>
/// Služba pro volání ARES a DPH.
/// </summary>
public interface IAresDphService
{
	/// <summary>
	/// Kombinuje volání ARES a DPH. Hledá sekvenčně.  Nejprve ARES. Pokud v ARESu zjistí že je plátce DPH, tak volá i PlatceDph.
	/// </summary>
	AresDphResponse GetAresAndPlatceDph(string ico);

	/// <summary>
	/// Kombinuje volání ARES a DPH. Hledá sekvenčně.  Nejprve ARES. Pokud v ARESu zjistí že je plátce DPH, tak volá i PlatceDph. Asynchronní varianta.
	/// </summary>
	Task<AresDphResponse> GetAresAndPlatceDphAsync(string ico, CancellationToken cancellationToken = default);
}