﻿using System.Text;
using System.Web;
using Havit.Diagnostics.Contracts;

namespace Havit.Web;

/// <summary>
/// Třída zajišťující navigaci mezi stránkami s historií navigace.
/// Adresy navigace jsou předávány v QueryString jako komprimovaný seznam adres.
/// Hodnoty nepřežívají postback, PageNavigator není control.
/// </summary>
public class PageNavigator
{
	/// <summary>
	/// Název parametru používaného v QueryStringu, hodnota nese adresy historie.
	/// </summary>
	private const string UrlsQueryParameterName = "NavigatorUrls";

	/// <summary>
	/// Vrací instanci PageNavigatoru pro aktuální HttpContext. "HttpContext signleton".
	/// </summary>
	public static PageNavigator Current
	{
		get
		{
			HttpContext context = HttpContext.Current;
			Contract.Requires<InvalidOperationException>(context != null, "HttpContext.Current je null.");

			PageNavigator result = (PageNavigator)context.Items[typeof(PageNavigator)];
			if (result == null)
			{
				result = new PageNavigator(context);
				context.Items[typeof(PageNavigator)] = result;
			}
			return result;
		}
	}

	private readonly HttpContext _currentContext;

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

	/// <summary>
	/// Konstruktor.
	/// Inicializuje kolekci HistoryUrls.
	/// </summary>
	protected PageNavigator(HttpContext context)
	{
		_currentContext = context;
		InitializeHistoryUrls();
	}

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
		byte[] buffer = null;
		try
		{
			buffer = HttpServerUtility.UrlTokenDecode(historyParameterValue);
		}
		catch (FormatException)
		{
			// noop
		}

		if (buffer != null) // pokud je hodnota parametru poškozena, získáváme null
		{
			using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(buffer, false))
			{
				using (System.IO.Compression.DeflateStream compressStream = new System.IO.Compression.DeflateStream(memoryStream, System.IO.Compression.CompressionMode.Decompress))
				{
					using (StreamReader reader = new StreamReader(compressStream, Encoding.UTF8))
					{
						string line = reader.ReadLine();
						while (line != null) // přečteme řádek do proměnné line a porovnáme s hodnotou null
						{
							HistoryUrls.Add(line);
							line = reader.ReadLine();
						}
					}
				}
			}
		}
	}

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

	/// <summary>
	/// Provede přesměrování s předáním adres navigace. Do historie adres navigace ja přidána adresa v parametru fromUrls.
	/// </summary>
	/// <param name="fromUrl">Adresa, která bude přidána do adres navigace (slouží pro návrat na aktuální stránku).</param>
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

		string targetUrl = GetNavigationBackUrl();
		_currentContext.Response.Redirect(targetUrl);
	}

	/// <summary>
	/// Udává, zda je možné provést navigaci zpět (tj. zda je k dispozici adresa pro návrat).
	/// </summary>
	public bool CanNavigateBack()
	{
		return HistoryUrls.Count > 0;
	}

	/// <summary>
	/// Vrátí adresu pro přesměrování s předáním adres navigace. Do historie adres navigace není přidána žádná adresa.
	/// </summary>
	/// <param name="toUrl">Adresa, na kterou bude přesměrováno.</param>
	/// <exception cref="ArgumentException">
	/// Pokud je toUrl null nebo prázdný řetězec.
	/// </exception>
	public string GetTransitionalNavigationUrlTo(string toUrl)
	{
		Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(toUrl), nameof(toUrl));

		toUrl = GetUrlWithoutNavigationUrlParameter(toUrl);
		return GetNavigationToUrlInternal(null, toUrl, true);
	}

	/// <summary>
	/// Vrátí url pro přesměrování s předáním adres navigace.
	/// </summary>
	/// <param name="fromUrl">Adresa, která bude přidána do adres navigace (slouží pro návrat na aktuální stránku).</param>
	/// <param name="toUrl">Adresa, na kterou bude přesměrováno.</param>
	/// <exception cref="ArgumentException">
	/// Pokud je fromUrl null nebo prázdný řetězec.
	/// </exception>
	/// <exception cref="ArgumentException">
	/// Pokud je toUrl null nebo prázdný řetězec.
	/// </exception>
	public string GetNavigationUrlFromTo(string fromUrl, string toUrl)
	{
		Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fromUrl), nameof(fromUrl));
		Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(toUrl), nameof(toUrl));

		fromUrl = GetUrlWithoutNavigationUrlParameter(fromUrl);
		toUrl = GetUrlWithoutNavigationUrlParameter(toUrl);

		return GetNavigationToUrlInternal(fromUrl, toUrl, true);
	}

	/// <summary>
	/// Vrátí adresu pro přesměrování s předáním adres navigace. Jako adresa pro návrat na aktuální stránku se použije HttpRequest.RawUrl.
	/// </summary>
	/// <param name="toUrl">Adresa, na kterou bude přesměrováno.</param>
	/// <exception cref="ArgumentException">
	/// Pokud je toUrl null nebo prázdný řetězec.
	/// </exception>
	public string GetNavigationUrlFromRawUrlTo(string toUrl)
	{
		Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(toUrl), nameof(toUrl));

		string fromUrl = GetUrlWithoutNavigationUrlParameter(_currentContext.Request.RawUrl);
		toUrl = GetUrlWithoutNavigationUrlParameter(toUrl);

		return GetNavigationToUrlInternal(fromUrl, toUrl, true);
	}

	/// <summary>
	/// Vrátí adresu pro přesměrování zpět. Adresa pro návrat zpět se bere z historie, ostatních adres v historie se zachovají.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	/// Pokud není k dispozici adresa pro návrat.
	/// </exception> 
	public string GetNavigationBackUrl()
	{
		if (!CanNavigateBack())
		{
			throw new InvalidOperationException("Není k dispozici Url pro návrat.");
		}

		string url = HistoryUrls[0];
		HistoryUrls.RemoveAt(0);
		string targetUrl = GetNavigationToUrlInternal(null, url, true);
		HistoryUrls.Insert(0, url);
		return targetUrl;
	}

	/// <summary>
	/// Vrátí adresu pro přesměrování s předáním adres navigace.
	/// </summary>
	/// <param name="fromUrl">Adresa, která bude přidána do adres navigace (slouží pro návrat na aktuální stránku).</param>
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
				historyParameterValue); // 3						

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
	}
}
