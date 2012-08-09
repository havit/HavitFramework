using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;
using System.Collections.Specialized;

namespace Havit.Web
{
	/// <summary>
	/// Třída zajišťující navigaci mezi stránkami s historií navigace.
	/// Adresy navigace jsou předávány v QueryString jako komprimovaný seznam adres.
	/// Hodnoty nepřežívají postback, PageNavigator není control.
	/// </summary>
	public class PageNavigator
	{
		#region PageNavigatorUrlsQueryParameterName (private const)
		/// <summary>
		/// Název parametru používaného v QueryStringu, hodnota nese adresy historie.
		/// </summary>
		private const string UrlsQueryParameterName = "NavigatorUrls";
 		#endregion

		#region Current (static)
		/// <summary>
		/// Vrací instanci PageNavigatoru pro aktuální HttpContext. "HttpContext signleton".
		/// </summary>
		public static PageNavigator Current
		{
			get
			{
				HttpContext context = HttpContext.Current;

				if (context == null)
				{
					throw new InvalidOperationException("HttpContext.Current je null.");
				}

				PageNavigator result = (PageNavigator)context.Items[typeof(PageNavigator)];
				if (result == null)
				{
					result = new PageNavigator(context);
					context.Items[typeof(PageNavigator)] = result;
				}
				return result;
			}
		}
		#endregion

		#region HistoryUrls
		/// <summary>
		/// Seznam adres historie navigace (protected)
		/// </summary>
		protected List<string> HistoryUrls
		{
            get
            {
                return _historyUrls;
            }
            private set
            {
                _historyUrls = value;
            }
		}
        private List<string> _historyUrls;
		#endregion

		private HttpContext _currentContext;

		#region Constructors
		/// <summary>
		/// Konstruktor.
		/// Inicializuje kolekci HistoryUrls.
		/// </summary>
		protected PageNavigator(HttpContext context)
		{
			_currentContext = context;
			InitializeHistoryUrls();
		}
		#endregion

		#region InitializeHistoryUrls
		/// <summary>
		/// Inicializuje kolekci HistoryUrls, voláno z konstruktoru.
		/// </summary>
		private void InitializeHistoryUrls()
		{
			HistoryUrls = new List<string>();

			string historyParameterValue = _currentContext.Request.QueryString[PageNavigator.UrlsQueryParameterName];
			if (String.IsNullOrEmpty(historyParameterValue))
			{
				// pokud nemáme hodnotu parametru z QueryStringu, končíme zpracování
				// zůstává nám inicializovaná (umyslně) prázdná kolekce HistoryUrls
				return;
			}

			// pokud máme hodnotu parametru , provedeme dekompresi a parse adres historie
			byte[] buffer = HttpServerUtility.UrlTokenDecode(historyParameterValue);
			if (buffer != null) // pokud je hodnota parametru poškozena, získáváme null (bohužel není v dokumentaci MSDN uvedeno)
			{
				using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(buffer, false))
				{
					using (System.IO.Compression.DeflateStream compressStream = new System.IO.Compression.DeflateStream(memoryStream, System.IO.Compression.CompressionMode.Decompress))
					{
						using (StreamReader reader = new StreamReader(compressStream, Encoding.UTF8))
						{
							string line;
							while ((line = reader.ReadLine()) != null) // přečteme řádek do proměnné line a porovnáme s hodnotou null
							{
								HistoryUrls.Add(line);
							}
						}
					}
				}
			}
		}
		#endregion

		#region NavigateFromTo
		/// <summary>
		/// Provede přesměrování s předáním adres navigace. Do historie adres navigace není přidána žádná adresa.
		/// </summary>
		/// <param name="toUrl">Adresa, na kterou bude přesměrováno.</param>
		/// <exception cref="ArgumentException">
		/// Pokud je toUrl null nebo prázdný řetězec.
		/// </exception>
		public void TransitionalNavigateTo(string toUrl)
		{
			string redirectUrl = GetTransitionalNavigationUrlTo(toUrl);
			_currentContext.Response.Redirect(redirectUrl);
		}
		#endregion

		#region NavigateFromTo
		/// <summary>
		/// Provede přesměrování s předáním adres navigace. Do historie adres navigace ja přidána adresa v parametru fromUrls.
		/// </summary>
		/// <param name="fromUrls">Adresa, která bude přidána do adres navigace (slouží pro návrat na aktuální stránku).</param>
		/// <param name="toUrl">Adresa, na kterou bude přesměrováno.</param>
		/// <exception cref="ArgumentException">
		/// Pokud je fromUrl null nebo prázdný řetězec.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Pokud je toUrl null nebo prázdný řetězec.
		/// </exception>
		public void NavigateFromTo(string fromUrl, string toUrl)
		{
			string redirectUrl = GetNavigationUrlFromTo(fromUrl, toUrl);
			_currentContext.Response.Redirect(redirectUrl);
		}
		#endregion

		#region NavigateFromRawUrlTo
		/// <summary>
		/// Provede přesměrování s předáním adres navigace. Jako adresa pro návrat na aktuální stránku se použije HttpRequest.RawUrl.
		/// </summary>
		/// <param name="toUrl">Adresa, na kterou bude přesměrováno.</param>
		/// <exception cref="ArgumentException">
		/// Pokud je toUrl null nebo prázdný řetězec.
		/// </exception>
		public void NavigateFromRawUrlTo(string toUrl)
		{
			string redirectUrl = GetNavigationUrlFromRawUrlTo(toUrl);
			_currentContext.Response.Redirect(redirectUrl);
		}
		#endregion

		#region NavigateBack
		/// <summary>
		/// Provede přesměrování na poslední adresu v historii adres.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">
		/// Pokud není k dispozici adresa pro návrat.
		/// </exception>
		public void NavigateBack()
		{
			if (!CanNavigateBack())
			{
				throw new InvalidOperationException("Není k dispozici Url pro návrat.");
			}

//			this.SetTransitional();
			string url = HistoryUrls[0];
			HistoryUrls.RemoveAt(0);

			string targetUrl = GetNavigationToUrlInternal(null, url, true);
			_currentContext.Response.Redirect(targetUrl);
		}
		#endregion

		#region CanNavigateBack
		/// <summary>
		/// Udává, zda je možné provést navigaci zpět (tj. zda je k dispozici adresa pro návrat).
		/// </summary>
		public bool CanNavigateBack()
		{
			return HistoryUrls.Count > 0;
		} 
		#endregion

		#region GetTransitionalNavigationUrlTo
		/// <summary>
		/// Vrátí adresu pro přesměrování s předáním adres navigace. Do historie adres navigace není přidána žádná adresa.
		/// </summary>
		/// <param name="toUrl">Adresa, na kterou bude přesměrováno.</param>
		/// <exception cref="ArgumentException">
		/// Pokud je toUrl null nebo prázdný řetězec.
		/// </exception>
		public string GetTransitionalNavigationUrlTo(string toUrl)
		{
			if (String.IsNullOrEmpty(toUrl))
			{
				throw new ArgumentException("Není zadána hodnota.", "toUrl");
			}

			toUrl = GetUrlWithoutNavigationUrlParameter(toUrl);
			return GetNavigationToUrlInternal(null, toUrl, true);
		}
		#endregion

		#region GetNavigationUrlFromTo
		/// <summary>
		/// Vrátí url pro přesměrování s předáním adres navigace.
		/// </summary>
		/// <param name="fromUrls">Adresa, která bude přidána do adres navigace (slouží pro návrat na aktuální stránku).</param>
		/// <param name="toUrl">Adresa, na kterou bude přesměrováno.</param>
		/// <exception cref="ArgumentException">
		/// Pokud je fromUrl null nebo prázdný řetězec.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Pokud je toUrl null nebo prázdný řetězec.
		/// </exception>
		public string GetNavigationUrlFromTo(string fromUrl, string toUrl)
		{
			if (String.IsNullOrEmpty(fromUrl))
			{
				throw new ArgumentException("Není zadána hodnota.", "fromUrl");
			}

			if (String.IsNullOrEmpty(toUrl))
			{
				throw new ArgumentException("Není zadána hodnota.", "toUrl");
			}

			fromUrl = GetUrlWithoutNavigationUrlParameter(fromUrl);
			toUrl = GetUrlWithoutNavigationUrlParameter(toUrl);

			return GetNavigationToUrlInternal(fromUrl, toUrl, true);
		}
		
		#endregion

		#region GetNavigationUrlFromRawUrlTo
		/// <summary>
		/// Vrátí adresu pro přesměrování s předáním adres navigace. Jako adresa pro návrat na aktuální stránku se použije HttpRequest.RawUrl.
		/// </summary>
		/// <param name="toUrl">Adresa, na kterou bude přesměrováno.</param>
		/// <exception cref="ArgumentException">
		/// Pokud je toUrl null nebo prázdný řetězec.
		/// </exception>
		public string GetNavigationUrlFromRawUrlTo(string toUrl)
		{
			if (String.IsNullOrEmpty(toUrl))
			{
				throw new ArgumentException("Není zadána hodnota.", "toUrl");
			}

			string fromUrl = GetUrlWithoutNavigationUrlParameter(_currentContext.Request.RawUrl);
			toUrl = GetUrlWithoutNavigationUrlParameter(toUrl);

            return GetNavigationToUrlInternal(fromUrl, toUrl, true);
		}
		
		#endregion

		#region GetNavigationToUrlInternal (private)
		/// <summary>
		/// Vrátí adresu pro přesměrování s předáním adres navigace.
		/// </summary>
		/// <param name="fromUrls">Adresa, která bude přidána do adres navigace (slouží pro návrat na aktuální stránku).</param>
		/// <param name="toUrl">Adresa, na kterou bude přesměrováno.</param>
		/// <param name="passHistoryUrls">True, pokud má být přidán parametr s historií adres navigace.</param>
		/// <returns>
		/// Pokud je passHistoryUrls false, vrací toUrl.
		/// Jinak sestaví url tak, že k toUrl přidá parametr se seznamem adres historie. Bere ohled na maximální délku url.
		/// </returns>
		private string GetNavigationToUrlInternal(string fromUrl, string toUrl, bool passHistoryUrls)
		{
			if (!passHistoryUrls || (HistoryUrls.Count == 0 && String.IsNullOrEmpty(fromUrl)))
			{
				// pokud nemáme předat historii adres nebo nemáme co přidat, vrátíme nezměněnou adresu k navigaci.
				return toUrl;
			}

			int maxSerializeIndex = HistoryUrls.Count;

			while (true)
			{
				// získáme hodnotu parametru při předání nejvýše maxSerializeIndex adres historie
				// (0 znamená, že se předá jen fromUrl)
				string historyParameterValue = GetNavigationUrlsValue(fromUrl, maxSerializeIndex);
				string result = String.Format(
					"{0}{1}{2}={3}",
					toUrl, // 0
					toUrl.Contains("?") ? "&" : "?", // 1
					PageNavigator.UrlsQueryParameterName, // 2
					historyParameterValue// 3						
				);

				bool lengthValid = HttpServerUtilityExt.ResolveUrl(result).Length <= 2048;

				if (lengthValid)
				{
					// pokud url splňuje požadavky na délku, vrátíme ji
					return result;
				}
				else
				{
					// url nesplňuje parametry délky
					if (maxSerializeIndex > 0)
					{
						// pokud jsou serializovány adresy z historie, snížíme jejich počet o jednu
						maxSerializeIndex -= 1;
					}
					else
					{
						// pokud už nemáme co z historie ubrat, vrátíme url bez historie adres.
						return GetNavigationToUrlInternal(null, toUrl, false);
					}
				}
			}
		}
		#endregion

		#region GetNavigationUrlsValue (private)
		/// <summary>
		/// Vrací hodnotu parametru, který nese seznam adres historie.
		/// </summary>
		/// <param name="fromUrl">Url pro návrat na aktuální stránku.</param>
		/// <param name="maxSerializeIndex">Maximální počet položek historie, který je serializován.</param>
		/// <returns>Encodovaná hodnota, je možné ji přímo použít do url.</returns>
		private string GetNavigationUrlsValue(string fromUrl, int maxSerializeIndex)
		{
		    byte[] buffer;
		    using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
		    {
		        using (System.IO.Compression.DeflateStream compressStream = new System.IO.Compression.DeflateStream(memoryStream, System.IO.Compression.CompressionMode.Compress, true))
		        {
		            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(compressStream, Encoding.UTF8))
		            {
		                if (!String.IsNullOrEmpty(fromUrl))
		                {
		                    writer.WriteLine(fromUrl);
		                }
		                for (int i = 0; i < maxSerializeIndex; i++)
		                {
		                    writer.WriteLine(HistoryUrls[i]);
		                }
		            }                    
		        }

		        buffer = new byte[(int)memoryStream.Length];
		        memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
		        memoryStream.Read(buffer, 0, buffer.Length);
		    }
		    return HttpServerUtility.UrlTokenEncode(buffer);
		} 
		#endregion

		#region GetUrlWithoutNavigationUrlParameter (private)
		/// <summary>
		/// Vrátí url adresu bez parametru s historií adres navigace.
		/// </summary>
		private string GetUrlWithoutNavigationUrlParameter(string url)
		{
			if (!url.ToLower().Contains(PageNavigator.UrlsQueryParameterName.ToLower()))
			{
				// pokud v url ani není obsaženo slovo názvu parametru, vracíme původní URL.
				return url;
			}

			string[] urlParts = url.Split('?');
			if (urlParts.Length < 2)
			{
				// pokud nemáme část s QP, vracíme původní adresu
				return url;
			}

			// rozebereme querystring
			QueryStringBuilder qsb = QueryStringBuilder.Parse(urlParts[1]);
			//odstranime hodnotu s historií adres, pokud existuje
			qsb.Remove(PageNavigator.UrlsQueryParameterName);
			// a opet slozime adresu
			return qsb.GetUrlWithQueryString(urlParts[0]);

			//// rozebereme QS na hodnoty
			//string[] queryStringParts = urlParts[1].Split('&');
			//NameValueCollection parameters = new NameValueCollection();
			//foreach (string queryStringPart in queryStringParts)
			//{
			//    // každý parametr přidáme do kolekce parametrů
			//    string[] itemInfo = queryStringPart.Split('=');
			//    parameters.Add(itemInfo[0], itemInfo[1]);
			//}

			//if (String.IsNullOrEmpty(parameters[PageNavigator.UrlsQueryParameterName]))
			//{
			//    // pokud není hodnota s historií adres, vracíme původní url
			//    return url;
			//}

			//// odstraníme hodnota s historií adres
			//parameters.Remove(PageNavigator.UrlsQueryParameterName);

			//// sestavíme url a tu vrátíme
			//StringBuilder result = new StringBuilder();
			//foreach (string key in parameters.Keys)
			//{
			//    if (result.Length > 0)
			//    {
			//        result.Append("&");
			//    }
			//    result.Append(key);
			//    result.Append("=");
			//    result.Append(parameters[key]);
			//}

			//result.Insert(0, "?");
			//result.Insert(0, urlParts[0]);

			//return result.ToString();
		}
		
		#endregion
	}
}
