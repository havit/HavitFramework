using System;
using System.Diagnostics.Contracts;
using Newtonsoft.Json;

namespace Havit.GoPay.DataObjects
{
	/// <summary>
	/// Bázová tøída reprezentující požadavek pro GoPay API
	/// </summary>
	public abstract class GoPayRequestBase
	{
		/// <summary>
		/// Access token pro ovìøení autorizace požadavku
		/// </summary>
		[JsonIgnore]
		public string AccessToken { get; }

		/// <summary>
		/// Nastavení access tokenu pro ovìøení autorizace požadavku
		/// </summary>
		/// <param name="accessToken">Access token pro ovìøení autorizace požadavku</param>
		protected GoPayRequestBase(string accessToken)
		{
			Contract.Requires(!String.IsNullOrEmpty(accessToken));
			AccessToken = accessToken;
		}
	}
}