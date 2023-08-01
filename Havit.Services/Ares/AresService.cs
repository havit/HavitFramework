using System.Net;
using System.Xml.Linq;

namespace Havit.Services.Ares;

/// <summary>
/// Třída implementující načítání dat z obchodního rejstříku (ARES).
/// </summary>
public class AresService
{
	private const string AresBasicDataRequestUrl = "http://wwwinfo.mfcr.cz/cgi-bin/ares/darv_bas.cgi?ico=";
	private const string AresObchodniRejstrikDataRequestUrl = "http://wwwinfo.mfcr.cz/cgi-bin/ares/darv_or.cgi?ico=";

	/// <summary>
	/// IČ subjektu, kterého údaje chceme získat.
	/// </summary>
	private string Ico { get; set; }

	/// <summary>
	/// Timeout (v milisekundách) jednoho requestu při načítání dat z ARESu.
	/// Pokud není hodnota nastavena, není délka requestu omezována (resp. je použito standardní nastavení .NETu).
	/// </summary>
	public int? Timeout
	{
		get; set;
	}

	/// <summary>
	/// Konstruktor.
	/// </summary>
	/// <param name="ico">IČO společnosti.</param>
	public AresService(string ico)
	{
		Ico = ico;
	}

	/// <summary>
	/// Vrací strukturovanou odpověd z obchodního rejstříku.
	/// </summary>
	public AresData GetData(AresRegistr rejstriky = AresRegistr.Basic | AresRegistr.ObchodniRejstrik)
	{
		AresData result = new AresData();
		List<Task> tasks = new List<Task>();

		if (rejstriky.HasFlag(AresRegistr.Basic))
		{
			tasks.Add(Task.Factory.StartNew(LoadBasicData, result));
		}

		if (rejstriky.HasFlag(AresRegistr.ObchodniRejstrik))
		{
			tasks.Add(Task.Factory.StartNew(LoadObchodniRejstrikData, result));
		}

		try
		{
			Task.WaitAll(tasks.ToArray());
		}
		catch (AggregateException exception)
		{
			// pokus o vybalení výjimky (chceme řešit jen jedinou)
			if ((exception.InnerExceptions.Count > 0) && (exception.InnerExceptions[0] is AresBaseException))
			{
				throw exception.InnerExceptions[0];
			}
			else
			{
				throw;
			}
		}

		return result;
	}

	private void LoadBasicData(object state)
	{
		AresData result = (AresData)state;

		string requestUrl = String.Format("{0}{1}", AresBasicDataRequestUrl, Ico);
		XDocument aresResponseXDocument = this.GetAresResponseXDocument(requestUrl);

		XNamespace aresDT = XNamespace.Get("http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3");

		// Errors
		IEnumerable<XElement> eElements = aresResponseXDocument.Root.Elements().Elements(aresDT + "E");
		if ((eElements != null) && (eElements.Count() > 0))
		{
			bool hasError = false;
			System.Text.StringBuilder errorMessages = new System.Text.StringBuilder();
			foreach (XElement item in eElements)
			{
				bool errorEK = ((int)item.Elements(aresDT + "EK").SingleOrDefault() == 1 /* Nenalezen */);
				if (errorEK && ((string)item.Elements(aresDT + "ET").SingleOrDefault()).Contains("Chyba 71 - nenalezeno"))
				{
					// nehlásíme chybu
					continue;
				}
				if (errorEK && ((string)item.Elements(aresDT + "ET").SingleOrDefault()).Contains("Chyba 61 - subjekt zanikl"))
				{
					result.SubjektZanikl = true;
					// nehlásíme chybu
					continue;
				}
				else
				{
					hasError = true;
					errorMessages.Append(String.Format("{0}; ", (string)item.Elements(aresDT + "ET").SingleOrDefault()));
				}
			}

			if (!hasError)
			{
				// nehlásíme chybu ani neparsujeme data - jde o prosté nenalezení záznamu
				return;
			}
			else
			{
				throw new AresException(errorMessages.ToString());
			}
		}

		lock (result)
		{
			this.ParseBasicData(aresResponseXDocument, aresDT, result);
		}
	}

	private void ParseBasicData(XDocument aresResponse, XNamespace aresDT, AresData result)
	{
		// Výpis BASIC (element).
		XElement vypisOrElement = aresResponse.Descendants(aresDT + "VBAS").FirstOrDefault();

		if (vypisOrElement != null)
		{
			result.SubjektZanikl = false;
			result.Ico = (string)vypisOrElement.Elements(aresDT + "ICO").FirstOrDefault();
			result.Dic = (string)vypisOrElement.Elements(aresDT + "DIC").FirstOrDefault();
			result.NazevObchodniFirmy = (string)vypisOrElement.Elements(aresDT + "OF").FirstOrDefault(); // obchodní firma

			XElement npfElement = vypisOrElement.Elements(aresDT + "PF").Elements(aresDT + "NPF").FirstOrDefault();
			if (npfElement != null)
			{
				result.PravniForma = new AresData.Classes.PravniForma()
				{
					Nazev = (string)npfElement
				};
			}

			XElement adresaElement = vypisOrElement.Elements(aresDT + "AA").FirstOrDefault();
			if (adresaElement != null)
			{
				result.Sidlo = new AresData.Classes.Sidlo()
				{
					Ulice = (string)adresaElement.Elements(aresDT + "NU").FirstOrDefault(),

					CisloDoAdresy = (string)adresaElement.Elements(aresDT + "CA").FirstOrDefault(),
					CisloPopisne = (string)adresaElement.Elements(aresDT + "CD").FirstOrDefault(),
					CisloOrientacni = (string)adresaElement.Elements(aresDT + "CO").FirstOrDefault(),

					Mesto = (string)adresaElement.Elements(aresDT + "N").FirstOrDefault(),
					MestskaCast = (string)adresaElement.Elements(aresDT + "NCO").FirstOrDefault(),
					Psc = (string)adresaElement.Elements(aresDT + "PSC").FirstOrDefault(),
					Stat = (string)adresaElement.Elements(aresDT + "NS").FirstOrDefault(),
					AdresaTextem = (string)adresaElement.Elements(aresDT + "AT").FirstOrDefault()
				};
			}
		}
	}

	private void LoadObchodniRejstrikData(object state)
	{
		AresData result = (AresData)state;

		string requestUrl = String.Format("{0}{1}", AresObchodniRejstrikDataRequestUrl, Ico);
		XDocument aresResponseXDocument = this.GetAresResponseXDocument(requestUrl);

		XNamespace aresDT = XNamespace.Get("http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.3");

		// Errors
		IEnumerable<XElement> eElements = aresResponseXDocument.Root.Elements().Elements(aresDT + "E");
		if ((eElements != null) && (eElements.Count() > 0))
		{
			bool hasError = false;
			System.Text.StringBuilder errorMessages = new System.Text.StringBuilder();
			foreach (XElement item in eElements)
			{
				bool errorEK = ((int)item.Elements(aresDT + "EK").SingleOrDefault() == 1 /* Nenalezen */);
				if (errorEK && ((string)item.Elements(aresDT + "ET").SingleOrDefault()).Contains("Chyba 71 - nenalezeno"))
				{
					// nehlásíme chybu
					continue;
				}
				else
				{
					hasError = true;
					errorMessages.Append(String.Format("{0}; ", (string)item.Elements(aresDT + "ET").SingleOrDefault()));
				}
			}

			if (!hasError)
			{
				// nehlásíme chybu ani neparsujeme data - jde o prosté nenalezení záznamu
				return;
			}
			else
			{
				throw new AresException(errorMessages.ToString());
			}
		}

		lock (result)
		{
			this.ParseObchodniRejstrikData(aresResponseXDocument, aresDT, result);
		}
	}

	private void ParseObchodniRejstrikData(XDocument aresResponse, XNamespace aresDT, AresData result)
	{
		// Výpis OR (element).
		XElement vypisOrElement = aresResponse.Descendants(aresDT + "Vypis_OR").FirstOrDefault();

		if (vypisOrElement != null)
		{

			XElement stavElement = vypisOrElement.Elements(aresDT + "ZAU").Elements(aresDT + "S").Elements(aresDT + "SSU").FirstOrDefault();
			if (stavElement != null)
			{
				string stavResult = (string)stavElement;
				if (stavResult.Equals("Aktivní"))
				{
					result.SubjektZanikl = false;
				}
				else if (stavResult.Equals("Zaniklý"))
				{
					result.SubjektZanikl = true;
				}
			}

			result.Ico = (string)vypisOrElement.Elements(aresDT + "ZAU").Elements(aresDT + "ICO").FirstOrDefault();
			result.NazevObchodniFirmy = (string)vypisOrElement.Elements(aresDT + "ZAU").Elements(aresDT + "OF").FirstOrDefault(); // obchodní firma

			// Registrace OR
			XElement registraceElement = vypisOrElement.Elements(aresDT + "REG").FirstOrDefault();
			if (registraceElement != null)
			{
				result.RegistraceOR = new AresData.Classes.RegistraceOR();

				XElement szElement = registraceElement.Elements(aresDT + "SZ").FirstOrDefault();

				if (szElement != null)
				{
					XElement sdElement = szElement.Elements(aresDT + "SD").FirstOrDefault();

					if (sdElement != null)
					{
						result.RegistraceOR.NazevSoudu = (string)sdElement.Elements(aresDT + "T").FirstOrDefault();
						result.RegistraceOR.KodSoudu = (string)sdElement.Elements(aresDT + "K").FirstOrDefault();
					}

					result.RegistraceOR.SpisovaZnacka = (string)szElement.Elements(aresDT + "OV").FirstOrDefault();
				}
			}

			XElement npfElement = vypisOrElement.Elements(aresDT + "ZAU").Elements(aresDT + "PFO").Elements(aresDT + "NPF").FirstOrDefault();
			if (npfElement != null)
			{
				result.PravniForma = new AresData.Classes.PravniForma()
				{
					Nazev = (string)npfElement
				};
			}

			//obchodniRejstrikResponse.StavSubjektu = (string)vbasElement.Descendants(aresDT + "SSU").FirstOrDefault();

			result.Sidlo = new AresData.Classes.Sidlo()
			{
				Ulice = (string)vypisOrElement.Elements(aresDT + "ZAU").Elements(aresDT + "SI").Elements(aresDT + "NU").FirstOrDefault(),

				CisloDoAdresy = (string)vypisOrElement.Elements(aresDT + "ZAU").Elements(aresDT + "SI").Elements(aresDT + "CA").FirstOrDefault(),
				CisloPopisne = (string)vypisOrElement.Elements(aresDT + "ZAU").Elements(aresDT + "SI").Elements(aresDT + "CD").FirstOrDefault(),
				CisloOrientacni = (string)vypisOrElement.Elements(aresDT + "ZAU").Elements(aresDT + "SI").Elements(aresDT + "CO").FirstOrDefault(),

				Mesto = (string)vypisOrElement.Elements(aresDT + "ZAU").Elements(aresDT + "SI").Elements(aresDT + "N").FirstOrDefault(),
				MestskaCast = (string)vypisOrElement.Elements(aresDT + "ZAU").Elements(aresDT + "SI").Elements(aresDT + "NCO").FirstOrDefault(),
				Psc = (string)vypisOrElement.Elements(aresDT + "ZAU").Elements(aresDT + "SI").Elements(aresDT + "PSC").FirstOrDefault(),
				Stat = (string)vypisOrElement.Elements(aresDT + "ZAU").Elements(aresDT + "SI").Elements(aresDT + "NS").FirstOrDefault(),
				AdresaTextem = (string)vypisOrElement.Elements(aresDT + "ZAU").Elements(aresDT + "SI").Elements(aresDT + "AT").FirstOrDefault()
			};

			// statutární orgán
			var soElement = vypisOrElement.Elements(aresDT + "SO").FirstOrDefault();
			if (soElement != null)
			{
				result.StatutarniOrgan = new AresData.Classes.StatutarniOrgan();
				var statutartniOrganTextElement = soElement.Elements(aresDT + "T").FirstOrDefault();
				if (statutartniOrganTextElement != null)
				{
					result.StatutarniOrgan.Text = ((string)statutartniOrganTextElement).Trim();
				}
				var csoElement = soElement.Element(aresDT + "CSO");
				if (csoElement != null)
				{
					var cElement = csoElement.Element(aresDT + "C");
					if (cElement != null)
					{
						var foElement = cElement.Element(aresDT + "FO");
						if (foElement != null)
						{
							var jmenoElement = foElement.Element(aresDT + "J");
							if (jmenoElement != null)
							{
								result.StatutarniOrgan.KrestniJmeno = jmenoElement.Value;
							}
							var prijmeniElement = foElement.Element(aresDT + "P");
							if (prijmeniElement != null)
							{
								result.StatutarniOrgan.Prijmeni = prijmeniElement.Value;
							}
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Odešle dotaz do obchodního rejstříku pro dané IČ a vrátí odpověd jako XDocument objekt.
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
			throw new AresLoadException(String.Format("Chyba \"{0}\" při pokusu o získání dat ze služby ARES ({1}).", e.Message, requestUrl));
		}

		return aresResponseXDocument;
	}
}