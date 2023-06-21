using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Net;
using System.IO;
using System.Web;
using System.Collections.Specialized;

namespace Havit.PayPal;

/// <summary>
/// Pomocná třída implementující volání funkcí ve PayPal Express Checkout Flow.
/// </summary>
public static class PayPalHelper
{
	// HttpWebRequest Timeout specified in milliseconds 
	private const int Timeout = 10000;

	/// <summary>
	/// Vytvoří request pro volání SetExpressCheckout API (inicializace PayPal platby - basic verze bez shipping info) 
	/// </summary>
	/// <param name="amount">Částka</param>
	/// <param name="returnUrl">Url pro návrat z PayPal-u</param>
	/// <param name="cancelUrl">Url pro návrat z PayPal-u v případě kliknutí na odkaz zrušení platby</param>
	/// <param name="paymentAction">Způsub vyrovnání platby</param>
	/// <param name="currency">Měna platby</param>
	/// <param name="landingPage">Typ PayPal stránky, která se má zobrazit uživateli</param>
	/// <param name="paymentName">Název platby</param>		
	public static PayPalRequestData CreateSetExpressCheckoutRequest(
		decimal amount,
		string returnUrl,
		string cancelUrl,
		PayPalPaymentAction paymentAction,
		PayPalCurrency currency,
		PayPalLandingPage landingPage,
		string paymentName)
	{
		PayPalRequestData request = CreateSetExpressCheckoutRequest(amount, returnUrl, cancelUrl, paymentAction, currency, landingPage, true, false, paymentName, null, null, null, null, null, null, null);

		return request;			
	}

	/// <summary>
	/// Vytvoří request pro volání SetExpressCheckout API (inicializace PayPal platby)
	/// </summary>
	/// <param name="amount">Částka</param>
	/// <param name="returnUrl">Url pro návrat z PayPal-u</param>
	/// <param name="cancelUrl">Url pro návrat z PayPal-u v případě kliknutí na odkaz zrušení platby</param>
	/// <param name="paymentAction">Způsub vyrovnání platby</param>
	/// <param name="currency">Měna platby</param>
	/// <param name="landingPage">Typ PayPal stránky, která se má zobrazit uživateli</param>
	/// <param name="suppressShippingInformation">Potlačení zobrazení shipping address nakupujícího na straně PayPal-u</param>
	/// <param name="allowNote">Povoluje poznámku na Review stránce</param>
	/// <param name="paymentName">Název platby</param>
	/// <param name="shipToName">Jméno doručitele</param>
	/// <param name="shipToStreet">Doručovací adresa - Ulice</param>
	/// <param name="shipToStreet2">Doručovací adresa - Ulice 2</param>
	/// <param name="shipToCity">Doručovací adresa - Město</param>
	/// <param name="shipToZip">Doručovací adresa - PSČ</param>
	/// <param name="shipToState">Doručovací adresa - Stát</param>
	/// <param name="shipToPayPalCountryCode">Doručovací adresa - kód země</param>
	public static PayPalRequestData CreateSetExpressCheckoutRequest(
		decimal amount,
		string returnUrl,
		string cancelUrl,
		PayPalPaymentAction paymentAction,
		PayPalCurrency currency,
		PayPalLandingPage landingPage,
		bool suppressShippingInformation,
		bool allowNote,
		string paymentName,
		string shipToName,
		string shipToStreet,
		string shipToStreet2,
		string shipToCity,
		string shipToZip,
		string shipToState,
		string shipToPayPalCountryCode)
	{
		PayPalRequestData request = new PayPalRequestData();

		request.Add("METHOD", "SetExpressCheckout");
		request.Add("RETURNURL", returnUrl);
		request.Add("CANCELURL", cancelUrl);

		if (currency == null)
		{
			throw new ArgumentNullException("currency");
		}
		if (amount <= 0)
		{
			throw new ArgumentOutOfRangeException("amount", "amount musí být kladné číslo");
		}			

		request.Add("PAYMENTREQUEST_0_AMT", amount.ToString(CultureInfo.InvariantCulture));
		request.Add("PAYMENTREQUEST_0_ITEMAMT", amount.ToString(CultureInfo.InvariantCulture));
		request.Add("L_PAYMENTREQUEST_0_AMT0", amount.ToString(CultureInfo.InvariantCulture));			
		request.Add("PAYMENTREQUEST_0_CURRENCYCODE", currency.Code);
		request.Add("PAYMENTREQUEST_0_PAYMENTACTION", PayPalPaymentActionHelper.GetPayPalPaymentActionCode(paymentAction));
		request.Add("LANDINGPAGE", PayPalLandingPageHelper.GetPayPalLandingPageCode(landingPage));

		if (suppressShippingInformation)
		{
			request.Add("NOSHIPPING", "1");
		}
		else
		{
			request.Add("NOSHIPPING", "0");
		}

		if (allowNote)
		{
			request.Add("ALLOWNOTE", "1");
		}
		else
		{
			request.Add("ALLOWNOTE", "0");
		}

		if (!String.IsNullOrEmpty(paymentName))
		{
			request.Add("L_PAYMENTREQUEST_0_NAME0", paymentName);				
		}		

		request.Add("GIFTMESSAGEENABLE", "0");
		request.Add("GIFTRECEIPTENABLE", "0");
		request.Add("GIFTWRAPENABLE", "0");

		request.Add("BUYEREMAILOPTINENABLE", "0");
		request.Add("SURVEYENABLE", "0");

		if (!String.IsNullOrEmpty(shipToName))
		{
			request.Add("PAYMENTREQUEST_0_SHIPTONAME", shipToName);
		}

		if (!String.IsNullOrEmpty(shipToStreet))
		{
			request.Add("PAYMENTREQUEST_0_SHIPTOSTREET", shipToStreet);
		}

		if (!String.IsNullOrEmpty(shipToStreet2))
		{
			request.Add("PAYMENTREQUEST_0_SHIPTOSTREET2", shipToStreet2);
		}		

		if (!String.IsNullOrEmpty(shipToCity))
		{
			request.Add("PAYMENTREQUEST_0_SHIPTOCITY", shipToCity);
		}

		if (!String.IsNullOrEmpty(shipToZip))
		{
			request.Add("PAYMENTREQUEST_0_SHIPTOZIP", shipToZip);
		}

		if (!String.IsNullOrEmpty(shipToState))
		{
			request.Add("PAYMENTREQUEST_0_SHIPTOSTATE", shipToState);
		}

		if (!String.IsNullOrEmpty(shipToPayPalCountryCode))
		{
			request.Add("PAYMENTREQUEST_0_SHIPTOCOUNTRY", shipToPayPalCountryCode);
		}	

		return request;
	}

	/// <summary>
	/// Vytvoří request pro volání GetExpressCheckoutDetails API
	/// </summary>
	/// <param name="token">Token</param>
	public static PayPalRequestData CreateGetExpressCheckoutDetailsRequest(string token)
	{
		PayPalRequestData request = new PayPalRequestData();

		request.Add("METHOD", "GetExpressCheckoutDetails");
		request.Add("TOKEN", token);

		return request;
	}

	/// <summary>
	/// Vytvoří request pro volání DoExpressCheckoutPayment API
	/// </summary>
	public static PayPalRequestData CreateDoExpressCheckoutRequest(string token, string payerID, decimal amount, PayPalPaymentAction paymentAction, PayPalCurrency currency)
	{
		PayPalRequestData request = new PayPalRequestData();

		request.Add("METHOD", "DoExpressCheckoutPayment");
		request.Add("TOKEN", token);
		request.Add("PAYERID", payerID);
		request.Add("PAYMENTREQUEST_0_PAYMENTACTION", PayPalPaymentActionHelper.GetPayPalPaymentActionCode(paymentAction));
		request.Add("PAYMENTREQUEST_0_CURRENCYCODE", currency.Code);
		request.Add("PAYMENTREQUEST_0_AMT", amount.ToString(CultureInfo.InvariantCulture));			

		return request;
	}

	/// <summary>
	/// Metoda vykoná volání PayPal API funkce a vrátí výsledek v podobě NVP data.
	/// </summary>
	/// <param name="requestData">Hodnoty pro request</param>
	/// <param name="credentials">Credentials pro PayPal API</param>
	public static NameValueCollection ExecutePayPalRequest(PayPalRequestData requestData, PayPalApiCredentials credentials)
	{			
		string postQueryString = requestData.GetQueryString(credentials);

		HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(credentials.ApiEndpointUrl);
		objRequest.Timeout = Timeout;
		objRequest.Method = "POST";
		objRequest.ContentLength = postQueryString.Length;
		PayPalResponseData responseData;
		
		using (StreamWriter myWriter = new StreamWriter(objRequest.GetRequestStream()))
		{
			myWriter.Write(postQueryString);
		}

		HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();

		string result;
		using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
		{
			result = sr.ReadToEnd();
		}

		responseData = new PayPalResponseData(result);			
		
		return responseData;
	}

	/// <summary>
	/// GetPayPalUrl
	/// </summary>
	/// <param name="payPalUrl">Url PayPal-u.</param>
	/// <param name="token">Token transakce na PayPal-u.</param>		
	public static string GetPayPalUrl(string payPalUrl, string token)
	{
		return payPalUrl + "&token=" + token;
	}
}
