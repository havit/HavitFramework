
namespace Havit.Ares;

/// <summary>
/// Třída PlatceDphService komunikuje s Web-Service MFCR pro zjištění Spolehlivosti plátce DPH a registrovaných podnikatelských bankovních účtů na MFCR.
/// </summary>
public interface IPlatceDphService
{
	/// <remarks>
	/// Vrací strukturovanou odpověd Nespolehlivý plátce + Bankovní účty . Informace z MFCR. Hledání dle DIC (vcetne CZ prefixu)
	/// </remarks>
	/// <returns>PlatceDphResponse</returns>
	PlatceDphResponse GetPlatceDph(string dic);

	/// <summary>
	/// Vrací strukturovanou odpověd Nespolehlivý plátce + Bankovní účty . Informace z MFCR. Hledání dle DIC (vcetne CZ prefixu). Asynchronní varianta.
	/// </summary>
	/// <returns>PlatceDphResponse</returns>
	Task<PlatceDphResponse> GetPlatceDphAsync(string dic, CancellationToken cancellationToken = default);
}