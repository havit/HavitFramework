using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Havit.Services.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace Havit.Services.Ares;

// Uchova udaje z libovolneho ciselniku ARES po dobu 10 minut
public class AresCiselnik
{
	private string _zdrojCiselniku { get; set; }
	private string _kodCiselniku { get; set; }
	private string _aresUrl { get; set; }
	public int pocetRows { get; set; }
	public string language { get; set; }
	private ConcurrentDictionary<string, string> anyCiselnik;

	public AresCiselnik(string aresUrl, string zdrojCiselniku, string kodCiselniku)
	{
		_zdrojCiselniku = zdrojCiselniku;
		_kodCiselniku = kodCiselniku;
		_aresUrl = aresUrl;
		//cache = new MemoryCache(zdrojCiselniku + "-" + _kodCiselniku);
		pocetRows = 10000;
		language = "cs";   // Zatim pouze cesky
		anyCiselnik = [];
	}

	public string GetItemValue(string code)
	{

		if (string.IsNullOrEmpty(code))
		{
			return "Empty Code";
		}
		if (!anyCiselnik.Any())
		{
			ReadAndPrepareCiselnik();
		}
		anyCiselnik.TryGetValue(code, out var value2);
		return value2 ?? "unknown code " + code;
	}

	private void ReadAndPrepareCiselnik()
	{
		CiselnikyNazevnikSeznam ciselnik;
		try
		{
			ciselnik = ReadAresCiselnik();
		}
		catch
		{
			throw;
		}
		if (ciselnik?.Ciselniky?.Count() > 0)
		{
			foreach (var item in ciselnik.Ciselniky.First().PolozkyCiselniku)
			{
				if (!anyCiselnik.ContainsKey(item.Kod))                         // V ARES číselníku se občas vyskytne 2x ta samá hodnota 
				{
					anyCiselnik.TryAdd(item.Kod, item.Nazev.First(x => x.KodJazyka == language).Nazev);
				}
			}
		}
	}

	private CiselnikyNazevnikSeznam ReadAresCiselnik()
	{
		System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
		AresRestApi aresRestApi = new AresRestApi(httpClient);
		aresRestApi.BaseUrl = _aresUrl;
		CiselnikyZakladniFiltr filter = new CiselnikyZakladniFiltr()
		{
			ZdrojCiselniku = _zdrojCiselniku,
			KodCiselniku = _kodCiselniku,
			Pocet = pocetRows,
			Start = 0
		};
		return aresRestApi.VyhledejCiselnik(filter);
	}
}
