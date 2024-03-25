using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Ares;

/// <inheritdoc/>
public class AresDphService : IAresDphService
{
	/// <inheritdoc/>
	public AresDphResponse GetAresAndPlatceDph(string ico)
	{
		AresDphResponse aresDphResponse = new AresDphResponse();
		EkonomickySubjektItem ekonomickySubjektItem = new AresService().GetEkonomickeSubjektyDleIco(ico);
		if (ekonomickySubjektItem != null)
		{
			aresDphResponse.EkonomickySubjektItem = ekonomickySubjektItem;
			if (ekonomickySubjektItem.EkonomickySubjektExtension.IsPlatceDph)
			{
				aresDphResponse.PlatceDphElement = new PlatceDphService().GetPlatceDph(ekonomickySubjektItem.EkonomickySubjektAres.Dic);
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
		EkonomickySubjektItem ekonomickySubjektItem = await new AresService().GetEkonomickeSubjektyDleIcoAsync(ico, cancellationToken).ConfigureAwait(false);
		if (ekonomickySubjektItem != null)
		{
			aresDphResponse.EkonomickySubjektItem = ekonomickySubjektItem;
			if (ekonomickySubjektItem.EkonomickySubjektExtension.IsPlatceDph)
			{
				aresDphResponse.PlatceDphElement = await new PlatceDphService().GetPlatceDphAsync(ekonomickySubjektItem.EkonomickySubjektAres.Dic);
				if (aresDphResponse.PlatceDphElement == null)       // Nekonzistentní data mezi ARES a DPH z MFCR
				{
					ekonomickySubjektItem.EkonomickySubjektExtension.IsPlatceDph = false;
				}
			}
		}
		return aresDphResponse;
	}

}
