
namespace Havit.Ares;

public interface IAresService
{
	EkonomickySubjektItem GetEkonomickeSubjektyDleIco(string ico);
	Task<EkonomickySubjektItem> GetEkonomickeSubjektyDleIcoAsync(string ico, CancellationToken cancellationToken = default);

	EkonomickeSubjektyResult GetEkonomickeSubjektyDleObchodnihoJmena(string obchodniJmeno, int maxResults = AresService.DefaultMaxResults);
	Task<EkonomickeSubjektyResult> GetEkonomickeSubjektyDleObchodnihoJmenaAsync(string obchodniJmeno, int maxResults = AresService.DefaultMaxResults, CancellationToken cancellationToken = default);

	PlatceDphResponse GetPlatceDph(string dic);
	Task<PlatceDphResponse> GetPlatceDphAsync(string dic, CancellationToken cancellationToken = default);

	AresDphResponse GetAresAndPlatceDph(string ico);
	Task<AresDphResponse> GetAresAndPlatceDphAsync(string ico, CancellationToken cancellationToken = default);
}