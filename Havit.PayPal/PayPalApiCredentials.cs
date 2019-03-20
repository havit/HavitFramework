using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.PayPal
{
	/// <summary>
	/// Třída reprezentující PayPal API credentials.
	/// </summary>
	public class PayPalApiCredentials
	{
		/// <summary>
		/// Url pro volání PayPal API.
		/// </summary>
		public string ApiEndpointUrl { get; protected set; }

		/// <summary>
		/// Username k PayPal API.
		/// </summary>
		public string ApiUsername { get; protected set; }

		/// <summary>
		/// Heslo k PayPal API.
		/// </summary>
		public string ApiPassword { get; protected set; }
		
		/// <summary>
		/// Signature k PayPal API.
		/// </summary>
		public string ApiSignature { get; protected set; }
		
		/// <summary>
		/// Verze PayPal API (vrací aktuální verzi API pro použití v Havit.PayPal).
		/// </summary>
		public string ApiVersion { get { return "63.0"; } }

		/// <summary>
		/// Vytvoří instanci třídy PayPalApiCredentials.		
		/// </summary>
		/// <param name="apiEndpointUrl">Url pro volání PayPal API</param>
		/// <param name="apiUsername">Username k PayPal API</param>
		/// <param name="apiPassword">Heslo k PayPal API</param>
		/// <param name="apiSignature">Signature k PayPal API</param>
		public PayPalApiCredentials(string apiEndpointUrl, string apiUsername, string apiPassword, string apiSignature)
		{
			this.ApiEndpointUrl = apiEndpointUrl;
			this.ApiUsername = apiUsername;
			this.ApiPassword = apiPassword;
			this.ApiSignature = apiSignature;			
		}
	}
}
