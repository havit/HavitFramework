using System.Collections.Concurrent;

namespace Havit.Ares;

internal class AresCiselnik
{
	private readonly string _zdrojCiselniku;
	private readonly string _kodCiselniku;

	private const string Language = "cs";
	internal const string UnknownValue = "Neznámý";

	private Dictionary<string, string> _aresCiselnik;

	public AresCiselnik(string zdrojCiselniku, string kodCiselniku)
	{
		_zdrojCiselniku = zdrojCiselniku;
		_kodCiselniku = kodCiselniku;
	}

	public string GetValue(string code, string unknownValue = UnknownValue)
	{
		if (string.IsNullOrEmpty(code))
		{
			return unknownValue;
		}

		EnsureCiselnik();

		if (_aresCiselnik.TryGetValue(code, out var result))
		{
			return result;
		}

		return unknownValue;
	}

	private void EnsureCiselnik()
	{
		if (_aresCiselnik == null)
		{
			CiselnikyNazevnikSeznam ciselnikyNazevnikSeznam = ReadAresCiselnik();

			_aresCiselnik = new Dictionary<string, string>();
			if (ciselnikyNazevnikSeznam?.Ciselniky?.Count() > 0)
			{
				foreach (var item in ciselnikyNazevnikSeznam.Ciselniky.First().PolozkyCiselniku.OrderByDescending(polozkaCiselniku => polozkaCiselniku.PlatnostDo))
				{
					// V ARES číselníku se občas vyskytne 2x ta samá hodnota 
					if (!_aresCiselnik.ContainsKey(item.Kod))
					{
						_aresCiselnik.Add(item.Kod, item.Nazev.First(x => x.KodJazyka == Language).Nazev);
					}
				}
			}
		}
	}

	private CiselnikyNazevnikSeznam ReadAresCiselnik()
	{
		System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
		AresRestApi aresRestApi = new AresRestApi(httpClient);
		aresRestApi.BaseUrl = AresService.AresUrl;
		CiselnikyZakladniFiltr filter = new CiselnikyZakladniFiltr()
		{
			ZdrojCiselniku = _zdrojCiselniku,
			KodCiselniku = _kodCiselniku,
			Start = 0,
			Pocet = 10000 // Předpokládáme, že číselník nemá víc než 10000 položek (Pocet není dle API reference nullable, ale při neposlání Pocet ze swaggeru to funguje dobře.
		};
		// Asynchronní volání neřešíme.
		return aresRestApi.VyhledejCiselnik(filter);
	}
}
