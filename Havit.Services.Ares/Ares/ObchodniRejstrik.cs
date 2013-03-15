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
	/// Tøída implementující naèítání dat z obchodního rejstøíku (ARES).
	/// </summary>
	public class ObchodniRejstrik
	{
		#region Const
		private const string AresIcoRequestUrl = "http://wwwinfo.mfcr.cz/cgi-bin/ares/darv_bas.cgi?ico="; 
		#endregion

		#region Private members
		/// <summary>
		/// IÈ subjektu, kterého údaje chceme získat.
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
		/// <param name="ico">IÈO spoleènosti.</param>
		/// <param name="requestDelay">Odloží vykonání requestu o požadovaný èas v sekundách (default 0 s).</param>
		public ObchodniRejstrik(string ico, int requestDelay = 0)
		{
			Ico = ico;
			RequestDelay = requestDelay;
		}
		#endregion

		#region GetAresResponseXDocument
		/// <summary>
		/// Odešle dotaz do obchodního rejstøíku pro dané IÈ a vrátí odpovìd jako XDocument objekt.
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
				throw new ApplicationException(String.Format("Chyba \"{0}\" pøi pokusu o získání dat ze služby ARES ({1}).", e.Message, requestUrl));

			}
			catch (Exception e)
			{
				throw new ApplicationException(String.Format("Chyba \"{0}\" pøi pokusu o získání dat ze služby ARES ({1}).", e.Message, requestUrl));
			}

			return aresResponseXDocument;
		}
		#endregion

		#region GetObchodniRejstrikResponse
		/// <summary>
		/// Vrací strukturovanou odpovìd z obchodního rejstøíka.
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
					XElement eElement = aresResponseXDocument.Descendants(dataTypesNamespace + "E").FirstOrDefault();
					if (eElement != null)
					{
						// vyplníme vlastnost chyby a vrátíme objekt odpovìdi (další elementy nezpracovávame)
						obchodniRejstrikResponse.ResponseErrorMessage = eElement.Value;
						return obchodniRejstrikResponse;
					}
					
					XElement vbasElement = aresResponseXDocument.Descendants(dataTypesNamespace + "VBAS").FirstOrDefault();

					if (vbasElement != null)
					{
						obchodniRejstrikResponse.Ico = (string)vbasElement.Descendants(dataTypesNamespace + "ICO").FirstOrDefault();
						obchodniRejstrikResponse.Dic = (string)vbasElement.Descendants(dataTypesNamespace + "DIC").FirstOrDefault();
						obchodniRejstrikResponse.NazevObchodniFirmy = (string)vbasElement.Descendants(dataTypesNamespace + "OF").FirstOrDefault();

						XElement dvElement = vbasElement.Descendants(dataTypesNamespace + "DV").FirstOrDefault();

						if (dvElement != null)
						{
							obchodniRejstrikResponse.DenZapisu = DateTime.ParseExact(dvElement.Value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
						}

						XElement rorElement = vbasElement.Descendants(dataTypesNamespace + "ROR").FirstOrDefault(); // ROR - registraèní organizace?

						if (rorElement != null)
						{
							XElement szElement = rorElement.Descendants(dataTypesNamespace + "SZ").FirstOrDefault();

							if (szElement != null)
							{
								XElement sdElement = rorElement.Descendants(dataTypesNamespace + "SD").FirstOrDefault();

								if (sdElement != null)
								{
									obchodniRejstrikResponse.NazevSoudu = (string)sdElement.Descendants(dataTypesNamespace + "T").FirstOrDefault();
									obchodniRejstrikResponse.KodSoudu = (string)sdElement.Descendants(dataTypesNamespace + "K").FirstOrDefault();
								}

								obchodniRejstrikResponse.SpisovaZnacka = (string)szElement.Descendants(dataTypesNamespace + "OV").FirstOrDefault();
							}
						}

						obchodniRejstrikResponse.PravniForma = (string)vbasElement.Descendants(dataTypesNamespace + "NPF").FirstOrDefault();
						obchodniRejstrikResponse.StavSubjektu = (string)vbasElement.Descendants(dataTypesNamespace + "SSU").FirstOrDefault();
						obchodniRejstrikResponse.SidloUlice = (string)vbasElement.Descendants(dataTypesNamespace + "NU").FirstOrDefault();
						obchodniRejstrikResponse.SidloCisloPopisne = (string)vbasElement.Descendants(dataTypesNamespace + "CD").FirstOrDefault();
						obchodniRejstrikResponse.SidloCisloOrientacni = (string)vbasElement.Descendants(dataTypesNamespace + "CO").FirstOrDefault();
						obchodniRejstrikResponse.SidloMesto = (string)vbasElement.Descendants(dataTypesNamespace + "N").FirstOrDefault();
						obchodniRejstrikResponse.SidloMestskaCast = (string)vbasElement.Descendants(dataTypesNamespace + "NCO").FirstOrDefault();
						obchodniRejstrikResponse.SidloPsc = (string)vbasElement.Descendants(dataTypesNamespace + "PSC").FirstOrDefault();
						obchodniRejstrikResponse.SidloStat = (string)vbasElement.Descendants(dataTypesNamespace + "NS").FirstOrDefault();
					}
				}
			}
			
			return obchodniRejstrikResponse;
		}
		#endregion
	}
}
