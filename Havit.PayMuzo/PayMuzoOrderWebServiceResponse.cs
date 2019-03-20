using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.PayMuzo
{
	/// <summary>
	/// Třída pro zpracování odezvy webové služby ORDER.
	/// </summary>
	public class PayMuzoOrderWebServiceResponse : PayMuzoWebServiceResponse
	{
		/// <summary>
		/// Číslo objednávky v systému PayMUZO.
		/// </summary>
		public int OrderNumber
		{
			get { return _orderNumber; }
			protected set { _orderNumber = value; }
		}
		private int _orderNumber;

		/// <summary>
		/// Gets a value indicating whether this instance is digest URL encoded.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is digest URL encoded; otherwise, <c>false</c>.
		/// </value>
		public override bool IsDigestUrlEncoded
		{
			get { return false; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PayMuzoOrderWebServiceResponse"/> class.
		/// </summary>
		/// <param name="response">The response.</param>
		public PayMuzoOrderWebServiceResponse(Havit.PayMuzo.WebServiceProxies.OrderResponse response)
			: base(response)
		{
		}

		/// <summary>
		/// Vytahá z web-servicové response data do properties.
		/// </summary>
		/// <param name="response">odpověď z WebService</param>
		public override void ParseResponse(Havit.PayMuzo.WebServiceProxies.Response response)
		{
			base.ParseResponse(response);

			Havit.PayMuzo.WebServiceProxies.OrderResponse orderResponse = (Havit.PayMuzo.WebServiceProxies.OrderResponse)response;

			this.OrderNumber = Convert.ToInt32(orderResponse.orderNumber);
		}

		/// <summary>
		/// Vytvoří normalizovaná data pro ověření podpisu.
		/// </summary>
		/// <param name="response">odpověď z WebService</param>
		public override PayMuzoRequestData CreateNormalizedData(Havit.PayMuzo.WebServiceProxies.Response response)
		{
			PayMuzoRequestData data = new PayMuzoRequestData();

			data.Add("orderNumber", this.OrderNumber.ToString());
			data.Add("primaryReturnCode", this.PrimaryReturnCode.Value.ToString());
			data.Add("secondaryReturnCode", this.SecondaryReturnCode.Value.ToString());

			return data;
		}
	}
}
