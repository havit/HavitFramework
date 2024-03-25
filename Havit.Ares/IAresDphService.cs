
namespace Havit.Ares;


/// <summary>
/// Služba pro volání ARES a DPH.
/// </summary>
public interface IAresDphService
{
	/// <summary>
	/// Kombinuje volání ARES a DPH. Hledá sekvenčně.  Nejprve ARES. Pokud v ARESu zjistí že je plátce DPH, tak volá i PlatceDph.
	/// </summary>
	/// <param name="ico"></param>
	/// <returns>AresDphResponse</returns>
	AresDphResponse GetAresAndPlatceDph(string ico);

	/// <summary>
	/// Kombinuje volání ARES a DPH. Hledá sekvenčně.  Nejprve ARES. Pokud v ARESu zjistí že je plátce DPH, tak volá i PlatceDph. Asynchronní varianta.
	/// </summary>
	/// <param name="ico"></param>
	/// <param name="cancellationToken"></param>
	/// <returns>AresDphResponse</returns>
	Task<AresDphResponse> GetAresAndPlatceDphAsync(string ico, CancellationToken cancellationToken = default);
}