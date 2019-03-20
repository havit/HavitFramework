using System;
using System.Collections.Generic;
using System.Text;
using Havit.PayMuzo.WebServiceProxies;
using System.Security.Cryptography.X509Certificates;

namespace Havit.PayMuzo
{
	/// <summary>
	/// Klient webových služeb PayMuzo. Pozor, není thread-safe.
	/// </summary>
	public class PayMuzoWebServiceClient
	{
		private readonly handleWSService webServiceProxy;
		private readonly X509Certificate2 merchantCertificate;
		private readonly X509Certificate2 payMuzoGateCertificate;
		private readonly ulong merchantNumber;
		private readonly string serviceUrl;

		/// <summary>
		/// Response poslední provedené operace. Pozor, není thread-safe!
		/// </summary>
		public PayMuzoResponse LastResponse
		{
			get { return _lastResponse; }
			protected set { _lastResponse = value; }
		}
		private PayMuzoResponse _lastResponse;

		/// <summary>
		/// Raw-Response poslední provedené operace. Pozor, není thread-safe!
		/// </summary>
		public Havit.PayMuzo.WebServiceProxies.Response LastRawResponse
		{
			get { return _lastRawResponse; }
			protected set { _lastRawResponse = value; }
		}
		private Havit.PayMuzo.WebServiceProxies.Response _lastRawResponse;

		/// <summary>
		/// Vytvoří instanci WebClienta a nastaví základní parametry komunikace.
		/// </summary>
		/// <param name="serviceUrl">URL adresa webové služby</param>
		/// <param name="payMuzoGateCertificate">certifikát s veřejným klíčem PayMUZO brány</param>
		/// <param name="merchantNumber">číslo obchodníka</param>
		/// <param name="merchantCertificate">certifikát s privátním i veřejným klíčem obchodníka</param>
		public PayMuzoWebServiceClient(
			string serviceUrl,
			X509Certificate2 payMuzoGateCertificate,
			ulong merchantNumber,
			X509Certificate2 merchantCertificate)
		{
			if (String.IsNullOrEmpty(serviceUrl))
			{
				throw new ArgumentException("Argument nesmí být null ani String.Empty.", "serviceUrl");
			}
			if (payMuzoGateCertificate == null)
			{
				throw new ArgumentNullException("payMuzoGateCertificate");
			}
			if (merchantNumber < 0ul)
			{
				throw new ArgumentException("Argument musí být kladný", "merchantNumber");
			}
			if (merchantCertificate == null)
			{
				throw new ArgumentNullException("merchantCertificate");
			}

			this.serviceUrl = serviceUrl;

			webServiceProxy = new handleWSService();
			webServiceProxy.Url = this.serviceUrl;

			this.merchantCertificate = merchantCertificate;
			this.merchantNumber = merchantNumber;
			this.payMuzoGateCertificate = payMuzoGateCertificate;
		}

		/// <summary>
		/// Queries the state of the order.
		/// </summary>
		/// <exception cref="PayMuzoInvalidDigestException">pokud se nepodaří ověřit podpis odpovědi</exception>
		/// <exception cref="PayMuzoResponseException">pokud odpověď není OK</exception>
		/// <param name="orderNumber">The order number.</param>
		/// <returns>orderState</returns>
		public PayMuzoOrderState QueryOrderState(int orderNumber)
		{
			if (orderNumber < 0)
			{
				throw new ArgumentException("Argument musí být nezáporné číslo", "orderNumber");
			}
			
			PayMuzoRequestData request = new PayMuzoRequestData();
			request.Add("MERCHANTNUMBER", merchantNumber.ToString());
			request.Add("ORDERNUMBER", orderNumber.ToString());

			string digest = PayMuzoHelper.CreateDigest(request.GetPipedRawData(), merchantCertificate, false);

			Havit.PayMuzo.WebServiceProxies.OrderStateResponse rawResponse = webServiceProxy.queryOrderState(merchantNumber.ToString(), orderNumber.ToString(), digest);
			this.LastRawResponse = rawResponse;

			PayMuzoOrderStateWebServiceResponse response = new PayMuzoOrderStateWebServiceResponse(rawResponse);
			this.LastResponse = response;

			if (!response.VerifyDigest(this.payMuzoGateCertificate))
			{
				throw new PayMuzoInvalidDigestException("Invalid response digest.");
			}

			if (!response.Ok)
			{
				throw new PayMuzoResponseException(response);
			}

			return response.OrderState;
		}
	}
}
