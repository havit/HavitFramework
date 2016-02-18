using System;
using System.Web;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Havit.Services.Ares
{
	/// <summary>
	/// Třída implementující načítání dat z obchodního rejstříku (ARES).
	/// </summary>
	public class AresPrehledSubjektuService
	{
		#region Const
		// http://wwwinfo.mfcr.cz/ares/ares_xml_es.html.cz
		private const string AresEkonomickySubjektRequestUrl = "http://wwwinfo.mfcr.cz/cgi-bin/ares/ares_es.cgi?obch_jm={0}&obec={1}&cestina=cestina&maxpoc=200&filtr=1&setrid=OBCHJM";
		#endregion

		#region Private members
		/// <summary>
		/// název subjektu, kterého údaje chceme získat.
		/// </summary>
		private string Nazev { get; set; }

		/// <summary>
		/// Obec/Město, kde se subjekt nachází.
		/// </summary>
		private string Obec { get; set; }
		#endregion

		#region Timeout
		/// <summary>
		/// Timeout (v milisekundách) jednoho requestu při načítání dat z ARESu.
		/// Pokud není hodnota nastavena, není délka requestu omezována (resp. je použito standardní nastavení .NETu).
		/// </summary>
		public int? Timeout
		{
			get; set;
		}
		#endregion

		#region GetData
		/// <summary>
		/// Vrací strukturovanou odpověd z obchodního rejstříku. Pouze existující subjekty, seřazené dle obchodního jména, max. 200 výsledků.
		/// </summary>
		public AresPrehledSubjektuResult GetData(string nazev, string obec = null)
		{
			AresPrehledSubjektuResult result = new AresPrehledSubjektuResult();
			List<Task> tasks = new List<Task>();

			this.Nazev = nazev;
			this.Obec = obec;

			tasks.Add(Task.Factory.StartNew(LoadEkonomickySubjektData, result));

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
		#endregion

		#region LoadEkonomickySubjektData
		private void LoadEkonomickySubjektData(object state)
		{
			string requestUrl = String.Format(AresEkonomickySubjektRequestUrl, System.Web.HttpUtility.UrlEncode(Nazev, Encoding.GetEncoding("windows-1250")), System.Web.HttpUtility.UrlEncode(Obec, Encoding.GetEncoding("windows-1250")));
			LoadData(requestUrl, state);
		}
		#endregion

		#region LoadData
		private void LoadData(String requestUrl, object state)
		{
			AresPrehledSubjektuResult result = (AresPrehledSubjektuResult)state;

			XDocument aresResponseXDocument = this.GetAresResponseXDocument(requestUrl);
			XNamespace aresDT = XNamespace.Get("http://wwwinfo.mfcr.cz/ares/xml_doc/schemas/ares/ares_datatypes/v_1.0.4");

			// Error
			XElement eElement = aresResponseXDocument.Root.Elements().Elements(aresDT + "Help").FirstOrDefault();
			if (eElement != null)
			{
				XElement rElement = eElement.Elements(aresDT + "R").FirstOrDefault();
				if (rElement != null && ((string)rElement).StartsWith("Nenalezen")  /* Nenalezen */)
				{
					return; // nehlásíme chybu ani neparsujeme data
				}

				if (rElement != null && (((string)rElement).Contains(" vede k výběru ") || ((string)rElement).Contains("Upravte hlediska pro vyhled") || ((string)rElement).Contains("přesáhl nastavenou mez"))   /* Příliš mnoho výsledků */)
				{
					result.PrilisMnohoVysledku = true;
					return;
				}

				throw new AresException((string)rElement);
			}

			lock (result)
			{
				this.ParseData(aresResponseXDocument, aresDT, result);
			}
		}
		#endregion

		#region ParseData
		private void ParseData(XDocument aresResponse, XNamespace aresDT, AresPrehledSubjektuResult result)
		{
			// Výpis BASIC (element).
			IEnumerable<XElement> vypisOrElements = aresResponse.Descendants(aresDT + "S");

			if ((vypisOrElements != null) && (vypisOrElements.Count() > 0))
			{
				foreach (XElement item in vypisOrElements)
				{
					AresPrehledSubjektuItem resultItem = new AresPrehledSubjektuItem();
					resultItem.Ico = (string)item.Elements(aresDT + "ico").FirstOrDefault();
					resultItem.Nazev = (string)item.Elements(aresDT + "ojm").FirstOrDefault();
					resultItem.Kontakt = (string)item.Elements(aresDT + "jmn").FirstOrDefault();
					result.Data.Add(resultItem);
				}
			}
		}
		#endregion

		#region GetAresResponseXDocument
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
		#endregion

	}
}
