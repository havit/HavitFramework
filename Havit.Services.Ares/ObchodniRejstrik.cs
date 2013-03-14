using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Xml.Linq;

namespace Havit.Services.Ares
{
	/// <summary>
	/// Třída implementující načítání dat z obchodního rejstříku (ARES).
	/// </summary>
	public class ObchodniRejstrik
	{
		#region Const
		private const string AresIcoRequestUrl = "http://wwwinfo.mfcr.cz/cgi-bin/ares/darv_bas.cgi?ico="; 
		#endregion

		#region Private members
		/// <summary>
		/// IČ subjektu, kterého údaje chceme získat.
		/// </summary>
		private string Ico { get; set; }

		/// <summary>
		/// Odklad vykonání requestu v sekundách.
		/// </summary>
		private int RequestDelay { get; set; }
		#endregion

		#region ObchodniRejstrik
		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="ico">IČO společnosti.</param>
		/// <param name="requestDelay">Odloží vykonání requestu o požadovaný čas v sekundách (default 0 s).</param>
		public ObchodniRejstrik(string ico, int requestDelay = 0)
		{
			Ico = ico;
			RequestDelay = requestDelay;
		}
		#endregion

		#region GetAresResponseXDocument
		/// <summary>
		/// Odešle dotaz do obchodního rejstříku pro dané IČ a vrátí odpověd jako XDocument objekt.
		/// </summary>
		private XDocument GetAresResponseXDocument()
		{
			XDocument aresResponseXDocument = null;

			string requestUrl = String.Format("{0}{1}", AresIcoRequestUrl, Ico);

			if (RequestDelay > 0)
			{
				Thread.Sleep(RequestDelay * 1000); // s -> ms
			}

			try
			{
				WebRequest aresRequest = HttpWebRequest.Create(requestUrl);
				HttpWebResponse aresResponse = (HttpWebResponse)aresRequest.GetResponse();

				aresResponseXDocument = XDocument.Load(new StreamReader(aresResponse.GetResponseStream()));
			}
			catch (WebException e)
			{
				throw new ApplicationException(String.Format("Chyba \"{0}\" při pokusu o získání dat ze služby ARES ({1}).", e.Message, requestUrl));

			}
			catch (Exception e)
			{
				throw new ApplicationException(String.Format("Chyba \"{0}\" při pokusu o získání dat ze služby ARES ({1}).", e.Message, requestUrl));
			}

			return aresResponseXDocument;
		}
		#endregion

		#region GetObchodniRejstrikResponse
		/// <summary>
		/// Vrací strukturovanou odpověd z obchodního rejstříka.
		/// </summary>
		public ObchodniRejstrikResponse GetObchodniRejstrikResponse()
		{
			ObchodniRejstrikResponse obchodniRejstrikResponse = new ObchodniRejstrikResponse();

			XDocument aresResponseXDocument = GetAresResponseXDocument();

			if (aresResponseXDocument.Root != null)
			{
				var dataTypesNamespace = aresResponseXDocument.Root.Elements().Select(item => item.GetNamespaceOfPrefix("D")).FirstOrDefault();

				if (dataTypesNamespace != null)
				{
					XElement eElement = aresResponseXDocument.Descendants(XName.Get("E", dataTypesNamespace.ToString())).FirstOrDefault();
					if (eElement != null)
					{
						// vyplníme vlastnost chyby a vrátíme objekt odpovědi (další elementy nezpracovávame)
						obchodniRejstrikResponse.ResponseErrorMessage = eElement.Value;
						return obchodniRejstrikResponse;
					}

					obchodniRejstrikResponse.Ico = (string)aresResponseXDocument.Descendants(XName.Get("ICO", dataTypesNamespace.ToString())).FirstOrDefault();
					obchodniRejstrikResponse.Dic = (string)aresResponseXDocument.Descendants(XName.Get("DIC", dataTypesNamespace.ToString())).FirstOrDefault();
					obchodniRejstrikResponse.NazevObchodniFirmy = (string)aresResponseXDocument.Descendants(XName.Get("OF", dataTypesNamespace.ToString())).FirstOrDefault();
					
					XElement dvElement = aresResponseXDocument.Descendants(XName.Get("DV", dataTypesNamespace.ToString())).FirstOrDefault();

					if (dvElement != null)
					{
						obchodniRejstrikResponse.DenZapisu = DateTime.ParseExact(dvElement.Value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
					}

					obchodniRejstrikResponse.NazevSoudu = (string)aresResponseXDocument.Descendants(XName.Get("T", dataTypesNamespace.ToString())).FirstOrDefault();
					obchodniRejstrikResponse.KodSoudu = (string)aresResponseXDocument.Descendants(XName.Get("K", dataTypesNamespace.ToString())).FirstOrDefault();
					obchodniRejstrikResponse.SpisovaZnacka = (string)aresResponseXDocument.Descendants(XName.Get("OV", dataTypesNamespace.ToString())).FirstOrDefault();
					obchodniRejstrikResponse.PravniForma = (string)aresResponseXDocument.Descendants(XName.Get("NPF", dataTypesNamespace.ToString())).FirstOrDefault();
					obchodniRejstrikResponse.StavSubjektu = (string)aresResponseXDocument.Descendants(XName.Get("SSU", dataTypesNamespace.ToString())).FirstOrDefault();
					obchodniRejstrikResponse.SidloUlice = (string)aresResponseXDocument.Descendants(XName.Get("NU", dataTypesNamespace.ToString())).FirstOrDefault();
					obchodniRejstrikResponse.SidloCisloPopisne = (string)aresResponseXDocument.Descendants(XName.Get("CD", dataTypesNamespace.ToString())).FirstOrDefault();
					obchodniRejstrikResponse.SidloCisloOrientacni = (string)aresResponseXDocument.Descendants(XName.Get("CO", dataTypesNamespace.ToString())).FirstOrDefault();
					obchodniRejstrikResponse.SidloMesto = (string)aresResponseXDocument.Descendants(XName.Get("N", dataTypesNamespace.ToString())).FirstOrDefault();
					obchodniRejstrikResponse.SidloMestskaCast = (string)aresResponseXDocument.Descendants(XName.Get("NCO", dataTypesNamespace.ToString())).FirstOrDefault();
					obchodniRejstrikResponse.SidloPsc = (string)aresResponseXDocument.Descendants(XName.Get("PSC", dataTypesNamespace.ToString())).FirstOrDefault();
					obchodniRejstrikResponse.SidloStat = (string)aresResponseXDocument.Descendants(XName.Get("NS", dataTypesNamespace.ToString())).FirstOrDefault();
				}
			}
			
			return obchodniRejstrikResponse;
		}
		#endregion
	}
}
