using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business;

/// <summary>
/// Třída reprezentující částku a měnu.
/// Třída je určena ke zdědění, potomkem by měla být projektová třída Money.
/// </summary>
/// <typeparam name="TCurrency">Typ měny pro vlastní implementaci Money.</typeparam>
/// <typeparam name="TResult">Money.</typeparam>
public abstract class MoneyImplementationBase<TCurrency, TResult> : Money<TCurrency>
	where TCurrency : class
	where TResult : MoneyImplementationBase<TCurrency, TResult>, new()
{
	/// <summary>
	/// Sečte dvě hodnoty Money. Pokud se neshoduje měna, operace vyvolá výjimku.
	/// </summary>
	public static TResult operator +(MoneyImplementationBase<TCurrency, TResult> money1, MoneyImplementationBase<TCurrency, TResult> money2)
	{
		return SumMoney<TResult>(money1, money2);
	}

	/// <summary>
	/// Odečte dvě hodnoty Money. Pokud se neshoduje měna, operace vyvolá výjimku.
	/// </summary>
	public static TResult operator -(MoneyImplementationBase<TCurrency, TResult> money1, MoneyImplementationBase<TCurrency, TResult> money2)
	{
		return SubtractMoney<TResult>(money1, money2);
	}

	/// <summary>
	/// Vynásobí hodnotu Money konstantou typu decimal.
	/// </summary>
	public static TResult operator *(MoneyImplementationBase<TCurrency, TResult> money, decimal multiplicand)
	{
		return MultipleMoney<TResult>(money, multiplicand);
	}

	/// <summary>
	/// Vynásobí hodnotu Money konstantou typu int.
	/// </summary>
	public static TResult operator *(MoneyImplementationBase<TCurrency, TResult> money, int multiplicand)
	{
		return MultipleMoney<TResult>(money, multiplicand);
	}

	/// <summary>
	/// Vydělí hodnotu Money konstantou typu decimal.
	/// </summary>
	public static TResult operator /(MoneyImplementationBase<TCurrency, TResult> money, decimal multiplicand)
	{
		return DivideMoney<TResult>(money, multiplicand);
	}

	/// <summary>
	/// Vydělí hodnotu Money konstantou typu int.
	/// </summary>
	public static TResult operator /(MoneyImplementationBase<TCurrency, TResult> money, int multiplicand)
	{
		return DivideMoney<TResult>(money, multiplicand);
	}

	/// <summary>
	/// Vypočte podíl částek. Např. pro výpočet poměru částek, marže, apod.
	/// </summary>	
	public static decimal operator /(MoneyImplementationBase<TCurrency, TResult> dividend, MoneyImplementationBase<TCurrency, TResult> divisor)
	{
		return DivideMoney(dividend, divisor);
	}

	/// <summary>
	/// Inicializuje třídu money s prázdními hodnotami (Amount i Currency jsou null).
	/// </summary>
	protected MoneyImplementationBase()
	{
	}

	/// <summary>
	/// Inicializuje třídu money zadanými hodnotami.
	/// </summary>
	protected MoneyImplementationBase(decimal? amount, TCurrency currency)
		: base(amount, currency)
	{
	}
}
