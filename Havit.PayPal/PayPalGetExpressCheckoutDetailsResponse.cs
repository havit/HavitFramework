using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Globalization;

namespace Havit.PayPal
{
	/// <summary>
	/// Třída pro strong-type reprezentaci odpovědí z PayPal po volání GetExpressCheckoutDetails API.
	/// https://cms.paypal.com/us/cgi-bin/?&amp;cmd=_render-content&amp;content_ID=developer/e_howto_api_nvp_r_GetExpressCheckoutDetails
	/// </summary>
	public class PayPalGetExpressCheckoutDetailsResponse : PayPalResponseBase
	{
		#region Properties (GetExpressCheckoutDetails Response Fields)
		/// <summary>
		/// A free-form field for your own use, as set by you in the Custom element of SetExpressCheckout request.
		/// </summary>
		public string Custom
		{
			get { return _custom; }
			protected set { _custom = value; }
		}
		private string _custom;

		/// <summary>
		/// Your own invoice or tracking number, as set by you in the element of the same name in SetExpressCheckout request.
		/// </summary>
		public string Invnum
		{
			get { return _invnum; }
			protected set { _invnum = value; }
		}
		private string _invnum;

		/// <summary>
		/// Payer’s contact telephone number. Character length and limitations: Field mask is XXX-XXX-XXXX (for US numbers) or +XXX XXXXXXXX (for international numbers).
		/// </summary>
		public string Phonenum
		{
			get { return _phonenum; }
			protected set { _phonenum = value; }
		}
		private string _phonenum;

		/// <summary>
		/// A discount or gift certificate offered by PayPal to the buyer. This amount will be represented by a negative amount. 
		/// If the buyer has a negative PayPal account balance, PayPal adds the negative balance to the transaction amount, which is represented as a positive value.
		/// </summary>
		public string PayPalAdjustement
		{
			get { return _payPalAdjustement; }
			protected set { _payPalAdjustement = value; }
		}
		private string _payPalAdjustement;

		/// <summary>
		/// Flag to indicate whether you need to redirect the customer to back to PayPal after completing the transaction.
		/// Note: Use this field only if you are using giropay or bank transfer payment methods in Germany.
		/// </summary>
		public bool RedirectRequired
		{
			get { return _redirectRequired; }
			protected set { _redirectRequired = value; }
		}
		private bool _redirectRequired;

		/// <summary>
		/// Returns the status of the checkout session.Possible values are:
		/// - PaymentActionNotInitiated
		/// - PaymentActionFailed
		/// - PaymentActionInProgress
		/// - PaymentCompleted
		/// If payment is completed, the transaction identification number of the resulting transaction is returned.
		/// </summary>
		public string CheckoutStatus
		{
			get { return _checkoutStatus; }
			protected set { _checkoutStatus = value; }
		}
		private string _checkoutStatus;

		/// <summary>
		/// The gift message entered by the buyer on the PayPal Review page.
		/// </summary>
		public string GiftMessage
		{
			get { return _giftMessage; }
			protected set { _giftMessage = value; }
		}
		private string _giftMessage;

		/// <summary>
		/// Returns true if the buyer requested a gift receipt on the PayPal Review page and false if the buyer did not.
		/// </summary>
		public bool GiftReceiptEnable
		{
			get { return _giftReceiptEnable; }
			protected set { _giftReceiptEnable = value; }
		}
		private bool _giftReceiptEnable;

		/// <summary>
		/// Return the gift wrap name only if the gift option on the PayPal Review page is selected by the buyer.
		/// </summary>
		public string GiftWrapName
		{
			get { return _giftWrapName; }
			protected set { _giftWrapName = value; }
		}
		private string _giftWrapName;

		/// <summary>
		/// Return the amount only if the gift option on the PayPal Review page is selected by the buyer.
		/// Limitations: Must not exceed $10,000 USD in any currency. No currency symbol. 
		/// Must have two decimal places, decimal separator must be a period (.), and the optional thousands separator must be a comma (,).
		/// </summary>
		public decimal GiftWrapAmount
		{
			get { return _giftWrapAmount; }
			protected set { _giftWrapAmount = value; }
		}
		private decimal _giftWrapAmount;

		/// <summary>
		/// The survey question on the PayPal Review page.
		/// </summary>
		public string SurveyQuestion
		{
			get { return _surveyQuestion; }
			protected set { _surveyQuestion = value; }
		}
		private string _surveyQuestion;

		/// <summary>
		/// The survey response selected by the buyer on the PayPal Review page.
		/// </summary>
		public string SurveyChoiceSelected
		{
			get { return _surveyChoiceSelected; }
			protected set { _surveyChoiceSelected = value; }
		}
		private string _surveyChoiceSelected;
		#endregion

		#region Properties (Payer Information Fields)
		/// <summary>
		/// Email address of payer. Character length and limitations: 127 single-byte characters.
		/// </summary>
		public string Email
		{
			get { return _email; }
			protected set { _email = value; }
		}
		private string _email;

		/// <summary>
		/// Unique PayPal customer account identification number. Character length and limitations:13 single-byte alphanumeric characters.
		/// </summary>
		public string PayerID
		{
			get { return _payerID; }
			protected set { _payerID = value; }
		}
		private string _payerID;

		/// <summary>
		/// Status of payer. Valid values are: verified/unverified
		/// </summary>
		public string PayerStatus
		{
			get { return _payerStatus; }
			protected set { _payerStatus = value; }
		}
		private string _payerStatus;

		/// <summary>
		/// Payer’s country of residence in the form of ISO standard 3166 two-character country codes. Character length and limitations: Two single-byte characters.
		/// </summary>
		public string CountryCode
		{
			get { return _countryCode; }
			protected set { _countryCode = value; }
		}
		private string _countryCode;

		/// <summary>
		/// Payer’s business name. Character length and limitations: 127 single-byte characters.
		/// </summary>
		public string Business
		{
			get { return _business; }
			protected set { _business = value; }
		}
		private string _business;
		#endregion

		#region Properties (Payer Name Fields)
		/// <summary>
		/// Payer’s salutation. Character length and limitations: 20 single-byte characters.
		/// </summary>
		public string Salutation
		{
			get { return _salutation; }
			protected set { _salutation = value; }
		}
		private string _salutation;

		/// <summary>
		/// Payer’s first name. Character length and limitations: 25 single-byte characters.
		/// </summary>
		public string FirstName
		{
			get { return _firstName; }
			protected set { _firstName = value; }
		}
		private string _firstName;

		/// <summary>
		/// Payer’s middle name. Character length and limitations: 25 single-byte characters.
		/// </summary>
		public string MiddleName
		{
			get { return _middleName; }
			protected set { _middleName = value; }
		}
		private string _middleName; 

		/// <summary>
		/// Payer’s last name. Character length and limitations: 25 single-byte characters.
		/// </summary>
		public string LastName
		{
			get { return _lastName; }
			protected set { _lastName = value; }
		}
		private string _lastName;

		/// <summary>
		/// Payer’s suffix. Character length and limitations: 12 single-byte characters.
		/// </summary>
		public string Suffix
		{
			get { return _suffix; }
			protected set { _suffix = value; }
		}
		private string _suffix;
		#endregion

		#region Properties (Address Type Fields)

		#region ShipToName
		/// <summary>
		/// Person’s name associated with this shipping address. Character length and limitations: 32 single-byte characters.
		/// </summary>
		public string ShipToName
		{
			get { return _shipToName; }
			protected set { _shipToName = value; }
		}
		private string _shipToName;
		#endregion

		#region ShipToStreet
		/// <summary>
		/// First street address. Character length and limitations: 100 single-byte characters.
		/// </summary>
		public string ShipToStreet
		{
			get { return _shipToStreet; }
			protected set { _shipToStreet = value; }
		}
		private string _shipToStreet;
		#endregion

		#region ShipToStreet2
		/// <summary>
		/// Second street address. Character length and limitations: 100 single-byte characters.
		/// </summary>
		public string ShipToStreet2
		{
			get { return _shipToStreet2; }
			protected set { _shipToStreet2 = value; }
		}
		private string _shipToStreet2;
		#endregion

		#region ShipToCity
		/// <summary>
		/// Name of city. Character length and limitations: 40 single-byte characters.
		/// </summary>
		public string ShipToCity
		{
			get { return _shipToCity; }
			protected set { _shipToCity = value; }
		}
		private string _shipToCity;
		#endregion

		#region ShipToState
		/// <summary>
		/// State or province. Character length and limitations: 40 single-byte characters.
		/// </summary>
		public string ShipToState
		{
			get { return _shipToState; }
			protected set { _shipToState = value; }
		}
		private string _shipToState;
		#endregion

		#region ShipToZip
		/// <summary>
		/// U.S. ZIP code or other country-specific postal code. Character length and limitations: 20 single-byte characters.
		/// </summary>
		public string ShipToZip
		{
			get { return _shipToZip; }
			protected set { _shipToZip = value; }
		}
		private string _shipToZip;
		#endregion

		#region ShipToCountry
		/// <summary>
		/// Country code. Character limit: 2 single-byte characters.
		/// </summary>
		public string ShipToCountry
		{
			get { return _shipToCountry; }
			protected set { _shipToCountry = value; }
		}
		private string _shipToCountry;
		#endregion

		#region ShipToPhoneNumber
		/// <summary>
		/// Phone number. Character length and limit: 20 single-byte characters.
		/// </summary>
		public string ShipToPhoneNumber
		{
			get { return _shipToPhoneNumber; }
			protected set { _shipToPhoneNumber = value; }
		}
		private string _shipToPhoneNumber;
		#endregion

		#region AddressStatus
		/// <summary>
		/// Status of street address on file with PayPal. Valid values are: none/Confirmed/Unconfirmed
		/// </summary>
		public string AddressStatus
		{
			get { return _addressStatus; }
			protected set { _addressStatus = value; }
		}
		private string _addressStatus;
		#endregion

		#endregion

		#region Properties (Payment Details Type Fields)
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
		/// Kód měny.
		/// </summary>
		public PayPalCurrency CurrencyCode
		{
			get { return _currencyCode; }
			protected set { _currencyCode = value; }
		}
		private PayPalCurrency _currencyCode;

		/// <summary>
		/// Suma všech položek v objednávce.
		/// </summary>
		public decimal ItemAmount
		{
			get { return _itemAmount; }
			protected set { _itemAmount = value; }
		}
		private decimal _itemAmount;

		/// <summary>
		/// Cena (dopravy) poštovného.
		/// </summary>
		public decimal ShippingAmount
		{
			get { return _shippingAmount; }
			protected set { _shippingAmount = value; }
		}
		private decimal _shippingAmount;

		/// <summary>
		/// Cena pojištění.
		/// </summary>
		public decimal ShippingDiscount
		{
			get { return _shippingDiscount; }
			protected set { _shippingDiscount = value; }
		}
		private decimal _shippingDiscount;

		/// <summary>
		/// Daň.
		/// </summary>
		public decimal TaxAmount
		{
			get { return _taxAmount; }
			protected set { _taxAmount = value; }
		}
		private decimal _taxAmount;
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the PayPalGetExpressCheckoutDetailsResponse class.
		/// </summary>
		/// <param name="rawResponseData">The response data, raw.</param>
		public PayPalGetExpressCheckoutDetailsResponse(NameValueCollection rawResponseData)
			: base(rawResponseData)
		{
		}
		#endregion

		#region ParseResponseData
		/// <summary>
		/// Rozparsuje data do strong-type properties.
		/// Poznámka: Parsuje jenom základní informace, které potřebujeme.
		/// </summary>
		/// <param name="rawResponseData">data požadavku</param>s
		protected override void ParseResponseData(NameValueCollection rawResponseData)
		{
			base.ParseResponseData(rawResponseData);

			//---------------------------------------------------------------------------
			// Payer Information Fields
			//---------------------------------------------------------------------------
			if (!String.IsNullOrEmpty(rawResponseData["PAYERID"]))
			{
				this.PayerID = rawResponseData["PAYERID"];
			}

			if (!String.IsNullOrEmpty(rawResponseData["PAYERSTATUS"]))
			{
				this.PayerStatus = rawResponseData["PAYERSTATUS"];
			}

			if (!String.IsNullOrEmpty(rawResponseData["COUNTRYCODE"]))
			{
				this.CountryCode = rawResponseData["COUNTRYCODE"];
			}

			if (!String.IsNullOrEmpty(rawResponseData["EMAIL"]))
			{
				this.Email = rawResponseData["EMAIL"];
			}

			if (!String.IsNullOrEmpty(rawResponseData["BUSINESS"]))
			{
				this.Business = rawResponseData["BUSINESS"];
			}

			//---------------------------------------------------------------------------
			// Payment Details Type Fields (Amount, CurrencyCode, ...)
			//---------------------------------------------------------------------------
			if (!String.IsNullOrEmpty(rawResponseData["PAYMENTREQUEST_0_AMT"]))
			{
				decimal amount;
				if (Decimal.TryParse(rawResponseData["PAYMENTREQUEST_0_AMT"], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out amount))
				{
					this.Amount = amount;
				}
			}

			if (!String.IsNullOrEmpty(rawResponseData["PAYMENTREQUEST_0_CURRENCYCODE"]))
			{
				PayPalCurrency currency = PayPalCurrency.FindByCurrencyCode(rawResponseData["PAYMENTREQUEST_0_CURRENCYCODE"]);
				this.CurrencyCode = currency;
			}

			//---------------------------------------------------------------------------
			// GetExpressCheckoutDetails Response Fields
			//---------------------------------------------------------------------------
			if (!String.IsNullOrEmpty(rawResponseData["CHECKOUTSTATUS"]))
			{
				this.CheckoutStatus = rawResponseData["CHECKOUTSTATUS"];
			}
			
			//---------------------------------------------------------------------------
			// Payer Name Fields
			//---------------------------------------------------------------------------
			if (!String.IsNullOrEmpty(rawResponseData["SALUTATION"]))
			{
				this.Salutation = rawResponseData["SALUTATION"];
			}

			if (!String.IsNullOrEmpty(rawResponseData["FIRSTNAME"]))
			{
				this.FirstName = rawResponseData["FIRSTNAME"];
			}

			if (!String.IsNullOrEmpty(rawResponseData["MIDDLENAME"]))
			{
				this.MiddleName = rawResponseData["MIDDLENAME"];
			}

			if (!String.IsNullOrEmpty(rawResponseData["LASTNAME"]))
			{
				this.LastName = rawResponseData["LASTNAME"];
			}

			if (!String.IsNullOrEmpty(rawResponseData["SUFFIX"]))
			{
				this.Suffix = rawResponseData["SUFFIX"];
			}
			
			//---------------------------------------------------------------------------
			// Address Type Fields
			//---------------------------------------------------------------------------
			if (!String.IsNullOrEmpty(rawResponseData["PAYMENTREQUEST_0_SHIPTONAME"]))
			{
				this.ShipToName = rawResponseData["PAYMENTREQUEST_0_SHIPTONAME"];
			}

			if (!String.IsNullOrEmpty(rawResponseData["PAYMENTREQUEST_0_SHIPTOSTREET"]))
			{
				this.ShipToStreet = rawResponseData["PAYMENTREQUEST_0_SHIPTOSTREET"];
			}		

			if (!String.IsNullOrEmpty(rawResponseData["PAYMENTREQUEST_0_SHIPTOSTREET2"]))
			{
				this.ShipToStreet2 = rawResponseData["PAYMENTREQUEST_0_SHIPTOSTREET2"];
			}

			if (!String.IsNullOrEmpty(rawResponseData["PAYMENTREQUEST_0_SHIPTOCITY"]))
			{
				this.ShipToCity = rawResponseData["PAYMENTREQUEST_0_SHIPTOCITY"];
			}

			if (!String.IsNullOrEmpty(rawResponseData["PAYMENTREQUEST_0_SHIPTOSTATE"]))
			{
				this.ShipToState = rawResponseData["PAYMENTREQUEST_0_SHIPTOSTATE"];
			}

			if (!String.IsNullOrEmpty(rawResponseData["PAYMENTREQUEST_0_SHIPTOZIP"]))
			{
				this.ShipToZip = rawResponseData["PAYMENTREQUEST_0_SHIPTOZIP"];
			}

			if (!String.IsNullOrEmpty(rawResponseData["PAYMENTREQUEST_0_SHIPTOCOUNTRY"]))
			{
				this.ShipToCountry = rawResponseData["PAYMENTREQUEST_0_SHIPTOCOUNTRY"];
			}

		}
		#endregion
	}
}
