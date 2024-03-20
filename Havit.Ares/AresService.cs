using System.Globalization;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using System.Net.Http;
using Havit.Diagnostics.Contracts;
using System.Diagnostics;

namespace Havit.Ares;

/// <summary>
/// Třída implementující načítání dat z obchodního rejstříku (ARES) a Kontroly pro DPH z MFCR .
/// </summary>
public class AresService : IAresService
{
	internal const string AresUrl = "https://ares.gov.cz/ekonomicke-subjekty-v-be/rest";
	private const string MfcrDphUrl = "https://adisrws.mfcr.cz/dpr/axis2/services/rozhraniCRPDPH.rozhraniCRPDPHSOAP";
	public const int DefaultMaxResults = 100;

	private string DateDphTimeFormat = "yyyy-MM-dd";

	private AresCiselnik _ciselnikPravniForma = new AresCiselnik("res", "PravniForma");
	private AresCiselnik _ciselnikRejstrikovySoud = new AresCiselnik("vr", "SoudVr");
	private AresCiselnik _ciselnikFinancniUrad = new AresCiselnik("ufo", "FinancniUrad");

	/// <remarks>
	/// Vyhledání ekonomického subjektu ARES podle zadaného iča
	/// </remarks>
	/// <returns>EkonomickeSubjektyResponse</returns>
	public EkonomickySubjektItem GetEkonomickeSubjektyDleIco(string ico)
	{
		EkonomickeSubjektyKomplexFiltr ekonomickeSubjektyKomplexFiltr = GetEkonomickeSubjektyDleIco_PrepareRequest(ico);
		using System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
		AresRestApi aresClient = new AresRestApi(httpClient);
		aresClient.BaseUrl = AresUrl;
		EkonomickeSubjektySeznam resp = aresClient.VyhledejEkonomickeSubjekty(ekonomickeSubjektyKomplexFiltr);
		EkonomickeSubjektyResult ekonomickeSubjektyResponse = GetEkonomickeSubjekty_ProcessResponse(resp);

		return ekonomickeSubjektyResponse.Items.SingleOrDefault();
	}

	/// <summary>
	/// Vrací výsledky z ARES. Hledání dle ICO. Odpověď má pouze jeden subjekt.
	/// </summary>
	public async Task<EkonomickySubjektItem> GetEkonomickeSubjektyDleIcoAsync(string ico, CancellationToken cancellationToken = default)
	{
		EkonomickeSubjektyKomplexFiltr ekonomickeSubjektyKomplexFiltr = GetEkonomickeSubjektyDleIco_PrepareRequest(ico);
		using System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
		AresRestApi aresClient = new AresRestApi(httpClient);
		aresClient.BaseUrl = AresUrl;
		EkonomickeSubjektySeznam resp = await aresClient.VyhledejEkonomickeSubjektyAsync(ekonomickeSubjektyKomplexFiltr).ConfigureAwait(false);
		EkonomickeSubjektyResult ekonomickeSubjektyResponse = GetEkonomickeSubjekty_ProcessResponse(resp);

		return ekonomickeSubjektyResponse.Items.SingleOrDefault();
	}

	/// <remarks>
	/// Vyhledání ekonomického subjektu ARES podle zadaného Obchodního Jména
	/// </remarks>
	/// <returns>EkonomickeSubjektyResponse</returns>
	public EkonomickeSubjektyResult GetEkonomickeSubjektyDleObchodnihoJmena(string obchodniJmeno, int maxResult = DefaultMaxResults)
	{
		EkonomickeSubjektyKomplexFiltr ekonomickeSubjektyKomplexFiltr = GetEkonomickeSubjektyDleObchodnihoJmena_PrepareRequest(obchodniJmeno, maxResult);
		using System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
		AresRestApi aresClient = new AresRestApi(httpClient);
		aresClient.BaseUrl = AresUrl;
		EkonomickeSubjektySeznam resp = aresClient.VyhledejEkonomickeSubjekty(ekonomickeSubjektyKomplexFiltr);
		EkonomickeSubjektyResult ekonomickeSubjektyResponse = GetEkonomickeSubjekty_ProcessResponse(resp);
		return ekonomickeSubjektyResponse;
	}

	/// <summary>
	/// Vrací výsledky z ARES. Hledání dle Obchodního jména. Odpověď může mít až 100 subjektů (defaultně - lze změnit via MaxResult).
	/// </summary>
	public async Task<EkonomickeSubjektyResult> GetEkonomickeSubjektyDleObchodnihoJmenaAsync(string obchodniJmeno, int maxResults = DefaultMaxResults, CancellationToken cancellationToken = default)
	{
		EkonomickeSubjektyKomplexFiltr ekonomickeSubjektyKomplexFiltr = GetEkonomickeSubjektyDleObchodnihoJmena_PrepareRequest(obchodniJmeno, maxResults);
		using System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
		AresRestApi aresClient = new AresRestApi(httpClient);
		aresClient.BaseUrl = AresUrl;
		EkonomickeSubjektySeznam resp = await aresClient.VyhledejEkonomickeSubjektyAsync(ekonomickeSubjektyKomplexFiltr).ConfigureAwait(false);
		EkonomickeSubjektyResult ekonomickeSubjektyResponse = GetEkonomickeSubjekty_ProcessResponse(resp);
		return ekonomickeSubjektyResponse;
	}

	private EkonomickeSubjektyKomplexFiltr GetEkonomickeSubjektyDleIco_PrepareRequest(string ico)
	{
		Contract.Requires<ArgumentNullException>(ico != null);
		Contract.Requires<ArgumentException>(ico.Length == 8, $"Argument '{nameof(ico)}' musí mít předepsanou délku 8 znaků.");
		EkonomickeSubjektyKomplexFiltr ekonomickeSubjektyKomplexFiltr = new EkonomickeSubjektyKomplexFiltr
		{
			Start = 0,
			Pocet = 1,
			Ico = new List<string>() { ico }
		};
		return ekonomickeSubjektyKomplexFiltr;
	}

	private EkonomickeSubjektyKomplexFiltr GetEkonomickeSubjektyDleObchodnihoJmena_PrepareRequest(string obchodniJmeno, int maxResults)
	{
		Contract.Requires<ArgumentNullException>(obchodniJmeno != null);
		Contract.Requires<ArgumentException>(obchodniJmeno.Length != 0, $"Argument '{nameof(obchodniJmeno)}' nesmí obsahovat prázdný řetězec.");
		EkonomickeSubjektyKomplexFiltr ekonomickeSubjektyKomplexFiltr = new EkonomickeSubjektyKomplexFiltr
		{
			Start = 0,
			Pocet = maxResults,
			ObchodniJmeno = obchodniJmeno
		};
		return ekonomickeSubjektyKomplexFiltr;
	}

	private EkonomickeSubjektyResult GetEkonomickeSubjekty_ProcessResponse(EkonomickeSubjektySeznam ekonomickeSubjektySeznam)
	{
		return new EkonomickeSubjektyResult
		{
			PocetCelkem = ekonomickeSubjektySeznam.PocetCelkem,
			Items = ekonomickeSubjektySeznam.EkonomickeSubjekty
				.Select(ekonomickySubjekt => new EkonomickySubjektItem
				{
					EkonomickySubjektAres = ekonomickySubjekt,
					EkonomickySubjektExtension = GetEkonomickeSubjekty_GetExtension(ekonomickySubjekt)
				})
				.ToList()
		};
	}

	private EkonomickySubjektExtension GetEkonomickeSubjekty_GetExtension(EkonomickySubjekt ekonomickySubjekt)
	{
		EkonomickySubjektExtension aresExtension = new EkonomickySubjektExtension();
		aresExtension.PravniFormaText = _ciselnikPravniForma.GetValue(ekonomickySubjekt.PravniForma);
		aresExtension.FinancniUradText = _ciselnikFinancniUrad.GetValue(ekonomickySubjekt.FinancniUrad);
		var subjectvr = ekonomickySubjekt.DalsiUdaje.FirstOrDefault(x => x.DatovyZdroj == "vr" && x.SpisovaZnacka?.Length > 5);
		if (subjectvr != null)
		{
			string[] spisovaZnackaSplit = subjectvr.SpisovaZnacka.Split('/');
			aresExtension.RejstrikovySoudText = _ciselnikRejstrikovySoud.GetValue(spisovaZnackaSplit[spisovaZnackaSplit.Count() - 1]);
			aresExtension.SpisovaZnackaFull = subjectvr.SpisovaZnacka + " " + aresExtension.RejstrikovySoudText;
		}
		else
		{
			aresExtension.RejstrikovySoudText = "";
			aresExtension.SpisovaZnackaFull = "";
		}
		aresExtension.SidloPscText = ekonomickySubjekt.Sidlo?.Psc.ToString("### ##");
		aresExtension.SidloAddressLines = ekonomickySubjekt.Sidlo?.TextovaAdresa?.Split(',');
		// TODO: Unit test
		var SidloAdsArray = ekonomickySubjekt.Sidlo?.TextovaAdresa?.ToCharArray().Where(c => !Char.IsWhiteSpace(c) && c != ',').ToArray();
		string DorucAds = ekonomickySubjekt.AdresaDorucovaci?.RadekAdresy1 + ekonomickySubjekt.AdresaDorucovaci?.RadekAdresy2 + ekonomickySubjekt.AdresaDorucovaci?.RadekAdresy3;
		var DorucAdsArray = DorucAds?.ToCharArray().Where(c => !Char.IsWhiteSpace(c) && c != ',').ToArray();
		aresExtension.IsDorucovaciAdresaStejna = (new string(SidloAdsArray) == new string(DorucAdsArray));
		aresExtension.IsPlatceDph = ekonomickySubjekt.SeznamRegistraci.StavZdrojeDph == "AKTIVNI";
		return aresExtension;
	}

	/// <remarks>
	/// Vrací strukturovanou odpověd Nespolehlivý plátce. Tj. Pouze z MFCR. Hledání dle DIC (vcetne CZ prefixu)
	/// </remarks>
	/// <returns>PlatceDphResponse</returns>
	public PlatceDphResponse GetPlatceDph(string dic)
	{
		string xmlSOAP = GetPlatceDph_PrepareRequest(dic);
		string resultXML = PostSoapDphRequest(MfcrDphUrl, xmlSOAP);
		return GetPlatceDph_ProcessResponse(resultXML);
	}

	/// <summary>
	/// Vrací strukturovanou odpověd Nespolehlivý plátce. Tj. Pouze z MFCR. Hledání dle DIC (vcetne CZ prefixu)
	/// </summary>
	public async Task<PlatceDphResponse> GetPlatceDphAsync(string dic, CancellationToken cancellationToken = default)
	{
		string xmlSOAP = GetPlatceDph_PrepareRequest(dic);
		string resultXML = await PostSoapDphRequestAsync(MfcrDphUrl, xmlSOAP, cancellationToken).ConfigureAwait(false);
		return GetPlatceDph_ProcessResponse(resultXML);
	}

	/// <remarks>
	/// Kombinuje volání ARES a DPH. Pracuje sekvenčně. Nejprve ARES. Pokud v ARESu zjistí že existuje DIC, tak volá i PlatceDph. 
	/// </remarks>
	/// <returns>AresDphResponse</returns>
	public AresDphResponse GetAresAndPlatceDph(string ico)
	{
		throw new NotImplementedException();
		//AresDphResponse npResult = new AresDphResponse();
		//var ekonomickeSubjekty = GetEkonomickeSubjektyByIco(ico);
		//if (ekonomickeSubjekty.EkonomickeSubjektyAres.Any())
		//{
		//	var aktSubject = ekonomickeSubjekty.EkonomickeSubjektyAres.First();
		//	npResult.AresElement = aktSubject;
		//	if (aktSubject.AresExtension.IsPlatceDph && aktSubject.Dic != null && aktSubject.Dic.Length >= 10)
		//	{
		//		var platceDPH = GetPlatceDph(aktSubject.Dic.Right(10));
		//		if (platceDPH.IsNalezeno)
		//		{
		//			npResult.PlatceDphElement = platceDPH;
		//		}
		//	}
		//}
		//return npResult;
	}

	/// <summary>
	/// Vrací výsledky ze dvou volani ARES + nespolehlivyPlatce (z MFCR).  Hledání pouze dle ICO
	/// </summary>
	public async Task<AresDphResponse> GetAresAndPlatceDphAsync(string ico, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
		//AresDphResponse aresDphResponse = new AresDphResponse();
		//var ekonomickeSubjekty = await GetEkonomickeSubjektyByIcoAsync(ico, cancellationToken).ConfigureAwait(false);
		//if (ekonomickeSubjekty.EkonomickeSubjektyAres.Any())
		//{
		//	var aktSubject = ekonomickeSubjekty.EkonomickeSubjektyAres.First();
		//	aresDphResponse.AresElement = aktSubject;
		//	if (aktSubject.AresExtension.IsPlatceDph && aktSubject.Dic != null && aktSubject.Dic.Length >= 10)
		//	{
		//		aresDphResponse.PlatceDphElement = await GetPlatceDphAsync(aktSubject.Dic.Right(10), cancellationToken).ConfigureAwait(false);
		//	}
		//}
		//return aresDphResponse;
	}


	#region PrepareRequest/ProcessResponse - Parts 

	private string GetPlatceDph_PrepareRequest(string dic)
	{
		Contract.Requires<ArgumentNullException>(dic != null);
		Contract.Requires<ArgumentException>(dic.Length == 10, "Dic musí mít délku 10 znaků ");
		Contract.Requires<ArgumentException>(dic.Substring(2).All(char.IsDigit), "Dic musí obsahovat posledních 8 znaků pouze číslice");
		string xmlSOAP = @"<?xml version=""1.0"" encoding=""utf-8""?>
        <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"">
            <soapenv:Body>
                <StatusNespolehlivyPlatceRequest xmlns=""http://adis.mfcr.cz/rozhraniCRPDPH/"">
                     <dic>ANYDIC</dic>
                </StatusNespolehlivyPlatceRequest>
            </soapenv:Body>
        </soapenv:Envelope>";
		return xmlSOAP.Replace("ANYDIC", dic);
	}

	private PlatceDphResponse GetPlatceDph_ProcessResponse(string xmlResponse)
	{
		PlatceDphResponse response = new PlatceDphResponse();
		XNamespace nsEnvelop = "http://schemas.xmlsoap.org/soap/envelope/";
		XNamespace nsRozhrani = "http://adis.mfcr.cz/rozhraniCRPDPH/";
		response.ResponseRaw = xmlResponse; // TODO: Bude si někdo parsovat nebo dáme XDocument? Nebo máme schéma??

		XDocument doc;
		try
		{
			doc = XDocument.Parse(xmlResponse);
		}
		catch (Exception e)
		{
			throw new AresDphException("Bad Format response XML.", PlatceDphStatusCode.XMLError, e);
		}

		IEnumerable<XElement> childListBody = from el in doc.Root?.Descendants(nsEnvelop + "Body") select el;
		bool isExistsStatus = false;
		foreach (XElement elm in childListBody?.Elements(nsRozhrani + "StatusNespolehlivyPlatceResponse")?.Elements(nsRozhrani + "status"))
		{
			string statusCodeText = elm.Attribute("statusCode")?.Value;
			if (statusCodeText != null)
			{
				int statusCode = -1;
				if (int.TryParse(elm.Attribute("statusCode")?.Value.ToString(), out statusCode))
				{
					if (statusCode >= 0 && statusCode <= 3)
					{
						isExistsStatus = true;
					}
					else
					{
						throw new AresDphException("Nepovolený StatusCode " + statusCode.ToString(), PlatceDphStatusCode.BadStatusCode);
					}
				}
			}
		}
		if (!isExistsStatus)
		{
			throw new AresDphException("Bad Format response XML - no exists status or Bad format", PlatceDphStatusCode.XMLError);
		}
		foreach (XElement elm in childListBody?.Elements(nsRozhrani + "StatusNespolehlivyPlatceResponse")?.Elements(nsRozhrani + "statusPlatceDPH"))
		{
			string NespolehlivyPlatce = elm.Attribute("nespolehlivyPlatce")?.Value;
			if (NespolehlivyPlatce == "NENALEZEN")
			{
				response.IsNespolehlivy = true;
				response.IsNalezeno = false;
				return response;
			}
			else if (NespolehlivyPlatce == "NE")
			{
				response.IsNespolehlivy = false;
				response.IsNalezeno = true;
			}
			else
			{
				response.IsNespolehlivy = true;
				response.IsNalezeno = true;
			}
			response.Dic = elm.Attribute("dic")?.Value;
			response.CisloFu = elm.Attribute("cisloFu")?.Value;
			response.NazevFu = _ciselnikFinancniUrad.GetValue(response.CisloFu);
			string dtZverejneniNespolehlivostiString = elm.Attribute("datumZverejneniNespolehlivosti")?.Value;
			if (!string.IsNullOrEmpty(dtZverejneniNespolehlivostiString))
			{
				DateTime datumZverejneniNespolehlivosti;
				DateTime.TryParseExact(dtZverejneniNespolehlivostiString, DateDphTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out datumZverejneniNespolehlivosti);
				response.NespolehlivyOd = datumZverejneniNespolehlivosti;
			}
			else
			{
				response.NespolehlivyOd = DateTime.MinValue;
			}

			foreach (var elmUcet in elm.Descendants(nsRozhrani + "ucet"))
			{
				PlatceDphCisloUctu cu = new PlatceDphCisloUctu();
				DateTime datumZverejneni;
				DateTime datumUkonceni;
				string dtZverejneniString = elmUcet.Attribute("datumZverejneni")?.Value;
				DateTime.TryParseExact(dtZverejneniString, DateDphTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out datumZverejneni);
				cu.DatumZverejneni = datumZverejneni;
				string dtUkonceniString = elmUcet.Attribute("datumUkonceni")?.Value;
				DateTime.TryParseExact(dtUkonceniString, DateDphTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out datumUkonceni);
				cu.DatumUkonceni = datumUkonceni;
				var standardniUcet = elmUcet.Descendants(nsRozhrani + "standardniUcet").FirstOrDefault();
				if (standardniUcet != null)
				{
					string Predcisli = standardniUcet.Attribute("predcisli")?.Value;
					cu.Predcisli = Predcisli == null ? "" : Predcisli;
					cu.Cislo = standardniUcet.Attribute("cislo").Value;
					cu.KodBanky = standardniUcet.Attribute("kodBanky").Value;
					response.CislaUctu.Add(cu);
				}
				var nestandardniUcet = elmUcet.Descendants(nsRozhrani + "nestandardniUcet").FirstOrDefault();
				if (nestandardniUcet != null)
				{
					cu.Predcisli = "IBAN";
					cu.Cislo = nestandardniUcet.Attribute("cislo").Value;
					response.CislaUctu.Add(cu);
				}
			}
		}

		return response;
	}

	#endregion

	#region ARES2 - Private

	private async Task<string> PostSoapDphRequestAsync(string url, string text, CancellationToken cancellationToken = default)
	{
		System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
		using (System.Net.Http.HttpContent content = new System.Net.Http.StringContent(text, Encoding.UTF8, "text/xml"))
		using (System.Net.Http.HttpRequestMessage request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, url))
		{
			request.Headers.Add("getStatusNespolehlivyPlatceRequestMessage", "");
			request.Content = content;
			using (System.Net.Http.HttpResponseMessage response = await httpClient.SendAsync(request, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
			{
				response.EnsureSuccessStatusCode(); // throws an Exception if 404, 500, etc.
				return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			}
		}
	}

	private string PostSoapDphRequest(string url, string text, CancellationToken cancellationToken = default)
	{
		System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
		using (System.Net.Http.HttpContent content = new System.Net.Http.StringContent(text, Encoding.UTF8, "text/xml"))
		using (System.Net.Http.HttpRequestMessage request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, url))
		{
			request.Headers.Add("getStatusNespolehlivyPlatceRequestMessage", "");
			request.Content = content;
			using (var response = Task.Run(() => httpClient.SendAsync(request, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken)).GetAwaiter().GetResult())
			{
				response.EnsureSuccessStatusCode(); // throws an Exception if 404, 500, etc.
				return Task.Run(() => response.Content.ReadAsStringAsync()).GetAwaiter().GetResult();
			}
		}
	}


	#endregion


}
