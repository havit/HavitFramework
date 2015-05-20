using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Havit.Services.Ares
{
	/// <summary>
	/// Tøída implementující naèítání dat z obchodního rejstøíku (ARES).
	/// </summary>
	public class AresService
	{
		#region Const
		private const string AresBasicDataRequestUrl = "http://wwwinfo.mfcr.cz/cgi-bin/ares/darv_bas.cgi?ico=";
		private const string AresObchodniRejstrikDataRequestUrl = "http://wwwinfo.mfcr.cz/cgi-bin/ares/darv_or.cgi?ico=";
		#endregion

		#region Private members
		/// <summary>
		/// IÈ subjektu, kterého údaje chceme získat.
		/// </summary>
		private string Ico { get; set; }

		#endregion

		#region Timeout
		/// <summary>
		/// Timeout (v milisekundách) jednoho requestu pøi naèítání dat z ARESu.
		/// Pokud není hodnota nastavena, není délka requestu omezována (resp. je použito standardní nastavení .NETu).
		/// </summary>
		public int? Timeout
		{
			get; set;
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="ico">IÈO spoleènosti.</param>
		public AresService(string ico)
		{
			Ico = ico;
		}
		#endregion

		#region GetData

		/// <summary>
		/// Vrací strukturovanou odpovìd z obchodního rejstøíku.
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
				// pokus o vybalení výjimky (chceme øešit jen jedinou)
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
		#endregion

		#region LoadBasicData, ParseBasicData
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
					if (((int)item.Elements(aresDT + "EK").SingleOrDefault() == 1 /* Nenalezen */) && ((string)item.Elements(aresDT + "ET").SingleOrDefault()).Contains("Chyba 71 - nenalezeno"))
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
				this.ParseBasicData(aresResponseXDocument, aresDT, result);
			}
		}

		private void ParseBasicData(XDocument aresResponse, XNamespace aresDT, AresData result)
		{
		// Výpis BASIC (element).
			XElement vypisOrElement = aresResponse.Descendants(aresDT + "VBAS").FirstOrDefault();

			if (vypisOrElement != null)
			{
				result.Ico = (string)vypisOrElement.Elements(aresDT + "ICO").FirstOrDefault();
				result.Dic = (string)vypisOrElement.Elements(aresDT + "DIC").FirstOrDefault();
				result.NazevObchodniFirmy = (string)vypisOrElement.Elements(aresDT + "OF").FirstOrDefault(); // obchodní firma

				XElement npfElement = vypisOrElement.Elements(aresDT + "PF").Elements(aresDT + "NPF").FirstOrDefault();
				if (npfElement != null)
				{
					result.PravniForma = new AresData.Classes.PravniForma();
					result.PravniForma.Nazev = (string)npfElement;
				}

			}
		}
		#endregion

		#region LoadObchodniRejstrikData, ParseObchodniRejstrikData
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
					if (((int)item.Elements(aresDT + "EK").SingleOrDefault() == 1 /* Nenalezen */) && ((string)item.Elements(aresDT + "ET").SingleOrDefault()).Contains("Chyba 71 - nenalezeno"))
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
					result.PravniForma = new AresData.Classes.PravniForma();
					result.PravniForma.Nazev = (string)npfElement;
				}

				//obchodniRejstrikResponse.StavSubjektu = (string)vbasElement.Descendants(aresDT + "SSU").FirstOrDefault();

				result.Sidlo = new AresData.Classes.Sidlo();
				result.Sidlo.Ulice = (string)vypisOrElement.Elements(aresDT + "ZAU").Elements(aresDT + "SI").Elements(aresDT + "NU").FirstOrDefault();

				result.Sidlo.CisloDoAdresy = (string)vypisOrElement.Elements(aresDT + "ZAU").Elements(aresDT + "SI").Elements(aresDT + "CA").FirstOrDefault();
				result.Sidlo.CisloPopisne = (string)vypisOrElement.Elements(aresDT + "ZAU").Elements(aresDT + "SI").Elements(aresDT + "CD").FirstOrDefault();
				result.Sidlo.CisloOrientacni = (string)vypisOrElement.Elements(aresDT + "ZAU").Elements(aresDT + "SI").Elements(aresDT + "CO").FirstOrDefault();

				result.Sidlo.Mesto = (string)vypisOrElement.Elements(aresDT + "ZAU").Elements(aresDT + "SI").Elements(aresDT + "N").FirstOrDefault();
				result.Sidlo.MestskaCast = (string)vypisOrElement.Elements(aresDT + "ZAU").Elements(aresDT + "SI").Elements(aresDT + "NCO").FirstOrDefault();
				result.Sidlo.Psc = (string)vypisOrElement.Elements(aresDT + "ZAU").Elements(aresDT + "SI").Elements(aresDT + "PSC").FirstOrDefault();
				result.Sidlo.Stat = (string)vypisOrElement.Elements(aresDT + "ZAU").Elements(aresDT + "SI").Elements(aresDT + "NS").FirstOrDefault();
				result.Sidlo.AdresaTextem = (string)vypisOrElement.Elements(aresDT + "ZAU").Elements(aresDT + "SI").Elements(aresDT + "AT").FirstOrDefault();

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
		#endregion

		#region GetAresResponseXDocument
		/// <summary>
		/// Odešle dotaz do obchodního rejstøíku pro dané IÈ a vrátí odpovìd jako XDocument objekt.
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
				HttpWebResponse aresResponse = (HttpWebResponse)aresRequest.GetResponse();

				aresResponseXDocument = XDocument.Load(new StreamReader(aresResponse.GetResponseStream()));
			}
			catch (WebException e)
			{
				throw new AresLoadException(String.Format("Chyba \"{0}\" pøi pokusu o získání dat ze služby ARES ({1}).", e.Message, requestUrl));
			}

			return aresResponseXDocument;
		}
		#endregion

	}
}
