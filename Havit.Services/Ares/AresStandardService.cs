using System.Net;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace Havit.Services.Ares;

/// <summary>
/// Třída implementující  načítání dat ze služby ARES z rozhraní Standard (http://wwwinfo.mfcr.cz/ares/ares_xml_standard.html.cz). Umí načítat vyhledávat firmy dle názvu
/// </summary>
public class AresStandardService
{
	private const string AresStandardServiceRequestByNameUrl = "http://wwwinfo.mfcr.cz/cgi-bin/ares/darv_std.cgi?obchodni_firma=";

	/// <summary>
	/// Timeout (v milisekundách) jednoho requestu při načítání dat z ARESu.
	/// Pokud není hodnota nastavena, není délka requestu omezována (resp. je použito standardní nastavení .NETu).
	/// </summary>
	public int? Timeout
	{
		get;
		set;
	}

	/// <summary>
	/// Načte data z AREsu dle názvu firmy
	/// </summary>
	public AresPrehledSubjektuResult GetData(string nazev)
	{
		string requestUrl = AresStandardServiceRequestByNameUrl + HttpUtility.UrlEncode(nazev, Encoding.GetEncoding("windows-1250"));
		XDocument aresResponseXDocument = this.GetAresResponseXDocument(requestUrl);
		XNamespace ns = XNamespace.Get("http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_answer/v_1.0.1");
		int pocetZaznamu;
		XElement responseElement = aresResponseXDocument.Element(ns + "Ares_odpovedi").Element(ns + "Odpoved");
		if (responseElement == null)
		{
			throw new AresLoadException("Neexistuje element Odpoved");
		}
		XElement errorElement = responseElement.Element(ns + "Error");
		if (errorElement != null)
		{
			throw new AresException("Chyba ARESu: " + errorElement.ToString());
		}
		if (int.TryParse(responseElement.Element(ns + "Pocet_zaznamu").Value, out pocetZaznamu))
		{
			if (pocetZaznamu == -1)
			{
				return new AresPrehledSubjektuResult
				{
					PrilisMnohoVysledku = true
				};
			}
			AresPrehledSubjektuResult result = new AresPrehledSubjektuResult();
			foreach (XElement record in responseElement.Elements(ns + "Zaznam"))
			{
				result.Data.Add(new AresPrehledSubjektuItem
				{
					Ico = GetElementValue(record, ns + "ICO"),
					Nazev = GetElementValue(record, ns + "Obchodni_firma")
				});
			}
			return result;
		}
		else
		{
			throw new AresLoadException("Pocet_zaznamu není platné číslo");
		}
	}

	private string GetElementValue(XElement element, XName name)
	{
		XElement value = element.Element(name);
		if (value != null)
		{
			return value.Value;
		}
		else
		{
			return null;
		}
	}

	/// <summary>
	/// Odešle dotaz do obchodního rejstříku a vrátí odpověd jako XDocument objekt.
	/// </summary>
	private XDocument GetAresResponseXDocument(string requestUrl)
	{
		XDocument aresResponseXDocument = null;

		try
		{
			WebRequest aresRequest = HttpWebRequest.Create(requestUrl);
			if (this.Timeout != null)
			{
				aresRequest.Timeout = this.Timeout.Value;
			}

			using (HttpWebResponse aresResponse = (HttpWebResponse)aresRequest.GetResponse())
			{
				aresResponseXDocument = XDocument.Load(new StreamReader(aresResponse.GetResponseStream()));
			}
		}
		catch (WebException e)
		{
			throw new AresLoadException(String.Format("Chyba \"{0}\" při pokusu o získání dat ze služby ARES ({1}).", e.Message, requestUrl), e);
		}

		return aresResponseXDocument;
	}
}