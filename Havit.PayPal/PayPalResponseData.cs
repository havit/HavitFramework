using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections.Specialized;

namespace Havit.PayPal
{
	/// <summary>
	/// Třída pro data přicházející v odpovědi z PayPal-u.
	/// </summary>
	public class PayPalResponseData : NameValueCollection
	{
		#region Constructor
		/// <summary>
		/// Contructor.
		/// </summary>
		public PayPalResponseData(string result)
		{
			Decode(result);
		}
		#endregion

		#region Decode
		/// <summary>
		/// Dekódování NVP řetezce.
		/// </summary>
		private void Decode(string nvpString)
		{
			Clear();
			foreach (string nvp in nvpString.Split('&'))
			{
				string[] tokens = nvp.Split('=');
				if (tokens.Length >= 2)
				{
					string name = HttpUtility.UrlDecode(tokens[0]);
					string value = HttpUtility.UrlDecode(tokens[1]);
					Add(name, value);
				}
			}
		}
		#endregion
	}
}
