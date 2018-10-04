using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Havit.PayPal
{
	/// <summary>
	/// Třída pro práci s měnama.
	/// </summary>
	public class PayPalCurrency
	{
		#region Hodnoty měn
		/// <summary>
		/// CZK, 203, Koruna česká.
		/// </summary>
		public static PayPalCurrency Czk { get { return FindByCurrencyCode("CZK"); } }

		/// <summary>
		/// EUR
		/// </summary>
		public static PayPalCurrency Eur { get { return FindByCurrencyCode("EUR"); } }
		
		/// <summary>
		/// GBP
		/// </summary>
		public static PayPalCurrency Gbp { get { return FindByCurrencyCode("GBP"); } }
		
		/// <summary>
		/// USD
		/// </summary>
		public static PayPalCurrency Usd { get { return FindByCurrencyCode("USD"); } }

		/// <summary>
		/// HUF
		/// </summary>
		public static PayPalCurrency Huf { get { return FindByCurrencyCode("HUF"); } }

		/// <summary>
		/// PLN
		/// </summary>
		public static PayPalCurrency Pln { get { return FindByCurrencyCode("PLN"); } }

		/// <summary>
		/// CAD
		/// </summary>
		public static PayPalCurrency Cad { get { return FindByCurrencyCode("CAD"); } }
		#endregion

		#region Properties
		/// <summary>
		/// Kód měny, např. "CZK". 
		/// </summary>
		public string Code
		{
			get { return _code; }
		}
		private readonly string _code;

		/// <summary>
		/// Číselný kód, např. 203 pro CZK
		/// </summary>
		public int NumericCode
		{
			get { return _numericCode; }
		}
		private readonly int _numericCode;
		
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="PayPalCurrency"/> class.
		/// https://cms.paypal.com/us/cgi-bin/?cmd=_render-content&amp;content_ID=developer/e_howto_api_nvp_currency_codes
		/// </summary> 
		/// <param name="code">Currency Code podle ISO 4217.</param>		
		/// <param name="numericCode">Numeric Code podle ISO 4217.</param>		
		private PayPalCurrency(string code, int numericCode)
		{
			_code = code;
			_numericCode = numericCode;			
		}
		#endregion

		/*******************************************************************************************************/

		#region currencyCodes, numericCodes (static, private)
		/// <summary>
		/// Hashtabulky s unikátními sdílenými instancemi.
		/// </summary>		
		private static readonly Hashtable currencyCodes;
		private static readonly Hashtable numericCodes;
		#endregion

		#region Static constructor
		/// <summary>
		/// Statický constructor
		/// </summary>
		static PayPalCurrency()
		{			
			currencyCodes = new Hashtable();
			numericCodes = new Hashtable();

			RegisterCurrency(new PayPalCurrency("CZK", 203));
			RegisterCurrency(new PayPalCurrency("EUR", 978));
			RegisterCurrency(new PayPalCurrency("USD", 840));
			RegisterCurrency(new PayPalCurrency("GBP", 826));
			RegisterCurrency(new PayPalCurrency("HUF", 348));
			RegisterCurrency(new PayPalCurrency("PLN", 985));
			RegisterCurrency(new PayPalCurrency("CAD", 124));
		}
		#endregion

		#region RegisterCurrency (static, private)
		/// <summary>
		/// Zaregistruje měnu do interní Hashtable.
		/// </summary>
		/// <param name="currency">měna k registraci</param>
		private static void RegisterCurrency(PayPalCurrency currency)
		{
			if (currency == null)
			{
				throw new ArgumentNullException("currency");
			}

			if (!currencyCodes.ContainsKey(currency.Code))
			{
				currencyCodes.Add(currency.Code, currency);
			}

			if (!numericCodes.ContainsKey(currency.NumericCode))
			{
				numericCodes.Add(currency.NumericCode, currency);
			}	
		}
		#endregion

		#region FindByCurrencyCode
		/// <summary>
		/// Najde měnu podle numerického kódu a vrátí ji. Pokud není nalezena, vrací <c>null</c>.
		/// </summary>
		/// <param name="numericCode">numerický kód měny</param>
		public static PayPalCurrency FindByNumericCode(int numericCode)
		{
			return (PayPalCurrency)numericCodes[numericCode];
		}
		#endregion

		#region FindByCurrencyCode
		/// <summary>
		/// Najde měnu podle třípísmenového kódu měny dle ISO 4217.
		/// </summary>
		/// <param name="currencyCode">Kód měny</param>
		public static PayPalCurrency FindByCurrencyCode(string currencyCode)
		{
			return (PayPalCurrency)currencyCodes[currencyCode.ToUpper()];		
		}
		#endregion
	}
}
