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

			Task.WaitAll(tasks.ToArray());

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

			// Error
			XElement eElement = aresResponseXDocument.Root.Elements().Elements(aresDT + "E").SingleOrDefault();
			if (eElement != null)
			{
				if ((int)eElement.Elements(aresDT + "EK").SingleOrDefault() == 1 /* Nenalezen */)
				{
					return; // nehlásíme chybu ani neparsujeme data
				}

				throw new AresException((string)eElement.Elements(aresDT + "ET").SingleOrDefault());
			}

			lock (result)
			{
				this.ParseBasicData(aresResponseXDocument, aresDT, result);
			}
		}

		private void ParseBasicData(XDocument aresResponse, XNamespace aresDT, AresData result)
		{
		// Výpis BASIC (element).
			XElement vypisOrElement = aresResponse.Descendants(aresDT + "VBAS").SingleOrDefault();

			if (vypisOrElement != null)
			{
				result.Ico = (string)vypisOrElement.Elements(aresDT + "ICO").SingleOrDefault();
				result.Dic = (string)vypisOrElement.Elements(aresDT + "DIC").SingleOrDefault();
				result.NazevObchodniFirmy = (string)vypisOrElement.Elements(aresDT + "OF").SingleOrDefault(); // obchodní firma

				XElement npfElement = vypisOrElement.Elements(aresDT + "PF").Elements(aresDT + "NPF").SingleOrDefault();
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

			// Error
			XElement eElement = aresResponseXDocument.Root.Elements().Elements(aresDT + "E").SingleOrDefault();
			if (eElement != null)
			{
				if ((int)eElement.Elements(aresDT + "EK").SingleOrDefault() == 1 /* Nenalezen */)
				{
					return; // nehlásíme chybu ani neparsujeme data
				}

				throw new AresException((string)eElement.Elements(aresDT + "ET").SingleOrDefault());
			}

			lock (result)
			{
				this.ParseObchodniRejstrikData(aresResponseXDocument, aresDT, result);
			}
		}

		private void ParseObchodniRejstrikData(XDocument aresResponse, XNamespace aresDT, AresData result)
		{
			// Výpis OR (element).
			XElement vypisOrElement = aresResponse.Descendants(aresDT + "Vypis_OR").SingleOrDefault();

			if (vypisOrElement != null)
			{
				result.Ico = (string)vypisOrElement.Elements(aresDT + "ZAU").Elements(aresDT + "ICO").SingleOrDefault();
				result.NazevObchodniFirmy = (string)vypisOrElement.Elements(aresDT + "ZAU").Elements(aresDT + "OF").SingleOrDefault(); // obchodní firma

				// Registrace OR
				XElement registraceElement = vypisOrElement.Elements(aresDT + "REG").SingleOrDefault();
				if (registraceElement != null)
				{
					result.RegistraceOR = new AresData.Classes.RegistraceOR();

					XElement szElement = registraceElement.Elements(aresDT + "SZ").SingleOrDefault();

					if (szElement != null)
					{
						XElement sdElement = szElement.Elements(aresDT + "SD").SingleOrDefault();

						if (sdElement != null)
						{
							result.RegistraceOR.NazevSoudu = (string)sdElement.Elements(aresDT + "T").SingleOrDefault();
							result.RegistraceOR.KodSoudu = (string)sdElement.Elements(aresDT + "K").SingleOrDefault();
						}

						result.RegistraceOR.SpisovaZnacka = (string)szElement.Elements(aresDT + "OV").SingleOrDefault();
					}
				}

				XElement npfElement = vypisOrElement.Elements(aresDT + "ZAU").Elements(aresDT + "PFO").Elements(aresDT + "NPF").SingleOrDefault();
				if (npfElement != null)
				{
					result.PravniForma = new AresData.Classes.PravniForma();
					result.PravniForma.Nazev = (string)npfElement;
				}

				//obchodniRejstrikResponse.StavSubjektu = (string)vbasElement.Descendants(aresDT + "SSU").SingleOrDefault();

				result.Sidlo = new AresData.Classes.Sidlo();
				result.Sidlo.Ulice = (string)vypisOrElement.Elements(aresDT + "ZAU").Elements(aresDT + "SI").Elements(aresDT + "NU").SingleOrDefault();
				
				result.Sidlo.CisloDoAdresy = (string)vypisOrElement.Elements(aresDT + "ZAU").Elements(aresDT + "SI").Elements(aresDT + "CA").SingleOrDefault();
				result.Sidlo.CisloPopisne = (string)vypisOrElement.Elements(aresDT + "ZAU").Elements(aresDT + "SI").Elements(aresDT + "CD").SingleOrDefault();
				result.Sidlo.CisloOrientacni = (string)vypisOrElement.Elements(aresDT + "ZAU").Elements(aresDT + "SI").Elements(aresDT + "CO").SingleOrDefault();

				result.Sidlo.Mesto = (string)vypisOrElement.Elements(aresDT + "ZAU").Elements(aresDT + "SI").Elements(aresDT + "N").SingleOrDefault();
				result.Sidlo.MestskaCast = (string)vypisOrElement.Elements(aresDT + "ZAU").Elements(aresDT + "SI").Elements(aresDT + "NCO").SingleOrDefault();
				result.Sidlo.Psc = (string)vypisOrElement.Elements(aresDT + "ZAU").Elements(aresDT + "SI").Elements(aresDT + "PSC").SingleOrDefault();
				result.Sidlo.Stat = (string)vypisOrElement.Elements(aresDT + "ZAU").Elements(aresDT + "SI").Elements(aresDT + "NS").SingleOrDefault();

				// statutární orgán
				var soElement = vypisOrElement.Elements(aresDT + "SO").SingleOrDefault();
				if (soElement != null)
				{
					result.StatutarniOrgan = new AresData.Classes.StatutarniOrgan();
					var statutartniOrganTextElement = soElement.Elements(aresDT + "T").SingleOrDefault();
					if (statutartniOrganTextElement != null)
					{
						result.StatutarniOrgan.Text = ((string)statutartniOrganTextElement).Trim();
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
			catch (Exception e)
			{
				throw new AresLoadException(String.Format("Chyba \"{0}\" pøi pokusu o získání dat ze služby ARES ({1}).", e.Message, requestUrl));
			}

			return aresResponseXDocument;
		}
		#endregion

	}
}
