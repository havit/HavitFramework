using System.Buffers;
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

		// Mezivýsledky (bajty textu a zakódované bajty) jsou jen dočasné, půjčíme si pro ně buffery z ArrayPool.
		byte[] bytesBuffer = ArrayPool<byte>.Shared.Rent(e.GetMaxByteCount(str.Length));
		try
		{
			int bytesCount = e.GetBytes(str, 0, str.Length, bytesBuffer, 0);
			int nonAsciiBytesCount = GetNonAsciiBytesCount(bytesBuffer, bytesCount);
			if (nonAsciiBytesCount == 0)
			{
				return Encoding.ASCII.GetString(bytesBuffer, 0, bytesCount);
			}

			byte[] encodedBytesBuffer = ArrayPool<byte>.Shared.Rent(bytesCount + (nonAsciiBytesCount * 2));
			try
			{
				int encodedBytesCount = UrlEncodeBytesToBytesNonAscii(bytesBuffer, bytesCount, encodedBytesBuffer);
				return Encoding.ASCII.GetString(encodedBytesBuffer, 0, encodedBytesCount);
			}
			finally
			{
				ArrayPool<byte>.Shared.Return(encodedBytesBuffer);
			}
		}
		finally
		{
			ArrayPool<byte>.Shared.Return(bytesBuffer);
		}
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
		int nonAsciiBytesCount = GetNonAsciiBytesCount(bytes, count);
		if (nonAsciiBytesCount == 0)
		{
			return bytes;
		}
		byte[] result = new byte[count + (nonAsciiBytesCount * 2)];
		UrlEncodeBytesToBytesNonAscii(bytes, count, result);
		return result;
	}

	/// <summary>
	/// Vrátí počet non-ASCII bajtů v prvních count bajtech pole.
	/// </summary>
	private static int GetNonAsciiBytesCount(byte[] bytes, int count)
	{
		int nonAsciiBytesCount = 0;
		for (int i = 0; i < count; i++)
		{
			if ((bytes[i] & 0x80) != 0)
			{
				nonAsciiBytesCount++;
			}
		}
		return nonAsciiBytesCount;
	}

	/// <summary>
	/// Zapíše do pole destination prvních count bajtů z pole bytes, non-ASCII bajty zakóduje (%XX).
	/// Pole destination musí mít alespoň count + 2 * (počet non-ASCII bajtů) prvků.
	/// </summary>
	/// <returns>Počet zapsaných bajtů.</returns>
	private static int UrlEncodeBytesToBytesNonAscii(byte[] bytes, int count, byte[] destination)
	{
		int destinationIndex = 0;
		for (int i = 0; i < count; i++)
		{
			byte b = bytes[i];
			if ((b & 0x80) == 0)
			{
				destination[destinationIndex++] = b;
			}
			else
			{
				destination[destinationIndex++] = 0x25;
				destination[destinationIndex++] = (byte)StringExt.IntToHex((b >> 4) & 15);
				destination[destinationIndex++] = (byte)StringExt.IntToHex(b & 15);
			}
		}
		return destinationIndex;
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
