
namespace Havit.Services.Ares;

public interface IAresService
{
	int MaxResults { get; set; }

	EkonomickeSubjektyResponse GetEkonomickeSubjektyFromIco(string ico);
	EkonomickeSubjektyResponse GetEkonomickeSubjektyFromIcoOrObchodniJmeno(string ico, string obchodniJmeno);
	EkonomickeSubjektyResponse GetEkonomickeSubjektyFromObchodniJmeno(string ico);
	AresDphResponse GetAresAndPlatceDph(string ico);
	PlatceDphResponse GetPlatceDph(string dic);
	Task<AresDphResponse> GetAresAndPlatceDphAsync(string ico, CancellationToken cancellationToken = default);
	Task<EkonomickeSubjektyResponse> GetEkonomickeSubjektyFromIcoAsync(string ico, CancellationToken cancellationToken = default);
	Task<EkonomickeSubjektyResponse> GetEkonomickeSubjektyFromObchodniJmenoAsync(string obchodniJmeno, CancellationToken cancellationToken = default);
	Task<PlatceDphResponse> GetPlatceDphAsync(string dic, CancellationToken cancellationToken = default);
}