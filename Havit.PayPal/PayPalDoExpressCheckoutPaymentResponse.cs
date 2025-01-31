using System.Collections.Specialized;
using System.Globalization;

namespace Havit.PayPal;

/// <summary>
/// Třída pro strong-type reprezentaci odpovědí z PayPal po volání DoExpressCheckoutPayment API.
/// https://cms.paypal.com/us/cgi-bin/?&amp;cmd=_render-content&amp;content_ID=developer/e_howto_api_nvp_r_DoExpressCheckoutPayment
/// </summary>
public class PayPalDoExpressCheckoutPaymentResponse : PayPalResponseBase
{
	// DoExpressCheckoutPayment Response Fields

	/// <summary>
	/// The text entered by the buyer on the PayPal website if the ALLOWNOTE field was set to 1 in SetExpressCheckout.
	/// </summary>
	public string Note
	{
		get { return _note; }
		protected set { _note = value; }
	}
	private string _note;

	/// <summary>
	/// Flag to indicate whether you need to redirect the customer to back to PayPal for guest checkout after successfully completing the transaction.
	/// Note: Use this field only if you are using giropay or bank transfer payment methods in Germany.
	/// </summary>
	public bool RedirectRequired
	{
		get { return _redirectRequired; }
		protected set { _redirectRequired = value; }
	}
	private bool _redirectRequired;

	/// <summary>
	/// Flag to indicate whether you need to redirect the customer to back to PayPal after completing the transaction.
	/// </summary>
	public bool SuccessPageRedirectRequested
	{
		get { return _successPageRedirectRequested; }
		protected set { _successPageRedirectRequested = value; }
	}
	private bool _successPageRedirectRequested;

	/// <summary>
	/// Identifikátor transakce.
	/// </summary>
	public string TransactionID
	{
		get { return _transactionID; }
		protected set { _transactionID = value; }
	}
	private string _transactionID;

	/// <summary>
	/// Typ transakce.
	/// </summary>
	public string TransactionType
	{
		get { return _transactionType; }
		protected set { _transactionType = value; }
	}
	private string _transactionType;

	/// <summary>
	/// Indicates whether the payment is instant or delayed. Character length and limitations: Seven single-byte characters Valid values:
	/// - none 
	/// - echeck 
	/// - instant
	/// </summary>
	public string PaymentType
	{
		get { return _paymentType; }
		protected set { _paymentType = value; }
	}
	private string _paymentType;

	/// <summary>
	/// Datum/čas platby.
	/// </summary>
	public DateTime OrderTime
	{
		get { return _orderTime; }
		protected set { _orderTime = value; }
	}
	private DateTime _orderTime;

	/// <summary>
	/// Kompletní částka transkace.
	/// </summary>
	public decimal Amount
	{
		get { return _amount; }
		protected set { _amount = value; }
	}
	private decimal _amount;

	/// <summary>
	/// Poplatky PayPalu za transakci.
	/// </summary>
	public decimal FeeAmount
	{
		get { return _feeAmount; }
		protected set { _feeAmount = value; }
	}
	private decimal _feeAmount;

	/// <summary>
	/// Zdanění PayPal transakce.
	/// </summary>
	public decimal TaxAmount
	{
		get { return _taxAmount; }
		protected set { _taxAmount = value; }
	}
	private decimal _taxAmount;

	/// <summary>
	/// Kód měny.
	/// </summary>
	public PayPalCurrency CurrencyCode
	{
		get { return _currencyCode; }
		protected set { _currencyCode = value; }
	}
	private PayPalCurrency _currencyCode;

	/// <summary>
	/// Status platby. 
	/// </summary>
	public string PaymentStatus
	{
		get { return _paymentStatus; }
		protected set { _paymentStatus = value; }
	}
	private string _paymentStatus;

	/// <summary>
	/// Vrací PaymentReason když je PaymentStatus "pending".
	/// </summary>
	public string PendingReason
	{
		get { return _pendingReason; }
		protected set { _pendingReason = value; }
	}
	private string _pendingReason;

	/// <summary>
	/// The reason for a reversal if TransactionType is reversal.
	/// </summary>
	public string ReasonCode
	{
		get { return _reasonCode; }
		protected set { _reasonCode = value; }
	}
	private string _reasonCode;

	/// <summary>
	/// The the kind of seller protection in force for the transaction, which is one of the following values:
	/// Eligible – Seller is protected by PayPal's Seller Protection Policy for Unauthorized Payments and Item Not Received
	/// PartiallyEligible – Seller is protected by PayPal's Seller Protection Policy for Item Not Received
	/// Ineligible – Seller is not protected under the Seller Protection Policy
	/// </summary>
	public string ProtectionEligibility
	{
		get { return _protectionEligibility; }
		protected set { _protectionEligibility = value; }
	}
	private string _protectionEligibility;

	/// <summary>
	/// PaymentInfo ErrorCode - není v dokumentaci k API
	/// </summary>
	public string PaymentErrorCode
	{
		get { return _paymentErrorCode; }
		protected set { _paymentErrorCode = value; }
	}
	private string _paymentErrorCode;

	/// <summary>
	/// The Yes/No option that you chose for insurance.
	/// </summary>
	public string InsuranceOptionSelected
	{
		get { return _insuranceOptionSelected; }
		protected set { _insuranceOptionSelected = value; }
	}
	private string _insuranceOptionSelected;

	/// <summary>
	/// Is true if the buyer chose the default shipping option. Character length and limitations: true or false.
	/// </summary>
	public string ShippingOptionIsDefault
	{
		get { return _shippingOptionIsDefault; }
		protected set { _shippingOptionIsDefault = value; }
	}
	private string _shippingOptionIsDefault;

	/// <summary>
	/// Initializes a new instance of the PayPalDoExpressCheckoutPaymentResponse class.
	/// </summary>
	/// <param name="rawResponseData">The response data, raw.</param>
	public PayPalDoExpressCheckoutPaymentResponse(NameValueCollection rawResponseData)
		: base(rawResponseData)
	{
	}

	/// <summary>
	/// Rozparsuje data do strong-type properties.
	/// Poznámka: Parsuje jenom základní informace, které potřebujeme.
	/// </summary>
	protected override void ParseResponseData(System.Collections.Specialized.NameValueCollection rawResponseData)
	{
		base.ParseResponseData(rawResponseData);

		if (!String.IsNullOrEmpty(rawResponseData["PAYMENTINFO_0_TRANSACTIONID"]))
		{
			this.TransactionID = rawResponseData["PAYMENTINFO_0_TRANSACTIONID"];
		}

		if (!String.IsNullOrEmpty(rawResponseData["PAYMENTINFO_0_TRANSACTIONTYPE"]))
		{
			this.TransactionType = rawResponseData["PAYMENTINFO_0_TRANSACTIONTYPE"];
		}

		if (!String.IsNullOrEmpty(rawResponseData["PAYMENTINFO_0_PAYMENTTYPE"]))
		{
			this.PaymentType = rawResponseData["PAYMENTINFO_0_PAYMENTTYPE"];
		}

		if (!String.IsNullOrEmpty(rawResponseData["PAYMENTINFO_0_ORDERTIME"]))
		{
			DateTime orderTime;
			if (DateTime.TryParse(rawResponseData["PAYMENTINFO_0_ORDERTIME"], out orderTime))
			{
				this.OrderTime = orderTime;
			}
		}

		if (!String.IsNullOrEmpty(rawResponseData["PAYMENTINFO_0_AMT"]))
		{
			decimal amount;
			if (Decimal.TryParse(rawResponseData["PAYMENTINFO_0_AMT"], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out amount))
			{
				this.Amount = amount;
			}
		}

		if (!String.IsNullOrEmpty(rawResponseData["PAYMENTINFO_0_FEEAMT"]))
		{
			decimal feeAmount;
			if (Decimal.TryParse(rawResponseData["PAYMENTINFO_0_FEEAMT"], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out feeAmount))
			{
				this.FeeAmount = feeAmount;
			}
		}

		if (!String.IsNullOrEmpty(rawResponseData["PAYMENTINFO_0_TAXAMT"]))
		{
			decimal taxAmount;
			if (Decimal.TryParse(rawResponseData["PAYMENTINFO_0_TAXAMT"], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out taxAmount))
			{
				this.TaxAmount = taxAmount;
			}
		}

		if (!String.IsNullOrEmpty(rawResponseData["PAYMENTINFO_0_CURRENCYCODE"]))
		{
			PayPalCurrency currency = PayPalCurrency.FindByCurrencyCode(rawResponseData["PAYMENTINFO_0_CURRENCYCODE"]);
			this.CurrencyCode = currency;
		}

		if (!String.IsNullOrEmpty(rawResponseData["PAYMENTINFO_0_PAYMENTSTATUS"]))
		{
			this.PaymentStatus = rawResponseData["PAYMENTINFO_0_PAYMENTSTATUS"];
		}

		if (!String.IsNullOrEmpty(rawResponseData["PAYMENTINFO_0_PENDINGREASON "]))
		{
			this.PendingReason = rawResponseData["PAYMENTINFO_0_PENDINGREASON "];
		}

		if (!String.IsNullOrEmpty(rawResponseData["PAYMENTINFO_0_REASONCODE"]))
		{
			this.ReasonCode = rawResponseData["PAYMENTINFO_0_REASONCODE"];
		}

		if (!String.IsNullOrEmpty(rawResponseData["PAYMENTINFO_0_PROTECTIONELIGIBILITY"]))
		{
			this.ProtectionEligibility = rawResponseData["PAYMENTINFO_0_PROTECTIONELIGIBILITY"];
		}

		// tohle není zdokumentováno
		if (!String.IsNullOrEmpty(rawResponseData["PAYMENTINFO_0_ERRORCODE"]))
		{
			this.PaymentErrorCode = rawResponseData["PAYMENTINFO_0_ERRORCODE"];
		}

		if (!String.IsNullOrEmpty(rawResponseData["SUCCESSPAGEREDIRECTREQUESTED"]))
		{
			bool successPageRedirectRequested;
			if (Boolean.TryParse(rawResponseData["SUCCESSPAGEREDIRECTREQUESTED"], out successPageRedirectRequested))
			{
				this.SuccessPageRedirectRequested = successPageRedirectRequested;
			}
		}

		if (!String.IsNullOrEmpty(rawResponseData["INSURANCEOPTIONSELECTED"]))
		{
			this.InsuranceOptionSelected = rawResponseData["INSURANCEOPTIONSELECTED"];
		}

		if (!String.IsNullOrEmpty(rawResponseData["SHIPPINGOPTIONISDEFAULT"]))
		{
			this.ShippingOptionIsDefault = rawResponseData["SHIPPINGOPTIONISDEFAULT"];
		}
	}
}
