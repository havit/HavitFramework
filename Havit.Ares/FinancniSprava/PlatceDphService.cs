using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Havit.Ares.Ares;
using Havit.Diagnostics.Contracts;

namespace Havit.Ares.FinancniSprava;

/// <inheritdoc/>
public class PlatceDphService : IPlatceDphService
{
	private const string DateDphTimeFormat = "yyyy-MM-dd";
	private const string MfcrDphUrl = "https://adisrws.mfcr.cz/dpr/axis2/services/rozhraniCRPDPH.rozhraniCRPDPHSOAP";
	private AresCiselnik _ciselnikFinancniUrad = new AresCiselnik("ufo", "FinancniUrad");

	/// <inheritdoc/>
	public PlatceDphResult GetPlatceDph(string dic)
	{
		string xmlSOAP = GetPlatceDph_PrepareRequest(dic);
		string resultXML = PostSoapDphRequest(MfcrDphUrl, xmlSOAP);
		return GetPlatceDph_ProcessResponse(resultXML);
	}
	/// <inheritdoc/>
	public async Task<PlatceDphResult> GetPlatceDphAsync(string dic, CancellationToken cancellationToken = default)
	{
		string xmlSOAP = GetPlatceDph_PrepareRequest(dic);
		string resultXML = await PostSoapDphRequestAsync(MfcrDphUrl, xmlSOAP, cancellationToken).ConfigureAwait(false);
		return GetPlatceDph_ProcessResponse(resultXML);
	}

	private string GetPlatceDph_PrepareRequest(string dic)
	{
		// Dic musi mit delku 10-12 znaku.  
		Contract.Requires<ArgumentNullException>(dic != null);
		Contract.Requires<ArgumentException>(dic.Length >= 10 && dic.Length <= 12, "Dic musí mít délku 10-12 znaků ");
		Contract.Requires<ArgumentException>(Regex.IsMatch(dic, "^[a-zA-Z0-9_]*$"), "Dic musí obsahovat pouze AlfaNumerické znaky");
		Contract.Requires<ArgumentException>(dic.Right(8).All(char.IsDigit), "Dic musí obsahovat posledních 8 znaků pouze číslice");
		Contract.Requires<ArgumentException>(dic.Length == 10 || dic.Length > 10 && dic.Left(2).ToUpper() == "CZ", "Dic delší jak 10 znaků musí začínat CZ");
		string xmlSOAP = @"<?xml version=""1.0"" encoding=""utf-8""?>
        <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"">
            <soapenv:Body>
                <StatusNespolehlivyPlatceRequest xmlns=""http://adis.mfcr.cz/rozhraniCRPDPH/"">
                     <dic>ANYDIC</dic>
                </StatusNespolehlivyPlatceRequest>
            </soapenv:Body>
        </soapenv:Envelope>";
		return xmlSOAP.Replace("ANYDIC", dic.Right(10));
	}

	private PlatceDphResult GetPlatceDph_ProcessResponse(string response)
	{
		PlatceDphResult result = new PlatceDphResult();
		XNamespace nsEnvelop = "http://schemas.xmlsoap.org/soap/envelope/";
		XNamespace nsRozhrani = "http://adis.mfcr.cz/rozhraniCRPDPH/";

		XDocument doc;
		try
		{
			doc = XDocument.Parse(response);
		}
		catch (XmlException xmlException)
		{
			throw new PlatceDphException("Bad Format response XML.", PlatceDphStatusCode.XmlError, response, xmlException);
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
						throw new PlatceDphException("Nepovolený StatusCode " + statusCode.ToString(), PlatceDphStatusCode.BadStatusCode);
					}
				}
			}
		}
		if (!isExistsStatus)
		{
			throw new PlatceDphException("Bad Format response XML - no exists status or Bad format", PlatceDphStatusCode.XmlError, response: response);
		}
		foreach (XElement elm in childListBody?.Elements(nsRozhrani + "StatusNespolehlivyPlatceResponse")?.Elements(nsRozhrani + "statusPlatceDPH"))
		{
			string NespolehlivyPlatce = elm.Attribute("nespolehlivyPlatce")?.Value;
			if (NespolehlivyPlatce == "NENALEZEN")
			{
				return null;
			}
			else if (NespolehlivyPlatce == "NE")
			{
				result.IsNespolehlivy = false;
			}
			else
			{
				result.IsNespolehlivy = true;
			}
			result.Dic = elm.Attribute("dic")?.Value;
			result.NazevFinancnihoUradu = _ciselnikFinancniUrad.GetValue(elm.Attribute("cisloFu")?.Value);
			string dtZverejneniNespolehlivostiString = elm.Attribute("datumZverejneniNespolehlivosti")?.Value;
			if (!string.IsNullOrEmpty(dtZverejneniNespolehlivostiString))
			{
				DateTime datumZverejneniNespolehlivosti;
				DateTime.TryParseExact(dtZverejneniNespolehlivostiString, DateDphTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out datumZverejneniNespolehlivosti);
				result.NespolehlivyOd = datumZverejneniNespolehlivosti;
			}
			else
			{
				result.NespolehlivyOd = null;
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
				if (DateTime.TryParseExact(dtUkonceniString, DateDphTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out datumUkonceni))
				{
					cu.DatumUkonceni = datumUkonceni;
				}
				var standardniUcet = elmUcet.Descendants(nsRozhrani + "standardniUcet").FirstOrDefault();
				if (standardniUcet != null)
				{
					string Predcisli = standardniUcet.Attribute("predcisli")?.Value;
					cu.Predcisli = Predcisli == null ? "" : Predcisli;
					cu.CisloUctu = standardniUcet.Attribute("cislo").Value;
					cu.KodBanky = standardniUcet.Attribute("kodBanky").Value;
					result.CislaUctu.Add(cu);
				}
				var nestandardniUcet = elmUcet.Descendants(nsRozhrani + "nestandardniUcet").FirstOrDefault();
				if (nestandardniUcet != null)
				{
					cu.Predcisli = "IBAN";
					cu.CisloUctu = nestandardniUcet.Attribute("cislo").Value;
					result.CislaUctu.Add(cu);
				}
			}
		}
		return result;
	}

	private string PostSoapDphRequest(string url, string text, CancellationToken cancellationToken = default)
	{
		HttpClient httpClient = new HttpClient();
		using (HttpContent content = new StringContent(text, Encoding.UTF8, "text/xml"))
		using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url))
		{
			request.Headers.Add("getStatusNespolehlivyPlatceRequestMessage", "");
			request.Content = content;
			using (var response = Task.Run(() => httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)).GetAwaiter().GetResult())
			{
				response.EnsureSuccessStatusCode(); // throws an Exception if 404, 500, etc.
				return Task.Run(() => response.Content.ReadAsStringAsync()).GetAwaiter().GetResult();
			}
		}
	}

	private async Task<string> PostSoapDphRequestAsync(string url, string text, CancellationToken cancellationToken = default)
	{
		HttpClient httpClient = new HttpClient();
		using (HttpContent content = new StringContent(text, Encoding.UTF8, "text/xml"))
		using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url))
		{
			request.Headers.Add("getStatusNespolehlivyPlatceRequestMessage", "");
			request.Content = content;
			using (HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
			{
				response.EnsureSuccessStatusCode(); // throws an Exception if 404, 500, etc.
				return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			}
		}
	}



}
