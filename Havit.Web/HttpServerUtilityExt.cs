using Havit.Diagnostics.Contracts;
using System.Collections;
using System.Web;
using System.Web.Caching;

namespace Havit.Web;

/// <summary>
/// Poskytuje další pomocné metody pro ovládání webového serveru.
/// </summary>
public static class HttpServerUtilityExt
{
	/// <summary>
	/// Vyčistí cache aplikace.
	/// </summary>
	public static void ClearCache()
	{
		Cache cache = HttpRuntime.Cache;
		foreach (DictionaryEntry de in cache)
		{
			cache.Remove(de.Key.ToString());
		}
	}

	/// <summary>
	/// Converts a URL into one that is usable on the requesting client.
	/// </summary>
	/// <remarks>Converts ~ to the requesting application path.  Mimics the behavior of the 
	/// <b>Control.ResolveUrl()</b> method, which is often used by control developers.</remarks>
	/// <param name="appPath">The application path.</param>
	/// <param name="url">The URL, which might contain ~.</param>
	/// <returns>A resolved URL.  If the input parameter <b>url</b> contains ~, it is replaced with the
	/// value of the <b>appPath</b> parameter.</returns>
	public static string ResolveUrl(string appPath, string url)
	{
		if (url.Length == 0 || url[0] != '~')
		{
			return url;     // there is no ~ in the first character position, just return the url
		}
		else
		{
			if (url.Length == 1)
			{
				return appPath;  // there is just the ~ in the URL, return the appPath
			}
			if (url[1] == '/' || url[1] == '\\')
			{
				// url looks like ~/ or ~\
				if (appPath.Length > 1)
				{
					if (appPath.EndsWith("/"))
					{
						return appPath + url.Substring(2);
					}
					else
					{
						return appPath + "/" + url.Substring(2);
					}
				}
				else
				{
					return "/" + url.Substring(2);
				}
			}
			else
			{
				// url looks like ~something
				if (appPath.Length > 1)
				{
					return appPath + "/" + url.Substring(1);
				}
				else
				{
					return appPath + url.Substring(1);
				}
			}
		}
	}

	/// <summary>
	/// Converts a URL into one that is usable on the requesting client.
	/// </summary>
	/// <remarks>Converts ~ to the requesting application path.  Mimics the behavior of the 
	/// <b>Control.ResolveUrl()</b> method, which is often used by control developers.</remarks>
	/// <param name="url">The URL, which might contain ~.</param>
	/// <returns>A resolved URL.  If the input parameter <b>url</b> contains ~, it is replaced with the
	/// value of the <see cref="System.Web.HttpRequest.ApplicationPath"/> parameter
	/// of <see cref="System.Web.HttpContext.Current"/>.</returns>
	public static string ResolveUrl(string url)
	{
		HttpRequest request = HttpContext.Current?.Request;
		Contract.Requires<InvalidOperationException>(request != null, "HttpContext.Current.Request unavailable.");

		return ResolveUrl(request.ApplicationPath, url);
	}
}
