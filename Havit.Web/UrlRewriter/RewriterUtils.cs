using System;
using System.Web;

namespace Havit.Web.UrlRewriter
{
	/// <summary>
	/// Provides utility helper methods for the rewriting HttpModule and HttpHandler.
	/// </summary>
	/// <remarks>This class is marked as internal, meaning only classes in the same assembly will be
	/// able to access its methods.</remarks>
	[Obsolete("Upgrade to ASP.NET Routing.")]
	internal static class RewriterUtils
	{
		#region RewriteUrl
		/// <summary>
		/// Rewrite's a URL using <b>HttpContext.RewriteUrl()</b>.
		/// </summary>
		/// <param name="context">The HttpContext object to rewrite the URL to.</param>
		/// <param name="sendToUrl">The URL to rewrite to.</param>
		internal static void RewriteUrl(HttpContext context, string sendToUrl)
		{
			string x;
			string y;

			RewriteUrl(context, sendToUrl, out x, out y);
		}

		/// <summary>
		/// Rewrite's a URL using <b>HttpContext.RewriteUrl()</b>.
		/// </summary>
		/// <param name="context">The HttpContext object to rewrite the URL to.</param>
		/// <param name="sendToUrl">The URL to rewrite to.</param>
		/// <param name="sendToUrlLessQString">Returns the value of sendToUrl stripped of the querystring.</param>
		/// <param name="filePath">Returns the physical file path to the requested page.</param>
		internal static void RewriteUrl(HttpContext context, string sendToUrl, out string sendToUrlLessQString, out string filePath)
		{
			// see if we need to add any extra querystring information
			if (context.Request.QueryString.Count > 0)
			{
				if (sendToUrl.IndexOf('?') != -1)
				{
					sendToUrl += "&" + context.Request.QueryString.ToString();
				}
				else
				{
					sendToUrl += "?" + context.Request.QueryString.ToString();
				}
			}

			// first strip the querystring, if any
			string queryString = String.Empty;
			sendToUrlLessQString = sendToUrl;
			if (sendToUrl.IndexOf('?') >= 0)
			{
				sendToUrlLessQString = sendToUrl.Substring(0, sendToUrl.IndexOf('?'));
				queryString = sendToUrl.Substring(sendToUrl.IndexOf('?') + 1);
			}

			// grab the file's physical path
			filePath = string.Empty;
			filePath = context.Server.MapPath(sendToUrlLessQString);

			// rewrite the path...
			context.RewritePath(sendToUrlLessQString, String.Empty, queryString);

			// NOTE!  The above RewritePath() overload is only supported in the .NET Framework 1.1
			// If you are using .NET Framework 1.0, use the below form instead:
			// context.RewritePath(sendToUrl);
		}
		#endregion
	}
}
