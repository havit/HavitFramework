using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Havit.Diagnostics.Contracts;
using Havit.GoPay.Codebooks;
using Havit.GoPay.DataObjects;
using Newtonsoft.Json;

namespace Havit.GoPay;

/// <summary>
/// GoPay client
/// </summary>
public class GoPayClient : IGoPayClient
{
	internal const string TokenUrl = "oauth2/token";
	internal const string PaymentUrl = "payments/payment";
	internal const string PaymentUrlWithIdFormat = "payments/payment/{0}";
	internal const string RefundPaymentUrlFormat = "payments/payment/{0}/refund";
	internal const string CreateRecurrentPaymentUrlFormat = "payments/payment/{0}/create-recurrence";
	internal const string CancelRecurrentPaymentUrlFormat = "payments/payment/{0}/void-recurrence";
	internal const string CancelPreauthorizedPaymentUrlFormat = "payments/payment/{0}/void-authorization";
	internal const string CapturePaymentUrlFormat = "payments/payment/{0}/capture";
	internal const string AllowedPaymentMethodsUrlFormat = "eshops/eshop/{0}/payment-instruments";

	private readonly HttpClient httpClient;

	/// <summary>
	/// Initializes a new instance of the <see cref="GoPayClient" /> class with <see cref="HttpClient" />.
	/// </summary>
	/// <param name="httpClient">Http client</param>
	/// <exception cref="ArgumentNullException">Thrown when instance of the <see cref="HttpClient" /> (or its <see cref="Uri">BaseAddress</see>) is missing.</exception>
	public GoPayClient(HttpClient httpClient)
	{
		Contract.Requires<ArgumentNullException>(httpClient != null, nameof(httpClient));
		Contract.Requires<ArgumentException>(httpClient.BaseAddress != null, "HttpClient nemá nastavenu BaseAddress.");

		this.httpClient = httpClient;

		if (!this.httpClient.BaseAddress.AbsoluteUri.EndsWith("/"))
		{
			this.httpClient.BaseAddress = new Uri(this.httpClient.BaseAddress.AbsoluteUri + '/');
		}
	}

	/// <summary>
	/// Získání přístupového tokenu
	/// </summary>
	/// <param name="clientId">Identifikátor klienta</param>
	/// <param name="clientSecret">Tajný kód klienta</param>
	/// <param name="scope">Scope</param>
	/// <returns>GoPayResponse</returns>
	public virtual GoPayResponse GetToken(string clientId, string clientSecret, GoPayPaymentScope scope)
	{
		string stringScope;
		switch (scope)
		{
			case GoPayPaymentScope.PaymentCreate:
				stringScope = "payment-create";
				break;
			case GoPayPaymentScope.PaymentAll:
				stringScope = "payment-all";
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(scope), scope, null);
		}

		return SendPost(TokenUrl, clientId, clientSecret, new FormUrlEncodedContent(new[]
		{
			new KeyValuePair<string, string>("grant_type", "client_credentials"),
			new KeyValuePair<string, string>("scope", stringScope)
		}));
	}

	/// <summary>
	/// Vytvoření platby
	/// </summary>
	/// <param name="request">Request</param>
	/// <returns>GoPayResponse</returns>
	public GoPayResponse CreatePayment(GoPayRequest request)
	{
		string jsonRequest = JsonConvert.SerializeObject(request);
		return SendPost(PaymentUrl, request.AccessToken, new StringContent(jsonRequest, Encoding.UTF8, "application/json"));
	}

	/// <summary>
	/// Získání platby
	/// </summary>
	/// <param name="paymentId">Identifikátor platby</param>
	/// <param name="accessToken">Přístupový token</param>
	/// <returns>GoPayResponse</returns>
	public GoPayResponse GetPayment(long paymentId, string accessToken)
	{
		return SendGet(String.Format(PaymentUrlWithIdFormat, paymentId), accessToken);
	}

	/// <summary>
	/// Vrácení platby
	/// </summary>
	/// <param name="paymentId">Identifikátor platby</param>
	/// <param name="amount">částka</param>
	/// <param name="accessToken">Přístupový token</param>
	/// <returns>GoPayResponse</returns>
	public GoPayResponse RefundPayment(long paymentId, long amount, string accessToken)
	{
		return SendPost(String.Format(RefundPaymentUrlFormat, paymentId), accessToken, new FormUrlEncodedContent(new[]
		{
			new KeyValuePair<string, string>("amount", amount.ToString())
		}));
	}

	/// <summary>
	/// CreateRecurrentPaymentOnDemand
	/// </summary>
	/// <param name="onDemandPaymentId">onDemandPaymentId</param>
	/// <param name="request">request</param>
	/// <returns>GoPayResponse</returns>
	public virtual GoPayResponse CreateRecurrentPaymentOnDemand(long onDemandPaymentId, GoPayRequest request)
	{
		string jsonRequest = JsonConvert.SerializeObject(request);
		return SendPost(String.Format(CreateRecurrentPaymentUrlFormat, onDemandPaymentId), request.AccessToken, new StringContent(jsonRequest, Encoding.UTF8, "application/json"));
	}

	/// <summary>
	/// CancelRecurrentPayment
	/// </summary>
	/// <param name="paymentId">Identifikátor platby</param>
	/// <param name="accessToken">Přístupový token</param>
	/// <returns>GoPayResponse</returns>
	public GoPayResponse CancelRecurrentPayment(long paymentId, string accessToken)
	{
		return SendPost(String.Format(CancelRecurrentPaymentUrlFormat, paymentId), accessToken);
	}

	/// <summary>
	/// CancelPreauthorizedPayment
	/// </summary>
	/// <param name="paymentId">Identifikátor platby</param>
	/// <param name="accessToken">Přístupový token</param>
	/// <returns>GoPayResponse</returns>
	public GoPayResponse CancelPreauthorizedPayment(long paymentId, string accessToken)
	{
		return SendPost(String.Format(CancelPreauthorizedPaymentUrlFormat, paymentId), accessToken);
	}

	/// <summary>
	/// CapturePayment
	/// </summary>
	/// <param name="paymentId">Identifikátor platby</param>
	/// <param name="accessToken">Přístupový token</param>
	/// <returns>GoPayResponse</returns>
	public GoPayResponse CapturePayment(long paymentId, string accessToken)
	{
		return SendPost(String.Format(CapturePaymentUrlFormat, paymentId), accessToken);
	}

	/// <summary>
	/// CapturePaymentAsync
	/// </summary>
	/// <param name="paymentId">Identifikátor platby</param>
	/// <param name="accessToken">Přístupový token</param>
	/// <returns>GoPayResponse</returns>
	public async Task<GoPayResponse> CapturePaymentAsync(long paymentId, string accessToken)
	{
		return await SendPostAsync(String.Format(CapturePaymentUrlFormat, paymentId), accessToken).ConfigureAwait(false);
	}

	/// <summary>
	/// GetAllowedPaymentMethods
	/// </summary>
	/// <param name="goId">goId</param>
	/// <param name="accessToken">Přístupový token</param>
	/// <returns>GoPayResponse</returns>
	public GoPayResponse GetAllowedPaymentMethods(long goId, string accessToken)
	{
		return SendGet(String.Format(AllowedPaymentMethodsUrlFormat, goId), accessToken);
	}

	private GoPayResponse SendGet(string apiPartialUrl, string accessToken)
	{
		httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
		return SendGetInternal(apiPartialUrl);
	}

	private GoPayResponse SendPost(string apiPartialUrl, string clientId, string clientSecret, HttpContent content = null)
	{
		string authorizationHeaderValue = Convert.ToBase64String(Encoding.ASCII.GetBytes(clientId + ":" + clientSecret));
		httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authorizationHeaderValue);
		return SendPostInternal(apiPartialUrl, content);
	}

	private GoPayResponse SendPost(string apiPartialUrl, string accessToken, HttpContent content = null)
	{
		httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
		return SendPostInternal(apiPartialUrl, content);
	}

	private async Task<GoPayResponse> SendPostAsync(string apiPartialUrl, string accessToken, HttpContent content = null)
	{
		httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
		return await SendPostInternalAsync(apiPartialUrl, content).ConfigureAwait(false);
	}

	private GoPayResponse SendGetInternal(string apiPartialUrl)
	{
		HttpResponseMessage message = httpClient.GetAsync(apiPartialUrl).Result;
		GoPayResponse response = ProcessResponse(message);
		return response;
	}

	private GoPayResponse SendPostInternal(string apiPartialUrl, HttpContent content = null)
	{
		HttpResponseMessage message = httpClient.PostAsync(apiPartialUrl, content).GetAwaiter().GetResult();
		GoPayResponse response = ProcessResponse(message);
		return response;
	}

	private async Task<GoPayResponse> SendPostInternalAsync(string apiPartialUrl, HttpContent content = null)
	{
		HttpResponseMessage message = await httpClient.PostAsync(apiPartialUrl, content).ConfigureAwait(false);
		GoPayResponse response = await ProcessResponseAsync(message).ConfigureAwait(false);
		return response;
	}

	private static GoPayResponse ProcessResponse(HttpResponseMessage result)
	{
		GoPayResponse response;
		using (Stream responseStream = result.Content.ReadAsStreamAsync().GetAwaiter().GetResult())
		using (TextReader textReader = new StreamReader(responseStream))
		{
			string resultStringValue = textReader.ReadToEnd();

			try
			{
				response = JsonConvert.DeserializeObject<GoPayResponse>(resultStringValue);
			}
			catch (Exception ex)
			{
				throw new GoPayResponseException($"Unable to process GoPay response: {resultStringValue}", ex);
			}

		}
		return response;
	}

	private static async Task<GoPayResponse> ProcessResponseAsync(HttpResponseMessage result)
	{
		GoPayResponse response;
		using (Stream responseStream = await result.Content.ReadAsStreamAsync().ConfigureAwait(false))
		using (TextReader textReader = new StreamReader(responseStream))
		{
			string resultStringValue = textReader.ReadToEnd();

			try
			{
				response = JsonConvert.DeserializeObject<GoPayResponse>(resultStringValue);
			}
			catch (Exception ex)
			{
				throw new GoPayResponseException($"Unable to process GoPay response: {resultStringValue}", ex);
			}

		}
		return response;
	}

	/// <summary>
	/// Dispose
	/// </summary>
	public void Dispose()
	{
		httpClient?.Dispose();
	}
}