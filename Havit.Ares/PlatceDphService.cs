using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using Havit.Diagnostics.Contracts;

namespace Havit.Ares;
/// <inheritdoc/>
public class PlatceDphService : IPlatceDphService
{
	private const string DateDphTimeFormat = "yyyy-MM-dd";
	private const string MfcrDphUrl = "https://adisrws.mfcr.cz/dpr/axis2/services/rozhraniCRPDPH.rozhraniCRPDPHSOAP";
	private AresCiselnik _ciselnikFinancniUrad = new AresCiselnik("ufo", "FinancniUrad");

	#region Public methods

	/// <inheritdoc/>
	public PlatceDphResponse GetPlatceDph(string dic)
	{
		string xmlSOAP = GetPlatceDph_PrepareRequest(dic);
		string resultXML = PostSoapDphRequest(MfcrDphUrl, xmlSOAP);
		return GetPlatceDph_ProcessResponse(resultXML);
	}
	/// <inheritdoc/>
	public async Task<PlatceDphResponse> GetPlatceDphAsync(string dic, CancellationToken cancellationToken = default)
	{
		string xmlSOAP = GetPlatceDph_PrepareRequest(dic);
		string resultXML = await PostSoapDphRequestAsync(MfcrDphUrl, xmlSOAP, cancellationToken).ConfigureAwait(false);
		return GetPlatceDph_ProcessResponse(resultXML);
	}

	#endregion

	

	#region Private - PreparaRequest, ProcessResponse
	private string GetPlatceDph_PrepareRequest(string dic)
	{
		// Dic musi mit delku 10-12 znaku.  
		Contract.Requires<ArgumentNullException>(dic != null);
		Contract.Requires<ArgumentException>(dic.Length >= 10 && dic.Length <= 12, "Dic musí mít délku 10-12 znaků ");
		Contract.Requires<ArgumentException>(Regex.IsMatch(dic, "^[a-zA-Z0-9_]*$"), "Dic musí obsahovat pouze AlfaNumerické znaky");
		Contract.Requires<ArgumentException>(dic.Right(8).All(char.IsDigit), "Dic musí obsahovat posledních 8 znaků pouze číslice");
		Contract.Requires<ArgumentException>(dic.Length == 10 || (dic.Length > 10 && dic.Left(2).ToUpper() == "CZ"), "Dic delší jak 10 znaků musí začínat CZ");
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

	private PlatceDphResponse GetPlatceDph_ProcessResponse(string xmlResponse)
	{
		PlatceDphResponse response = new PlatceDphResponse();
		XNamespace nsEnvelop = "http://schemas.xmlsoap.org/soap/envelope/";
		XNamespace nsRozhrani = "http://adis.mfcr.cz/rozhraniCRPDPH/";

		XDocument doc;
		try
		{
			doc = XDocument.Parse(xmlResponse);
		}
		catch (Exception e)
		{
			throw new PlatceDphException("Bad Format response XML.", PlatceDphStatusCode.XMLError, response: xmlResponse, e);
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
			throw new PlatceDphException("Bad Format response XML - no exists status or Bad format", PlatceDphStatusCode.XMLError, response: xmlResponse);
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
				response.IsNespolehlivy = false;
			}
			else
			{
				response.IsNespolehlivy = true;
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

	#region Private - Call SOAP WebService

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
