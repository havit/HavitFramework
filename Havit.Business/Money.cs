﻿using Havit.Diagnostics.Contracts;

namespace Havit.Business;

/// <summary>
/// Třída reprezentující peněžní částky s měnou.
/// </summary>
/// <typeparam name="TCurrency">Typ měny pro třídu Money.</typeparam>
[Serializable]
public class Money<TCurrency>
	where TCurrency : class
{
	/// <summary>
	/// Částka v jednotkách měny. Udává se hodnota v základní
	/// jednotce a zlomky měny (např. haléře) se zadávají za desetinou tečkou (např.
	/// 57.30).
	/// </summary>
	public decimal? Amount
	{
		get
		{
			return _amount;
		}
		private set
		{
			_amount = value;
		}
	}
	private decimal? _amount;

	/// <summary>
	/// Měna částky.
	/// </summary>
	public TCurrency Currency
	{
		get
		{
			return _currency;
		}
		private set
		{
			_currency = value;
		}
	}
	private TCurrency _currency;

	/// <summary>
	/// Inicializuje třídu money s prázdními hodnotami (Amount i Currency jsou null).
	/// </summary>
	public Money() : this(null, null)
	{
	}

	/// <summary>
	/// Inicializuje třídu money zadanými hodnotami.
	/// </summary>
	public Money(decimal? amount, TCurrency currency)
	{
		this._amount = amount;
		this._currency = currency;
	}

	/// <summary>
	/// Vrátí true, pokud se hodnoty aktuální instance rovnají hodnotám instance v parametru. Porovnává se částka a měna.
	/// </summary>
	public virtual bool Equals(Money<TCurrency> money)
	{
		if ((money == null) || (this.GetType() != money.GetType()))
		{
			return false;
		}
		return (this.Amount == money.Amount) && (this.Currency == money.Currency);
	}

	/// <summary>
	/// Vrátí true, pokud se hodnoty aktuální instance rovnají hodnotám instance v parametru. Porovnává se částka a měna.
	/// </summary>
	public override bool Equals(object obj)
	{
		if (obj is Money<TCurrency>)
		{
			Money<TCurrency> money = obj as Money<TCurrency>;
			return this.Equals(money);
		}
		return false;
	}

	/// <summary>
	/// Vrátí true, pokud se rovnají hodnoty instancí (porovnává se částka a měna), nebo pokud jsou obojí null.
	/// </summary>
	public static bool operator ==(Money<TCurrency> objA, Money<TCurrency> objB)
	{
		return Object.Equals(objA, objB);
	}

	/// <summary>
	/// Vrátí true, pokud se nerovnají hodnoty instancí (porovnává se částka a měna), nebo pokud má právě jeden z parametrů hodnotu null.		
	/// </summary>
	public static bool operator !=(Money<TCurrency> objA, Money<TCurrency> objB)
	{
		return !Object.Equals(objA, objB);
	}

	/// <summary>
	/// HashCode je složen jako XOR hash kódů amount a currency, pokud tyto hodnoty nejsou null.
	/// </summary>
	public override int GetHashCode()
	{
		int result = 0;

		if (_amount != null)
		{
			result ^= _amount.GetHashCode();
		}

		if (_currency != null)
		{
			result ^= _currency.GetHashCode();
		}

		return result;
	}

	/// <summary>
	/// Porovná měny zadané v parametrech. Pokud se liší, je vyhozena výjimka.
	/// </summary>
	protected static void AssertSameCurrencies(TCurrency currency1, TCurrency currency2)
	{
		if ((currency1 is BusinessObjectBase) && (currency2 is BusinessObjectBase))
		{
			// pokud jde o businessObjekty, pak porovnáme jako business objekty (může jít o různé instance stejné měny (např. objekty z různých identity map)
			// raději bych použil přetypování (BusinessObjectBase)currencyX, ale compiler ho z mě neznámého důvodu odmítá
			BusinessObjectBase businessObjectCurrency1 = currency1 as BusinessObjectBase;
			BusinessObjectBase businessObjectCurrency2 = currency2 as BusinessObjectBase;

			Contract.Requires<InvalidOperationException>(businessObjectCurrency1 == businessObjectCurrency2, "Currencies are not same.");
		}
		else
		{
			Contract.Requires<InvalidOperationException>(currency1 == currency2, "Currencies are not same.");
		}
	}

	/// <summary>
	/// Vyvolá výjimku, pokud má parametr hodnotu null.
	/// </summary>
	private static void AssertNotNull(object value, string parameterName)
	{
		Contract.Requires<ArgumentNullException>(value != null, parameterName);
	}

	/// <summary>
	/// Porovná se částka. Pokud se neshoduje měna, operace vyvolá výjimku.
	/// </summary>
	public static bool operator >(Money<TCurrency> money1, Money<TCurrency> money2)
	{
		AssertSameCurrencies(money1.Currency, money2.Currency);
		return money1.Amount > money2.Amount;
	}

	/// <summary>
	/// Porovná se částka. Pokud se neshoduje měna, operace vyvolá výjimku.
	/// </summary>
	public static bool operator <(Money<TCurrency> money1, Money<TCurrency> money2)
	{
		AssertSameCurrencies(money1.Currency, money2.Currency);
		return money1.Amount < money2.Amount;
	}

	/// <summary>
	/// Porovná se částka. Pokud se neshoduje měna, operace vyvolá výjimku.
	/// </summary>
	public static bool operator >=(Money<TCurrency> money1, Money<TCurrency> money2)
	{
		return (money1 > money2) || (money1 == money2);
	}

	/// <summary>
	/// Porovná se částka. Pokud se neshoduje měna, operace vyvolá výjimku.
	/// </summary>
	public static bool operator <=(Money<TCurrency> money1, Money<TCurrency> money2)
	{
		return (money1 < money2) || (money1 == money2);
	}

	/// <summary>
	/// Sečte dvě hodnoty Money. Pokud se neshoduje měna, operace vyvolá výjimku.
	/// </summary>
	public static Money<TCurrency> operator +(Money<TCurrency> money1, Money<TCurrency> money2)
	{
		return SumMoney<Money<TCurrency>>(money1, money2);
	}

	/// <summary>
	/// Odečte dvě hodnoty Money. Pokud se neshoduje měna, operace vyvolá výjimku.
	/// </summary>
	public static Money<TCurrency> operator -(Money<TCurrency> money1, Money<TCurrency> money2)
	{
		return SubtractMoney<Money<TCurrency>>(money1, money2);
	}

	/// <summary>
	/// Vynásobí hodnotu Money konstantou typu decimal.
	/// </summary>
	public static Money<TCurrency> operator *(Money<TCurrency> money, decimal multiplicand)
	{
		return MultipleMoney<Money<TCurrency>>(money, multiplicand);
	}

	/// <summary>
	/// Vynásobí hodnotu Money konstantou typu int.
	/// </summary>
	public static Money<TCurrency> operator *(Money<TCurrency> money, int multiplicand)
	{
		return MultipleMoney<Money<TCurrency>>(money, multiplicand);
	}

	/// <summary>
	/// Vydělí hodnotu Money konstantou typu decimal.
	/// </summary>
	public static Money<TCurrency> operator /(Money<TCurrency> money, decimal multiplicand)
	{
		return DivideMoney<Money<TCurrency>>(money, multiplicand);
	}

	/// <summary>
	/// Vydělí hodnotu Money konstantou typu int.
	/// </summary>
	public static Money<TCurrency> operator /(Money<TCurrency> money, int multiplicand)
	{
		return DivideMoney<Money<TCurrency>>(money, multiplicand);
	}

	/// <summary>
	/// Vypočte podíl částek. Např. pro výpočet poměru částek, marže, apod.
	/// </summary>
	public static decimal operator /(Money<TCurrency> dividend, Money<TCurrency> divisor)
	{
		return DivideMoney(dividend, divisor);
	}

	/// <summary>
	/// Sečte dvě hodnoty Money. Pokud se neshoduje měna, operace vyvolá výjimku.	
	/// </summary>
	/// <typeparam name="TResult">Cílový typ Money.</typeparam>
	public static TResult SumMoney<TResult>(Money<TCurrency> money1, Money<TCurrency> money2)
		where TResult : Money<TCurrency>, new()
	{
		AssertNotNull(money1, nameof(money1));
		AssertNotNull(money1.Amount, nameof(money1) + "." + nameof(money1.Amount));
		AssertNotNull(money2, nameof(money2));
		AssertNotNull(money2.Amount, nameof(money2) + "." + nameof(money2.Amount));
		AssertSameCurrencies(money1.Currency, money2.Currency);

		TResult result = new TResult();
		result.Amount = money1.Amount + money2.Amount;
		result.Currency = money1.Currency;
		return result;
	}

	/// <summary>
	/// Odečte měny (odčítá se money2 od money1). Pokud se neshoduje měna, operace vyvolá výjimku.
	/// </summary>
	/// <typeparam name="TResult">Cílový typ Money.</typeparam>
	public static TResult SubtractMoney<TResult>(Money<TCurrency> money1, Money<TCurrency> money2)
		where TResult : Money<TCurrency>, new()
	{
		AssertNotNull(money1, nameof(money1));
		AssertNotNull(money1.Amount, nameof(money1) + "." + nameof(money1.Amount));
		AssertNotNull(money2, nameof(money2));
		AssertNotNull(money2.Amount, nameof(money2) + "." + nameof(money2.Amount));
		AssertSameCurrencies(money1.Currency, money2.Currency);

		TResult result = new TResult();
		result.Amount = money1.Amount - money2.Amount;
		result.Currency = money1.Currency;
		return result;
	}

	/// <summary>
	/// Vynásobí částku konstantou.
	/// </summary>
	/// <typeparam name="TResult">Cílový typ Money.</typeparam>
	public static TResult MultipleMoney<TResult>(Money<TCurrency> money, decimal multiplicand)
		where TResult : Money<TCurrency>, new()
	{
		AssertNotNull(money, nameof(money));
		AssertNotNull(money.Amount, nameof(money) + "." + nameof(money.Amount));

		TResult result = new TResult();
		result.Amount = money.Amount * multiplicand;
		result.Currency = money.Currency;
		return result;
	}

	/// <summary>
	/// Vydělí částku konstantou.
	/// </summary>
	/// <typeparam name="TResult">Cílový typ Money.</typeparam>
	public static TResult DivideMoney<TResult>(Money<TCurrency> money, decimal divisor)
		where TResult : Money<TCurrency>, new()
	{
		AssertNotNull(money, nameof(money));
		AssertNotNull(money.Amount, nameof(money) + "." + nameof(money.Amount));

		TResult result = new TResult();
		result.Amount = money.Amount / divisor;
		result.Currency = money.Currency;
		return result;
	}

	/// <summary>
	/// Vypočte podíl částek. Např. pro výpočet poměru částek, marže, apod.
	/// </summary>
	public static decimal DivideMoney(Money<TCurrency> dividend, Money<TCurrency> divisor)
	{
		AssertNotNull(dividend, nameof(dividend));
		AssertNotNull(dividend.Amount, nameof(dividend) + "." + nameof(dividend.Amount));

		AssertNotNull(divisor, nameof(divisor));
		AssertNotNull(divisor.Amount, nameof(divisor) + "." + nameof(divisor.Amount));

		AssertSameCurrencies(dividend.Currency, divisor.Currency);

		decimal result = dividend.Amount.Value / divisor.Amount.Value;
		return result;
	}
}
