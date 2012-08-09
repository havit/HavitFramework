using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.PayMuzo
{
	/// <summary>
	/// Abstraktní předek pro zpracování odezvy webových služeb PayMUZO.
	/// </summary>
	public abstract class PayMuzoWebServiceResponse : PayMuzoResponse
	{
		#region Ok, RequestID
		/// <summary>
		/// OK pole z odpovědi web-service.
		/// </summary>
		public bool Ok
		{
			get { return _ok; }
			protected set { _ok = value; }
		}
		private bool _ok;

		/// <summary>
		/// Pole requestId z odpovědi web-service.
		/// </summary>
		public long RequestId
		{
			get { return _requestId; }
			protected set { _requestId = value; }
		}
		private long _requestId;
		#endregion

		#region Costructor
		/// <summary>
		/// Inicializuje novou instanci třídy <see cref="PayMuzoWebServiceResponse"/> na základě odpovědi z WebService.
		/// </summary>
		/// <param name="response">odpověď z WebService</param>
		public PayMuzoWebServiceResponse(Havit.PayMuzo.WebServiceProxies.Response response)
		{
			if (response == null)
			{
				throw new ArgumentNullException("response");
			}

			ParseResponse(response);
			this.NormalizedRawData = CreateNormalizedData(response);
		}
		#endregion

		#region ParseResponse
		/// <summary>
		/// Vytahá z web-servicové response data do properties.
		/// </summary>
		/// <param name="response">odpověď z WebService</param>
		public virtual void ParseResponse(Havit.PayMuzo.WebServiceProxies.Response response)
		{
			this.Digest = response.digest;
			this.PrimaryReturnCode = PayMuzoPrimaryReturnCode.FindByValue(response.primaryReturnCode);
			this.SecondaryReturnCode = PayMuzoSecondaryReturnCode.FindByValue(response.secondaryReturnCode);
			this.Ok = response.ok;
			this.RequestId = response.requestId;
		}
		#endregion

		#region CreateNormalizedData
		/// <summary>
		/// Vytvoří normalizovaná data pro ověření podpisu.
		/// </summary>
		/// <param name="response">odpověď z WebService</param>
		public abstract PayMuzoRequestData CreateNormalizedData(Havit.PayMuzo.WebServiceProxies.Response response);
		#endregion
	}
}
