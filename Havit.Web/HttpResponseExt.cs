using System.Web;
using Havit.Diagnostics.Contracts;

namespace Havit.Web;

/// <summary>
/// Obsahuje rozšiřující funkčnost k třídě <see cref="System.Web.HttpResponse"/>.
/// </summary>
public static class HttpResponseExt
{
	/// <summary>
	/// Provede redirect pomocí HTTP status kódu 301 - Moved Permanently.
	/// Klasický <see cref="System.Web.HttpResponse.Redirect(string)"/> provádí redirect přes 302 - Found (Object Moved).
	/// </summary>
	/// <remarks>
	/// Zatímco klasický <see cref="System.Web.HttpResponse.Redirect(string)"/> provádí redirect přes HTTP status kód 302,
	/// což je "temporarily moved", redirect přes "301 - Moved Permanently" říká klientovi, že URL požadované stránky
	/// se definitivně změnilo na novou adresu.<br/>
	/// Klient by měl teoreticky reagovat úpravou bookmarku, ale žádný to nedělá. Smysl to má však pro indexovací roboty
	/// vyhledávačů, které se tím údajně docela řídí.<br/>
	/// POZOR: Na rozdíl od <see cref="System.Web.HttpResponse.Redirect(string)"/> nekontroluje, jestli už nebyly odeslány klientovi hlavičky.
	/// </remarks>
	/// <param name="url">Cílová adresa.</param>
	/// <param name="endResponse">Indikuje, zdali má skončit zpracování vykonávání stránky.</param>
	public static void MovedPermanently(string url, bool endResponse)
	{
		HttpResponse response = HttpContext.Current?.Response;
		Contract.Requires<InvalidOperationException>(response != null, "HttpContext.Current.Response unavailable.");

		Contract.Requires<ArgumentNullException>(url != null, nameof(url));
		Contract.Requires<ArgumentException>(!url.Contains("\n"), "Cannot redirect to newline");

		url = response.ApplyAppPathModifier(url);
		url = HttpUtilityExt.UrlEncodePathWithQueryString(url);

		response.Clear();
		response.StatusCode = 301;
		response.StatusDescription = "Moved Permanently";
		response.AddHeader("Location", url);
		response.Write("<html><head><title>Moved Permanently</title></head>\r\n");
		response.Write("<body><h2>301 Moved Permanently</h2>\r\n");
		response.Write("<p>Requested page permanently moved to <a href=\"" + HttpUtility.HtmlEncode(url) + "\">here</a>.</p>\r\n");
		response.Write("</body></html>\r\n");

		if (endResponse)
		{
			response.End();
		}
	}

	/// <summary>
	/// Provede redirect pomocí HTTP status kódu 301 - Moved Permanently a ukončí zpracování stránky.
	/// Klasický <see cref="System.Web.HttpResponse.Redirect(string)"/> provádí redirect přes 302 - Found (Object Moved).
	/// </summary>
	/// <remarks>
	/// Zatímco klasický <see cref="System.Web.HttpResponse.Redirect(string)"/> provádí redirect přes HTTP status kód 302,
	/// což je "temporarily moved", redirect přes "301 - Moved Permanently" říká klientovi, že URL požadované stránky
	/// se definitivně změnilo na novou adresu.<br/>
	/// Klient by měl teoreticky reagovat úpravou bookmarku, ale žádný to nedělá. Smysl to má však pro indexovací roboty
	/// vyhledávačů, které se tím údajně docela řídí.
	/// </remarks>
	/// <param name="url">Cílová adresa.</param>
	public static void MovedPermanently(string url)
	{
		HttpResponseExt.MovedPermanently(url, true);
	}

	/// <summary>
	/// Odešle klientovi odezvu se status kódem 410 - Gone, tj. "stránka byla zrušena bez náhrady".
	/// </summary>
	/// <param name="endResponse">Indikuje, zdali má skončit zpracování vykonávání stránky.</param>
	public static void Gone(bool endResponse)
	{
		HttpResponse response = HttpContext.Current?.Response;
		Contract.Requires<InvalidOperationException>(response != null, "HttpContext.Current.Response unavailable.");

		response.Clear();
		response.StatusCode = 410;
		response.StatusDescription = "Gone";
		response.Write("<html><head><title>Gone</title></head>\r\n");
		response.Write("<body><h2>410 Gone</h2>\r\n");
		response.Write("<p>Requested page was permanently discontinued. Please remove all links here.</p>\r\n");
		response.Write("</body></html>\r\n");

		if (endResponse)
		{
			response.End();
		}
	}

	/// <summary>
	/// Odešle klientovi odezvu se status kódem 410 - Gone, tj. "stránka byla zrušena bez náhrady"
	/// a ukončí zpracování stránky.
	/// </summary>
	public static void Gone()
	{
		HttpResponseExt.Gone(true);
	}
}
