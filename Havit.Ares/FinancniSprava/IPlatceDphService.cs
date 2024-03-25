namespace Havit.Ares.FinancniSprava;

/// <summary>
/// Třída PlatceDphService komunikuje s Web-Service MFCR pro zjištění Spolehlivosti plátce DPH a registrovaných podnikatelských bankovních účtů na MFCR.
/// </summary>
public interface IPlatceDphService
{
	/// <summary>
	/// Vrací strukturovanou odpověd Nespolehlivý plátce + Bankovní účty . Informace z MFCR. Hledání dle DIC (vcetne CZ prefixu)
	/// </summary>
	PlatceDphResult GetPlatceDph(string dic);

	/// <summary>
	/// Vrací strukturovanou odpověd Nespolehlivý plátce + Bankovní účty . Informace z MFCR. Hledání dle DIC (vcetne CZ prefixu). Asynchronní varianta.
	/// </summary>
	Task<PlatceDphResult> GetPlatceDphAsync(string dic, CancellationToken cancellationToken = default);
}