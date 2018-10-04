using System;
using System.Text;
using System.Web;
using Havit;
using System.Globalization;
using System.Configuration;
using System.Resources;

namespace Havit.Web
{
	/// <summary>
	/// Poskytuje další pomocné metody pro kódování a dekódování textu pro použití na webu.
	/// </summary>
	public static class HttpUtilityExt
	{
		#region HtmlEncode
		/// <summary>
		/// Zkonvertuje string do HTML-encoded podoby.
		/// Oproti standardnímu <see cref="System.Web.HttpUtility.HtmlEncode(string)"/> může encodovat všechny non-ASCII znaky
		/// a hlavně umožňuje pomocí options řídit požadovanou výslednou podobu. Lze například použít rozšířenou sadu HTML-entit,
		/// popřípadě úplně vyloučit převod ne-ASCII znaků na podobu &amp;#1234;.
		/// </summary>
		/// <param name="unicodeText">převáděný string v Unicode</param>
		/// <param name="options">options volby konverze</param>
		/// <returns>HTML-encoded string dle options</returns>
		public static string HtmlEncode(string unicodeText, HtmlEncodeOptions options)
		{
			int unicodeValue;
			StringBuilder result = new StringBuilder();

			bool opIgnoreNonASCIICharacters = ((options & HtmlEncodeOptions.IgnoreNonASCIICharacters) == HtmlEncodeOptions.IgnoreNonASCIICharacters);
			bool opExtendedHtmlEntities = ((options & HtmlEncodeOptions.ExtendedHtmlEntities) == HtmlEncodeOptions.ExtendedHtmlEntities);
			bool opXmlApostropheEntity = ((options & HtmlEncodeOptions.XmlApostropheEntity) == HtmlEncodeOptions.XmlApostropheEntity);

			int length = unicodeText.Length;
			for (int i = 0; i < length; i++)
			{
				unicodeValue = unicodeText[i];
				switch (unicodeValue) 
				{
					case '&':
						result.Append("&amp;");
						break;
					case '<':
						result.Append("&lt;");
						break;
					case '>':
						result.Append("&gt;");
						break;
					case '"':
						result.Append("&quot;");
						break;
					case '\'':
						if (opXmlApostropheEntity)
						{
							result.Append("&apos;");
							break;
						}
						else
						{
							goto default;
						}
					case 0xA0: // no-break space
						if (opExtendedHtmlEntities)
						{
							result.Append("&nbsp;");
							break;
						}
						else
						{
						goto default;
						}
					case '€':
						if (opExtendedHtmlEntities)
						{
							result.Append("&euro;");
							break;
						}
						else
						{
							goto default;
						}
					case '©':
						if (opExtendedHtmlEntities)
						{
							result.Append("&copy;");
							break;
						}
						else
						{
							goto default;
						}
					case '®':
						if (opExtendedHtmlEntities)
						{
							result.Append("&reg;");
							break;
						}
						else
						{
							goto default;
						}
					case '™': // trade-mark
						if (opExtendedHtmlEntities)
						{
							result.Append("&trade;");
							break;
						}
						else
						{
							goto default;
						}
					default:
						if (((unicodeText[i] >= ' ') && (unicodeText[i] <= 0x007E)) 
							|| opIgnoreNonASCIICharacters)
						{ 
							result.Append(unicodeText[i]);
						} 
						else 
						{
							result.Append("&#");
							result.Append(unicodeValue.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
							result.Append(";");
						}
						break;
				}
			}
			return result.ToString();
		}

		/// <summary>
		/// Zkonvertuje string do HTML-encoded podoby s použitím výchozích options.
		/// Oproti standardnímu <see cref="System.Web.HttpUtility.HtmlEncode(string)"/> encoduje všechny non-ASCII znaky.
		/// </summary>
		/// <remarks>
		/// Pro podrobné řízení voleb konverze je nutno použít overload s options, takto je použito <see cref="HtmlEncodeOptions.None"/>,
		/// tj. pouze pět standardních XML entit (&amp;gt;; &amp;lt;, &amp;amp;, &amp;quot;, &amp;apos;)
		/// </remarks>
		/// <param name="unicodeText">převáděný string v Unicode</param>
		/// <returns>HTML-encoded string</returns>
		public static string HtmlEncode(string unicodeText)
		{
			return HtmlEncode(unicodeText, HtmlEncodeOptions.None);
		}
		#endregion

		#region UrlEncodeSpaces
		/// <summary>
		/// Encoduje řetězec tak, že vymění mezery za %20.
		/// </summary>
		/// <remarks>
		/// Public přepis internal metody System.Web.HttpUtility.UrlEncodeSpaces.
		/// </remarks>
		/// <param name="str">Text k encodování</param>
		/// <returns>Řetězec, kde jsou mezery vyměněny za %20.</returns>
		public static string UrlEncodeSpaces(string str)
		{
			if ((str != null) && (str.IndexOf(' ') >= 0))
			{
				str = str.Replace(" ", "%20");
			}
			return str;
		}
		#endregion

		#region UrlEncodeNonAscii, UrlEncodeBytesToBytesNonAscii
		/// <summary>
		/// Encoduje všechny non-ACSII znaky v zadaném řetězci pro bezpečný přenos v URL.
		/// Lze použít na již sestavený QueryString, nezlikviduje totiž &amp;, =, atp.
		/// </summary>
		/// <remarks>
		/// Public přepis internal metody System.Web.HttpUtility.UrlEncodeNonAcsii.
		/// </remarks>
		/// <param name="str">Text k encodování.</param>
		/// <param name="e">Encoding textu</param>
		/// <returns>Text encodovaný pro použití v URL.</returns>
		public static string UrlEncodeNonAscii(string str, Encoding e)
		{
			if ((str == null) || (str.Length == 0))
			{
				return str;
			}
			if (e == null)
			{
				e = Encoding.UTF8;
			}
			byte[] buffer1 = e.GetBytes(str);
			buffer1 = HttpUtilityExt.UrlEncodeBytesToBytesNonAscii(buffer1);
			return Encoding.ASCII.GetString(buffer1);
		}

		/// <summary>
		/// Encoduje všechny non-ACSII znaky v zadaném poli bytů pro bezpečný přenos v URL.
		/// Lze použít na již sestavený QueryString, nezlikviduje totiž &amp;, =, atp.
		/// </summary>
		/// <remarks>
		/// Public přepis internal metody System.Web.HttpUtility.UrlEncodeBytesToBytesInternalNonAscii.
		/// </remarks>
		/// <param name="bytes">vstupní text</param>
		/// <returns>Text encodovaný pro použití v URL.</returns>
		public static byte[] UrlEncodeBytesToBytesNonAscii(byte[] bytes)
		{
			int count = bytes.Length;
			int num1 = 0;
			for (int num2 = 0; num2 < count; num2++)
			{
				if ((bytes[num2] & 0x80) != 0)
				{
					num1++;
				}
			}
			if (num1 == 0)
			{
				return bytes;
			}
			byte[] buffer1 = new byte[count + (num1 * 2)];
			int num3 = 0;
			for (int num4 = 0; num4 < count; num4++)
			{
				byte num5 = bytes[num4];
				if ((bytes[num4] & 0x80) == 0)
				{
					buffer1[num3++] = num5;
				}
				else
				{
					buffer1[num3++] = 0x25;
					buffer1[num3++] = (byte)StringExt.IntToHex((num5 >> 4) & 15);
					buffer1[num3++] = (byte)StringExt.IntToHex(num5 & 15);
				}
			}
			return buffer1;
		}
		#endregion

		#region UrlEncodePathWithQueryString
		/// <summary>
		/// Encoduje všechny non-ACSII znaky v zadaném poli bytů pro bezpečný přenos v URL.
		/// Lze použít na již sestavený QueryString, nezlikviduje totiž &amp;, =, atp.
		/// </summary>
		/// <remarks>
		/// Public přepis internal metody System.Web.HttpUtility.UrlEncodeBytesToBytesInternalNonAscii.
		/// </remarks>
		/// <param name="urlWithQueryString">vstupní text</param>
		/// <returns>Text encodovaný pro použití v URL.</returns>
		public static string UrlEncodePathWithQueryString(string urlWithQueryString)
		{
			if ((HttpContext.Current == null)
				|| (HttpContext.Current.Request == null))
			{
				throw new InvalidOperationException("HttpContext.Current.Request unavailable.");
			}
			HttpRequest request = HttpContext.Current.Request;

			int otaznik = urlWithQueryString.IndexOf('?');
			if (otaznik >= 0)
			{
				Encoding encoding1 = request.ContentEncoding;
				urlWithQueryString = HttpUtilityExt.UrlEncodeSpaces(HttpUtilityExt.UrlEncodeNonAscii(urlWithQueryString.Substring(0, otaznik), Encoding.UTF8)) +
					HttpUtilityExt.UrlEncodeNonAscii(urlWithQueryString.Substring(otaznik), encoding1);
				return urlWithQueryString;
			}
			urlWithQueryString = HttpUtilityExt.UrlEncodeSpaces(HttpUtilityExt.UrlEncodeNonAscii(urlWithQueryString, Encoding.UTF8));
			return urlWithQueryString;
		}
		#endregion

		#region GetResourceString
		/// <summary>
		/// Vrátí resource-řetězec (lokalizaci) resolvovanou ze standardizované podoby resource odkazu používané např. ve web.sitemap, skinech, menu, atp.
		/// </summary>
		/// <example>
		/// $resources: MyGlobalResources, MyResourceKey, My default value<br/>
		/// $resources: MyGlobalResources, MyResourceKey<br/>
		/// </example>
		/// <param name="resourceExpression">resource odkaz dle příkladů</param>
		/// <returns>resolvovaný lokalizační řetězec</returns>
		public static string GetResourceString(string resourceExpression)
		{
			if ((resourceExpression != null)
				&& (resourceExpression.Length > 10)
				&& resourceExpression.ToLower(CultureInfo.InvariantCulture).StartsWith("$resources:", StringComparison.Ordinal))
			{
				string resourceOdkaz = resourceExpression.Substring(11);
				if (resourceOdkaz.Length == 0)
				{
					throw new InvalidOperationException("Resource odkaz nesmí být prázdný.");
				}
				string resourceClassKey = null;
				string resourceKey = null;
				int length = resourceOdkaz.IndexOf(',');
				if (length == -1)
				{
					throw new InvalidOperationException("Resource odkaz není platný");
				}
				resourceClassKey = resourceOdkaz.Substring(0, length);
				resourceKey = resourceOdkaz.Substring(length + 1);
				string defaultPropertyValue = null;
				int index = resourceKey.IndexOf(',');
				if (index != -1)
				{
					defaultPropertyValue = resourceKey.Substring(index + 1).Trim(); // default value
					resourceKey = resourceKey.Substring(0, index);
				}
				else
				{
					resourceExpression = null;
				}

				try
				{
					resourceExpression = (string)HttpContext.GetGlobalResourceObject(resourceClassKey.Trim(), resourceKey.Trim());
				}
				catch (MissingManifestResourceException)
				{
					resourceExpression = defaultPropertyValue;
				}

				if (resourceExpression == null)
				{
					resourceExpression = defaultPropertyValue;
				}
			}
			return resourceExpression;
		}
		#endregion

		#region GetApplicationRootUri
		/// <summary>
		/// Vrátí Uri rootu webové aplikace vytvořené na základě aktuálního requestu!
		/// (WebSite může poslouchat pro více hostnames a nikde není řečeno, který je primární.)
		/// </summary>
		/// <returns>Uri rootu webové aplikace vytvořená na základě aktuálního requestu</returns>
		public static Uri GetApplicationRootUri()
		{
			HttpContext context = HttpContext.Current;
			if (context == null)
			{
				throw new InvalidOperationException("HttpContext.Current je null, nelze vyhodnotit GetApplicationRootUri()");
			}

			HttpRequest request = context.Request;
			if (request == null)
			{
				throw new InvalidOperationException("HttpContext.Current.Request je null, nelze vyhodnotit GetApplicationRootUri()");
			}

			UriBuilder ub = new UriBuilder(request.Url.Scheme, request.Url.Host, request.Url.Port, request.ApplicationPath);

			return ub.Uri;
		}
		#endregion
	}

	#region HtmlEncodeOptions (enum)
	/// <summary>
	/// Poskytuje množinu hodnot k nastavení voleb metody <see cref="Havit.Web.HttpUtilityExt.HtmlEncode(string, HtmlEncodeOptions)"/>
	/// </summary>
	[Flags]
	public enum HtmlEncodeOptions
	{
		/// <summary>
		/// Označuje, že nemají být nastaveny žádné options, použije se default postup.
		/// Default postup převede pouze čtyři základní entity
		/// <list type="bullet">
		///		<item>&lt; --- &amp;lt;</item>
		///		<item>&gt; --- &amp;gt;</item>
		///		<item>&amp; --- &amp;amp;</item>
		///		<item>&quot; --- &amp;quot;</item>
		/// </list>
		/// </summary>
		None = 0,

		/// <summary>
		/// Při konverzi budou ignorovány znaky mimo ASCII hodnoty, nebudou tedy tvořeny číselné entity typu &amp;#123;.
		/// </summary>
		IgnoreNonASCIICharacters = 1,

		/// <summary>
		/// Při konverzi bude použita rozšířená sada HTML-entit, které by se jinak převedly na číselné entity.
		/// Např. bude použito &amp;copy;, &amp;nbsp;, &amp;sect;, atp. 
		/// </summary>
		ExtendedHtmlEntities = 2,

		/// <summary>
		/// Při konverzi převede apostrofy na &amp;apos; entitu.
		/// POZOR! &amp;apos; není standardní HTML entita a třeba IE ji v HTML režimu nepozná!!!
		/// </summary>
		/// <remarks>
		/// V kombinaci se základním <see cref="HtmlEncodeOptions.None"/> dostaneme sadu pěti built-in XML entit:
		/// <list type="bullet">
		///		<item>&lt; --- &amp;lt;</item>
		///		<item>&gt; --- &amp;gt;</item>
		///		<item>&amp; --- &amp;amp;</item>
		///		<item>&quot; --- &amp;quot;</item>
		///		<item>&apos; --- &amp;apos;</item>
		/// </list>
		/// </remarks>
		XmlApostropheEntity = 4
	}
	#endregion
}
