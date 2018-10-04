using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Web;

namespace Havit.PayPal
{
	/// <summary>
	/// Třída pro připravu dotazu na PayPal.
	/// </summary>
	public class PayPalRequestData : NameValueCollection
	{
		#region GetQueryString
		/// <summary>
		/// Vrátí QueryString (NVP řetezec všech name/value hodnot z hash tabulky) bez Url.
		/// </summary>
		/// <param name="credentials">PayPal API credentials</param>
		public string GetQueryString(PayPalApiCredentials credentials)
		{
			if (!this.AllKeys.Contains("USER"))
			{
				this.Add("USER", credentials.ApiUsername);
			}

			if (!this.AllKeys.Contains("PWD"))
			{
				this.Add("PWD", credentials.ApiPassword);
			}

			if (!this.AllKeys.Contains("SIGNATURE"))
			{
				this.Add("SIGNATURE", credentials.ApiSignature);
			}

			if (!this.AllKeys.Contains("VERSION"))
			{
				this.Add("VERSION", credentials.ApiVersion);
			}			

			StringBuilder sb = new StringBuilder();
			bool firstPair = true;
			foreach (string kv in AllKeys)
			{
				string name = HttpUtility.UrlEncode(kv);
				string value = HttpUtility.UrlEncode(this[kv]);
				if (!firstPair)
				{
					sb.Append("&");
				}
				sb.Append(name).Append("=").Append(value);
				firstPair = false;
			}
			return sb.ToString();
		}
		#endregion
	}
}
