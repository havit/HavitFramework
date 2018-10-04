using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace Havit.PayMuzo
{
	/// <summary>
	/// Kolekce dat pro PayMUZO request. Potomek <see cref="NameValueCollection"/> s pomocnými metodami.
	/// </summary>
	[Serializable]
	public class PayMuzoRequestData : NameValueCollection
	{
		#region GetPipedRawData
		/// <summary>
		/// Vrátí raw-data requestu jako pipe-separated-string.
		/// </summary>
		public string GetPipedRawData()
		{
			StringBuilder rawData = new StringBuilder();
			for (int i = 0; i < this.Count - 1; i++)
			{
				rawData.Append(this[i]);
				rawData.Append("|");
			}
			rawData.Append(this[this.Count - 1]); // poslední položka za pipe |
			string rawDataString = rawData.ToString();
			return rawDataString;
		}
		#endregion

		#region GetUrlWithQueryString
		/// <summary>
		/// Vrátí URL s QueryStringem s daty requestu. Lze následně použít pro GET volání, např. Response.Redirect.
		/// </summary>
		/// <param name="targetUrlWithoutQueryString">základní URL adresa bez QueryStringu</param>
		public string GetUrlWithQueryString(string targetUrlWithoutQueryString)
		{
			StringBuilder requestUrl = new StringBuilder();
			requestUrl.Append(targetUrlWithoutQueryString);
			requestUrl.Append("?");
			for (int i = 0; i < this.Count; i++)
			{
				requestUrl.Append(this.GetKey(i));
				requestUrl.Append("=");
				requestUrl.Append(this[i]);
				if (i != (this.Count - 1))
				{
					requestUrl.Append("&");
				}
			}
			return requestUrl.ToString();
		}
		#endregion
	}
}
