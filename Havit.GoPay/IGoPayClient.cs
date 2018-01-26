using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Havit.GoPay.Codebooks;
using Havit.GoPay.DataObjects;

namespace Havit.GoPay
{
	/// <summary>
	/// Rozhraní klienta pro komunikaci s GoPay API
	/// </summary>
	public interface IGoPayClient : IDisposable
	{
		/// <summary>
		/// Na základě definovaného scope vrací odpověď s tokenem pro další operace nad platbami
		/// </summary>
		/// <param name="clientId">ID klienta GoPay</param>
		/// <param name="clientSecret">Heslo klienta GoPay</param>
		/// <param name="scope">Scope operací, které lze provádět nad platbami s tokenem v odpovědi</param>
		/// <exception cref="System.ArgumentOutOfRangeException">Vyhazuje výjimku pokud je v parametru metody neznámý/neimplementovaný scope tokenu.</exception>
		GoPayResponse GetToken(string clientId, string clientSecret, GoPayPaymentScope scope);

		/// <summary>
		/// Vytvoří novou platbu
		/// </summary>
		/// <param name="request">Požadavek na platbu</param>
		/// <returns>Odpověď na vytvoření platby</returns>
		GoPayResponse CreatePayment(GoPayRequest request);

		/// <summary>
		/// Vrací existující platbu
		/// </summary>
		/// <param name="paymentId">ID platby</param>
		/// <param name="accessToken">Access token</param>
		GoPayResponse GetPayment(long paymentId, string accessToken);

		/// <summary>
		/// Umožňuje vrátit finanční prostředky platby, ať už úplně nebo částečně na základě zadané částky
		/// </summary>
		/// <param name="paymentId">ID plaby</param>
		/// <param name="amount">Vracená celá/částečná částka v haléřích</param>
		/// <param name="accessToken">Access token</param>
		GoPayResponse RefundPayment(long paymentId, long amount, string accessToken);

		/// <summary>
		/// Na základě dříve vytvořené platby s opakování ON_DEMAND vytvoří opakování platby
		/// </summary>
		/// <param name="onDemandPaymentId">ID dříve vytvořené platby s ON_DEMAND opakováním</param>
		/// <param name="request">Požadavek platby</param>
		GoPayResponse CreateRecurrentPaymentOnDemand(long onDemandPaymentId, GoPayRequest request);

		/// <summary>
		/// Stornuje opakovanou platby
		/// </summary>
		/// <param name="paymentId">ID platby</param>
		/// <param name="accessToken">Access token</param>
		GoPayResponse CancelRecurrentPayment(long paymentId, string accessToken);

		/// <summary>
		/// Stornuje předautorizovanou platbu
		/// </summary>
		/// <param name="paymentId">ID platby</param>
		/// <param name="accessToken">Access token</param>
		GoPayResponse CancelPreauthorizedPayment(long paymentId, string accessToken);

		/// <summary>
		/// Stržení finačních prostředků z účtu plátce dříve vytvořené předautorizované platby
		/// </summary>
		/// <param name="paymentId">ID platby</param>
		/// <param name="accessToken">Access token</param>
		GoPayResponse CapturePayment(long paymentId, string accessToken);

		/// <summary>
		/// Stržení finačních prostředků z účtu plátce dříve vytvořené předautorizované platby
		/// </summary>
		/// <param name="paymentId">ID platby</param>
		/// <param name="accessToken">Access token</param>
		Task<GoPayResponse> CapturePaymentAsync(long paymentId, string accessToken);

		/// <summary>
		/// Vrací všechny povolené platební metody v profilu obhcodníka u GoPay
		/// </summary>
		/// <param name="goId">Identifikátor obchodníka u GoPay tzv. GoID</param>
		/// <param name="accessToken">Access token</param>
		GoPayResponse GetAllowedPaymentMethods(long goId, string accessToken);
	}
}
