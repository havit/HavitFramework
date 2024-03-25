namespace Havit.Ares.FinancniSprava;

/// <inheritdoc/>
public class AresDphService(
	IAresService _aresService,
	IPlatceDphService _platceDphService) : IAresDphService
{
	/// <inheritdoc/>
	public AresDphResponse GetAresAndPlatceDph(string ico)
	{
		AresDphResponse aresDphResponse = new AresDphResponse();
		EkonomickySubjektItem ekonomickySubjektItem = _aresService.GetEkonomickeSubjektyDleIco(ico);
		if (ekonomickySubjektItem != null)
		{
			aresDphResponse.EkonomickySubjektItem = ekonomickySubjektItem;
			if (ekonomickySubjektItem.EkonomickySubjektExtension.IsPlatceDph)
			{
				aresDphResponse.PlatceDphElement = _platceDphService.GetPlatceDph(ekonomickySubjektItem.EkonomickySubjektAres.Dic);
				if (aresDphResponse.PlatceDphElement == null)              // Nekonzistentní data mezi ARES a DPH z MFCR
				{
					ekonomickySubjektItem.EkonomickySubjektExtension.IsPlatceDph = false;
				}
			}
		}
		return aresDphResponse;
	}
	/// <inheritdoc/>
	public async Task<AresDphResponse> GetAresAndPlatceDphAsync(string ico, CancellationToken cancellationToken = default)
	{
		AresDphResponse aresDphResponse = new AresDphResponse();
		EkonomickySubjektItem ekonomickySubjektItem = await _aresService.GetEkonomickeSubjektyDleIcoAsync(ico, cancellationToken).ConfigureAwait(false);
		if (ekonomickySubjektItem != null)
		{
			aresDphResponse.EkonomickySubjektItem = ekonomickySubjektItem;
			if (ekonomickySubjektItem.EkonomickySubjektExtension.IsPlatceDph)
			{
				aresDphResponse.PlatceDphElement = await _platceDphService.GetPlatceDphAsync(ekonomickySubjektItem.EkonomickySubjektAres.Dic);
				if (aresDphResponse.PlatceDphElement == null)       // Nekonzistentní data mezi ARES a DPH z MFCR
				{
					ekonomickySubjektItem.EkonomickySubjektExtension.IsPlatceDph = false;
				}
			}
		}
		return aresDphResponse;
	}

}
