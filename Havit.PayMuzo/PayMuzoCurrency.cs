using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Havit.PayMuzo;

/// <summary>
/// Třída pro identifikátor měny v PayMUZO komunikaci. Pro použití v BusinessLayeru je připraven TypeConverter na Int32.
/// PayMUZO stejně podporuje jenom Kč s kódem currency 203.
/// </summary>
public class PayMuzoCurrency
{
	/// <summary>
	/// CZK, 203, Koruna česká.
	/// </summary>
	public static PayMuzoCurrency Czk { get { return FindByNumericCode(203); } }

	/// <summary>
	/// EUR, 978, Euro
	/// </summary>
	public static PayMuzoCurrency Eur
	{
		get
		{
			return FindByNumericCode(978);
		}
	}

	/// <summary>
	/// USD, 840, Americký dolar.
	/// </summary>
	public static PayMuzoCurrency Usd
	{
		get
		{
			return FindByNumericCode(840);
		}
	}

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

	/// <summary>
	/// Počet nejmenších jednotek dané měny. Např. 100 pro haléře.
	/// </summary>
	public int SmallestUnits
	{
		get { return _smallestUnits; }
	}
	private readonly int _smallestUnits;

	/// <summary>
	/// Initializes a new instance of the <see cref="PayMuzoCurrency"/> class.
	/// </summary>
	/// <param name="code">Currency Code podle ISO 4217.</param>
	/// <param name="numericCode">Numeric Code podle ISO 4217.</param>
	/// <param name="smallestUnits">Počet nejmenších jednotek dané měny. Např. 100 pro haléře.</param>
	private PayMuzoCurrency(string code, int numericCode, int smallestUnits)
	{
		_code = code;
		_numericCode = numericCode;
		_smallestUnits = smallestUnits;
	}

	/// <summary>
	/// Hashtabulka s unikátními sdílenými instancemi.
	/// </summary>
	private static readonly Hashtable currencies;

	/// <summary>
	/// Statický constructor
	/// </summary>
	static PayMuzoCurrency()
	{
		currencies = new Hashtable();

		RegisterCurrency(new PayMuzoCurrency("CZK", 203, 100));
		RegisterCurrency(new PayMuzoCurrency("EUR", 978, 100));
		RegisterCurrency(new PayMuzoCurrency("USD", 840, 100));
	}

	/// <summary>
	/// Zaregistruje měnu do interní Hashtable.
	/// </summary>
	/// <param name="currency">měna k registraci</param>
	private static void RegisterCurrency(PayMuzoCurrency currency)
	{
		if (currency == null)
		{
			throw new ArgumentNullException("currency");
		}

		if (!currencies.ContainsKey(currency.NumericCode))
		{
			currencies.Add(currency.NumericCode, currency);
		}
	}

	/// <summary>
	/// Najde měnu podle numerického kódu a vrátí ji. Pokud není nalezena, vrací <c>null</c>.
	/// </summary>
	/// <param name="numericCode">numerický kód měny</param>
	public static PayMuzoCurrency FindByNumericCode(int numericCode)
	{
		return (PayMuzoCurrency)currencies[numericCode];
	}
}
