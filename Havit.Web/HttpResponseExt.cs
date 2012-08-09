using System;
using System.Text;
using System.Web;
using Havit.Reflection;

namespace Havit.Web
{
	/// <summary>
	/// Obsahuje rozšiøující funkènost k tøídì <see cref="System.Web.HttpResponse"/>.
	/// </summary>
	public class HttpResponseExt
	{
		#region MovedPermanently
		/// <summary>
		/// Provede redirect pomocí HTTP status kódu 301 - Moved Permanently.
		/// Klasický <see cref="System.Web.HttpResponse.Redirect(string)"/> provádí redirect pøes 302 - Found (Object Moved).
		/// </summary>
		/// <remarks>
		/// Zatímco klasický <see cref="System.Web.HttpResponse.Redirect(string)"/> provádí redirect pøes HTTP status kód 302,
		/// což je "temporarily moved", redirect pøes "301 - Moved Permanently" øíká klientovi, že URL požadované stránky
		/// se definitivnì zmìnilo na novou adresu.<br/>
		/// Klient by mìl teoreticky reagovat úpravou bookmarku, ale žádný to nedìlá. Smysl to má však pro indexovací roboty
		/// vyhledávaèù, které se tím údajnì docela øídí.<br/>
		/// POZOR: Na rozdíl od <see cref="System.Web.HttpResponse.Redirect(string)"/> nekontroluje, jestli už nebyly odeslány klientovi hlavièky.
		/// </remarks>
		/// <param name="url">Cílová adresa.</param>
		/// <param name="endResponse">Indikuje, zda-li má skonèit zpracování vykonávání stránky.</param>
		public static void MovedPermanently(string url, bool endResponse)
		{
			if ((HttpContext.Current == null)
				|| (HttpContext.Current.Response == null))
			{
				throw new InvalidOperationException("HttpContext.Current.Response unavailable.");
			}
			HttpResponse response = HttpContext.Current.Response;

			if (url == null)
			{
				throw new ArgumentNullException("url");
			}
			if (url.IndexOf('\n') >= 0)
			{
				throw new ArgumentException("Cannot redirect to newline");
			}
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
		/// Provede redirect pomocí HTTP status kódu 301 - Moved Permanently a ukonèí zpracování stránky.
		/// Klasický <see cref="System.Web.HttpResponse.Redirect(string)"/> provádí redirect pøes 302 - Found (Object Moved).
		/// </summary>
		/// <remarks>
		/// Zatímco klasický <see cref="System.Web.HttpResponse.Redirect(string)"/> provádí redirect pøes HTTP status kód 302,
		/// což je "temporarily moved", redirect pøes "301 - Moved Permanently" øíká klientovi, že URL požadované stránky
		/// se definitivnì zmìnilo na novou adresu.<br/>
		/// Klient by mìl teoreticky reagovat úpravou bookmarku, ale žádný to nedìlá. Smysl to má však pro indexovací roboty
		/// vyhledávaèù, které se tím údajnì docela øídí.
		/// </remarks>
		/// <param name="url">Cílová adresa.</param>
		public static void MovedPermanently(string url)
		{
			HttpResponseExt.MovedPermanently(url, true);
		}
		#endregion

		#region Gone
		/// <summary>
		/// Odešle klientovi odezvu se status kódem 410 - Gone, tj. "stránka byla zrušena bez náhrady".
		/// </summary>
		/// <param name="endResponse">Indikuje, zda-li má skonèit zpracování vykonávání stránky.</param>
		public static void Gone(bool endResponse)
		{
			if ((HttpContext.Current == null)
				|| (HttpContext.Current.Response == null))
			{
				throw new InvalidOperationException("HttpContext.Current.Response unavailable.");
			}
			HttpResponse response = HttpContext.Current.Response;
			
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
		/// a ukonèí zpracování stránky.
		/// </summary>
		public static void Gone()
		{
			HttpResponseExt.Gone(true);
		}
		#endregion

		#region Private Constructor
		private HttpResponseExt() {}
		#endregion
	}
}
