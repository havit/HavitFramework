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
	#region Inicializace
	private const string urlAres = "https://ares.gov.cz/ekonomicke-subjekty-v-be/rest";
	private const string urlMfcrDph = "https://adisrws.mfcr.cz/dpr/axis2/services/rozhraniCRPDPH.rozhraniCRPDPHSOAP";
	private string DateDphTimeFormat = "yyyy-MM-dd";
	/// <summary>
	/// Max počet nalezených firem při načítání dat z ARESu (podle názvu).
	/// Implicitně 100.
	/// </summary>
	public int MaxResults { get; set; }

	public AresService()
	{
		MaxResults = 100;
	}

	public AresCiselnik CiselnikPravniForma = new AresCiselnik(urlAres, "res", "PravniForma");
	public AresCiselnik CiselnikRejstrikovySoud = new AresCiselnik(urlAres, "vr", "SoudVr");
	public AresCiselnik CiselnikFinancniUrad = new AresCiselnik(urlAres, "ufo", "FinancniUrad");

	#endregion

	#region ARES2 - Public (Synchro)

	/// <remarks>
	/// Vyhledání ekonomického subjektu ARES podle zadaného iča
	/// </remarks>
	/// <returns>EkonomickeSubjektyResponse</returns>
	public virtual EkonomickeSubjektyResponse GetEkonomickeSubjektyFromIco(string ico)
	{
		EkonomickeSubjektyResponse ekonomickeSubjektyResponse;
		EkonomickeSubjektyKomplexFiltr ekonomickeSubjektyKomplexFiltr = GetEkonomickeSubjektyFromIco_PrepareRequest(ico);
		System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
		AresRestApi aresClient = new AresRestApi(httpClient);
		aresClient.BaseUrl = urlAres;
		EkonomickeSubjektySeznam resp = aresClient.VyhledejEkonomickeSubjekty(ekonomickeSubjektyKomplexFiltr);
		ekonomickeSubjektyResponse = GetEkonomickeSubjekty_ProcessResponse(resp);
		return ekonomickeSubjektyResponse;
	}

	/// <remarks>
	/// Vyhledání ekonomického subjektu ARES podle zadaného Obchodního Jména
	/// </remarks>
	/// <returns>EkonomickeSubjektyResponse</returns>
	public virtual EkonomickeSubjektyResponse GetEkonomickeSubjektyFromObchodniJmeno(string obchodniJmeno)
	{
		EkonomickeSubjektyResponse ekonomickeSubjektyResponse;
		EkonomickeSubjektyKomplexFiltr ekonomickeSubjektyKomplexFiltr = GetEkonomickeSubjektyFromObchodniJmeno_PrepareRequest(obchodniJmeno);
		System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
		AresRestApi aresClient = new AresRestApi(httpClient);
		aresClient.BaseUrl = urlAres;
		EkonomickeSubjektySeznam resp = aresClient.VyhledejEkonomickeSubjekty(ekonomickeSubjektyKomplexFiltr);
		ekonomickeSubjektyResponse = GetEkonomickeSubjekty_ProcessResponse(resp);
		return ekonomickeSubjektyResponse;
	}

	/// <remarks>
	/// Vyhledání ekonomického subjektu ARES podle zadaného iča nebo Názvu
	/// </remarks>
	/// <returns>OK</returns>
	public virtual EkonomickeSubjektyResponse GetEkonomickeSubjektyFromIcoOrObchodniJmeno(string ico, string obchodniJmeno)
	{
		EkonomickeSubjektyResponse ekonomickeSubjektyResponse;
		EkonomickeSubjektyKomplexFiltr ekonomickeSubjektyKomplexFiltr = GetEkonomickeSubjektyFromIcoOrObchodniJmeno_PrepareRequest(ico, obchodniJmeno);
		System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
		AresRestApi aresClient = new AresRestApi(httpClient);
		aresClient.BaseUrl = urlAres;
		EkonomickeSubjektySeznam resp = aresClient.VyhledejEkonomickeSubjekty(ekonomickeSubjektyKomplexFiltr);
		ekonomickeSubjektyResponse = GetEkonomickeSubjekty_ProcessResponse(resp);
		return ekonomickeSubjektyResponse;
	}

	/// <remarks>
	/// Vrací strukturovanou odpověd Nespolehlivý plátce. Tj. Pouze z MFCR. Hledání dle DIC (vcetne CZ prefixu)
	/// </remarks>
	/// <returns>PlatceDphResponse</returns>
	public virtual PlatceDphResponse GetPlatceDph(string dic)
	{
		PlatceDphResponse response = new PlatceDphResponse();
		string xmlSOAP = GetPlatceDph_PrepareRequest(dic);
		string resultXML = PostSoapDphRequest(urlMfcrDph, xmlSOAP);
		return GetPlatceDph_ProcessResponse(resultXML);
	}

	/// <remarks>
	/// Kombinuje volání ARES a DPH. Pracuje sekvenčně. Nejprve ARES. Pokud v ARESu zjistí že existuje DIC, tak volá i PlatceDph. 
	/// </remarks>
	/// <returns>AresDphResponse</returns>
	public AresDphResponse GetAresAndPlatceDph(string ico)
	{
		AresDphResponse npResult = new AresDphResponse();
		var ekonomickeSubjekty = GetEkonomickeSubjektyFromIco(ico);
		if (ekonomickeSubjekty.EkonomickeSubjektyAres.Any())
		{
			var aktSubject = ekonomickeSubjekty.EkonomickeSubjektyAres.First();
			npResult.AresElement = aktSubject;
			if (aktSubject.AresExtension.IsPlatceDph && aktSubject.Dic != null && aktSubject.Dic.Length >= 10)
			{
				var platceDPH = GetPlatceDph(aktSubject.Dic.Right(10));
				if (platceDPH.IsNalezeno)
				{
					npResult.PlatceDphElement = platceDPH;
				}
			}
		}
		return npResult;
	}
	#endregion

	#region ARES2 - Public (Async)

	/// <summary>
	/// Vrací výsledky z ARES. Hledání dle ICO. Odpověď má pouze jeden subjekt.
	/// </summary>
	public async Task<EkonomickeSubjektyResponse> GetEkonomickeSubjektyFromIcoAsync(string ico, CancellationToken cancellationToken = default)
	{
		EkonomickeSubjektyResponse ekonomickeSubjektyResponse = new EkonomickeSubjektyResponse();
		EkonomickeSubjektyKomplexFiltr ekonomickeSubjektyKomplexFiltr = GetEkonomickeSubjektyFromIco_PrepareRequest(ico);
		System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
		AresRestApi aresClient = new AresRestApi(httpClient);
		aresClient.BaseUrl = urlAres;
		EkonomickeSubjektySeznam resp = await aresClient.VyhledejEkonomickeSubjektyAsync(ekonomickeSubjektyKomplexFiltr).ConfigureAwait(false);
		ekonomickeSubjektyResponse = GetEkonomickeSubjekty_ProcessResponse(resp);
		return ekonomickeSubjektyResponse;
	}

	/// <summary>
	/// Vrací výsledky z ARES. Hledání dle Obchodního jména. Odpověď může mít až 100 subjektů (defaultně - lze změnit via MaxResult).
	/// </summary>
	public async Task<EkonomickeSubjektyResponse> GetEkonomickeSubjektyFromObchodniJmenoAsync(string obchodniJmeno, CancellationToken cancellationToken = default)
	{
		EkonomickeSubjektyResponse ekonomickeSubjektyResponse = new EkonomickeSubjektyResponse();
		EkonomickeSubjektyKomplexFiltr ekonomickeSubjektyKomplexFiltr = GetEkonomickeSubjektyFromObchodniJmeno_PrepareRequest(obchodniJmeno);
		System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
		AresRestApi aresClient = new AresRestApi(httpClient);
		aresClient.BaseUrl = urlAres;
		EkonomickeSubjektySeznam resp = await aresClient.VyhledejEkonomickeSubjektyAsync(ekonomickeSubjektyKomplexFiltr).ConfigureAwait(false);
		ekonomickeSubjektyResponse = GetEkonomickeSubjekty_ProcessResponse(resp);
		return ekonomickeSubjektyResponse;
	}

	/// <summary>
	/// Vrací strukturovanou odpověd Nespolehlivý plátce. Tj. Pouze z MFCR. Hledání dle DIC (vcetne CZ prefixu)
	/// </summary>
	public async Task<PlatceDphResponse> GetPlatceDphAsync(string dic, CancellationToken cancellationToken = default)
	{
		PlatceDphResponse platceDphResponse = new PlatceDphResponse();
		string xmlSOAP = GetPlatceDph_PrepareRequest(dic);
		string resultXML = await PostSoapDphRequestAsync(urlMfcrDph, xmlSOAP, cancellationToken).ConfigureAwait(false);
		platceDphResponse = GetPlatceDph_ProcessResponse(resultXML);
		return platceDphResponse;
	}

	/// <summary>
	/// Vrací výsledky ze dvou volani ARES + nespolehlivyPlatce (z MFCR).  Hledání pouze dle ICO
	/// </summary>
	public async Task<AresDphResponse> GetAresAndPlatceDphAsync(string ico, CancellationToken cancellationToken = default)
	{
		AresDphResponse aresDphResponse = new AresDphResponse();
		var ekonomickeSubjekty = await GetEkonomickeSubjektyFromIcoAsync(ico, cancellationToken).ConfigureAwait(false);
		if (ekonomickeSubjekty.EkonomickeSubjektyAres.Any())
		{
			var aktSubject = ekonomickeSubjekty.EkonomickeSubjektyAres.First();
			aresDphResponse.AresElement = aktSubject;
			if (aktSubject.AresExtension.IsPlatceDph && aktSubject.Dic != null && aktSubject.Dic.Length >= 10)
			{
				aresDphResponse.PlatceDphElement = await GetPlatceDphAsync(aktSubject.Dic.Right(10), cancellationToken).ConfigureAwait(false);
			}
		}
		return aresDphResponse;
	}
	#endregion


	#region PrepareRequest/ProcessResponse - Parts 
	private EkonomickeSubjektyKomplexFiltr GetEkonomickeSubjektyFromIco_PrepareRequest(string ico)
	{
		Contract.Requires(ico != null, "Ico = Null");
		Contract.Requires(ico.Length == 8, "Ico nemá předepsanou délku 8 znaků");
		EkonomickeSubjektyKomplexFiltr ekonomickeSubjektyKomplexFiltr = new EkonomickeSubjektyKomplexFiltr
		{
			Start = 0,
			Pocet = 1,
			Ico = new List<string>() { ico }
		};
		return ekonomickeSubjektyKomplexFiltr;
	}

	private EkonomickeSubjektyKomplexFiltr GetEkonomickeSubjektyFromObchodniJmeno_PrepareRequest(string obchodniJmeno)
	{
		Contract.Requires(obchodniJmeno != null, "ObchodniJmeno = Null");
		Contract.Requires(obchodniJmeno.Length != 0, "ObchodniJmeno nesmí obsahovat prázdný řetězec");
		EkonomickeSubjektyResponse ekonomickeSubjektyResponse = new EkonomickeSubjektyResponse();
		EkonomickeSubjektyKomplexFiltr ekonomickeSubjektyKomplexFiltr = new EkonomickeSubjektyKomplexFiltr
		{
			Start = 0,
			Pocet = MaxResults,
			ObchodniJmeno = obchodniJmeno
		};
		return ekonomickeSubjektyKomplexFiltr;
	}

	private EkonomickeSubjektyKomplexFiltr GetEkonomickeSubjektyFromIcoOrObchodniJmeno_PrepareRequest(string ico, string obchodniJmeno)
	{
		if (!string.IsNullOrEmpty(ico))
		{
			return GetEkonomickeSubjektyFromIco_PrepareRequest(ico);
		}
		else if (!string.IsNullOrEmpty(obchodniJmeno))
		{
			return GetEkonomickeSubjektyFromObchodniJmeno_PrepareRequest(obchodniJmeno);
		}
		else
		{
			Contract.Requires(true, "Parametr ObchodniJmeno nebo Ico musí být zadán.");
			return null;
		}
	}

	private EkonomickeSubjektyResponse GetEkonomickeSubjekty_ProcessResponse(EkonomickeSubjektySeznam ekonomickeSubjektySeznam)
	{
		EkonomickeSubjektyResponse ekonomickeSubjektyResponse = new EkonomickeSubjektyResponse();
		ekonomickeSubjektyResponse.EkonomickeSubjektyAres = ekonomickeSubjektySeznam.EkonomickeSubjekty.ToList();
		ekonomickeSubjektyResponse.PocetCelkem = ekonomickeSubjektySeznam.PocetCelkem;
		foreach (var subjekt in ekonomickeSubjektyResponse.EkonomickeSubjektyAres)
		{
			AppendAresExtension(subjekt);
		}
		return ekonomickeSubjektyResponse;
	}

	private string GetPlatceDph_PrepareRequest(string dic, bool checkInputParamValidity = true)
	{
		if (checkInputParamValidity)
		{
			Contract.Requires(dic != null, "dic = null");
			Contract.Requires(dic.Length == 10, "dic musí mít délku 10 znaků ");
			Contract.Requires(dic.Substring(2).All(char.IsDigit), "dic musí obsahovat posledních 8 znaků pouze číslice");
		}
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

	private PlatceDphResponse GetPlatceDph_ProcessResponse(string XMLResponse)
	{
		PlatceDphResponse response = new PlatceDphResponse();
		try
		{
			XNamespace nsEnvelop = "http://schemas.xmlsoap.org/soap/envelope/";
			XNamespace nsRozhrani = "http://adis.mfcr.cz/rozhraniCRPDPH/";
			response.ResponseRaw = XMLResponse;
			XDocument doc = XDocument.Parse(XMLResponse);
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
				response.NazevFu = CiselnikFinancniUrad.GetItemValue(response.CisloFu);
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
		}
		catch (Exception Ex)
		{
			throw new AresDphException("Bad Format response XML - " + Ex.Message, PlatceDphStatusCode.XMLError);
		}
		return response;
	}

	#endregion

	#region ARES2 - Private
	private void AppendAresExtension(EkonomickySubjekt ekonomickySubjekt)
	{
		ekonomickySubjekt.AresExtension = new Ares_Extension();
		ekonomickySubjekt.AresExtension.PravniFormaText = CiselnikPravniForma.GetItemValue(ekonomickySubjekt.PravniForma);
		ekonomickySubjekt.AresExtension.FinancniUradText = CiselnikFinancniUrad.GetItemValue(ekonomickySubjekt.FinancniUrad);
		var subjectvr = ekonomickySubjekt.DalsiUdaje.FirstOrDefault(x => x.DatovyZdroj == "vr" && x.SpisovaZnacka?.Length > 5);
		if (subjectvr != null)
		{
			string[] spisovaZnackaSplit = subjectvr.SpisovaZnacka.Split('/');
			ekonomickySubjekt.AresExtension.RejstrikovySoudText = CiselnikRejstrikovySoud.GetItemValue(spisovaZnackaSplit[spisovaZnackaSplit.Count() - 1]);
			ekonomickySubjekt.AresExtension.SpisovaZnackaFull = subjectvr.SpisovaZnacka + " " + ekonomickySubjekt.AresExtension.RejstrikovySoudText;
		}
		else
		{
			ekonomickySubjekt.AresExtension.RejstrikovySoudText = "";
			ekonomickySubjekt.AresExtension.SpisovaZnackaFull = "";
		}
		ekonomickySubjekt.AresExtension.SidloPscText = ekonomickySubjekt.Sidlo?.Psc.ToString("### ##");
		ekonomickySubjekt.AresExtension.SidloAddressLine = ekonomickySubjekt.Sidlo?.TextovaAdresa?.Split(',');
		var SidloAdsArray = ekonomickySubjekt.Sidlo?.TextovaAdresa?.ToCharArray().Where(c => !Char.IsWhiteSpace(c) && c != ',').ToArray();
		string DorucAds = ((ekonomickySubjekt.AdresaDorucovaci?.RadekAdresy1 ?? "") + (ekonomickySubjekt.AdresaDorucovaci?.RadekAdresy2 ?? "") + (ekonomickySubjekt.AdresaDorucovaci?.RadekAdresy3 ?? "")).ToString();
		var DorucAdsArray = DorucAds?.ToCharArray().Where(c => !Char.IsWhiteSpace(c) && c != ',').ToArray();
		ekonomickySubjekt.AresExtension.IsDorucovaciAdresaStejna = (new string(SidloAdsArray) == new string(DorucAdsArray));
		ekonomickySubjekt.AresExtension.IsPlatceDph = ekonomickySubjekt.SeznamRegistraci.StavZdrojeDph == "AKTIVNI";

	}

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
