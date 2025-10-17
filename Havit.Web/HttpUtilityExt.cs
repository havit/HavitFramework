using System.Text;
using System.Web;
using System.Globalization;
using System.Resources;
using Havit.Diagnostics.Contracts;

namespace Havit.Web;

/// <summary>
/// Poskytuje další pomocné metody pro kódování a dekódování textu pro použití na webu.
/// </summary>
public static partial class HttpUtilityExt
{
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
		HttpRequest request = HttpContext.Current?.Request;
		Contract.Requires<InvalidOperationException>(request != null, "HttpContext.Current.Request unavailable.");

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
			int length = resourceOdkaz.IndexOf(',');
			if (length == -1)
			{
				throw new InvalidOperationException("Resource odkaz není platný");
			}
			var resourceClassKey = resourceOdkaz.Substring(0, length);
			var resourceKey = resourceOdkaz.Substring(length + 1);
			string defaultPropertyValue = null;
			int index = resourceKey.IndexOf(',');
			if (index != -1)
			{
				defaultPropertyValue = resourceKey.Substring(index + 1).Trim(); // default value
				resourceKey = resourceKey.Substring(0, index);
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
}
