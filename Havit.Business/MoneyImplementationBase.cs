using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business
{
	/// <summary>
	/// Tøída reprezentující èástku a mìnu.
	/// Tøída je urèena ke zdìdìní, potomkem by mìla být projektová tøída Money.
	/// </summary>
	public abstract class MoneyImplementationBase<TCurrency, TResult>: Money<TCurrency>
		where TCurrency: class
		where TResult : MoneyImplementationBase<TCurrency, TResult>, new()
	{
		#region Operátory +, -, *, /
		/// <summary>
		/// Seète dvì hodnoty Money. Pokud se neshoduje mìna, operace vyvolá výjimku.
		/// </summary>
		public static TResult operator +(MoneyImplementationBase<TCurrency, TResult> money1, MoneyImplementationBase<TCurrency, TResult> money2)
		{
			return SumMoney<TResult>(money1, money2);
		}

		/// <summary>
		/// Odeète dvì hodnoty Money. Pokud se neshoduje mìna, operace vyvolá výjimku.
		/// </summary>
		/// <returns></returns>
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
		/// Vydìlí hodnotu Money konstantou typu decimal.
		/// </summary>
		public static TResult operator /(MoneyImplementationBase<TCurrency, TResult> money, decimal multiplicand)
		{
			return DivideMoney<TResult>(money, multiplicand);
		}

		/// <summary>
		/// Vydìlí hodnotu Money konstantou typu int.
		/// </summary>
		public static TResult operator /(MoneyImplementationBase<TCurrency, TResult> money, int multiplicand)
		{
			return DivideMoney<TResult>(money, multiplicand);
		}

		/// <summary>
		/// Vypoète podíl èástek. Napø. pro výpoèet pomìru èástek, marže, apod.
		/// </summary>	
		public static decimal operator /(MoneyImplementationBase<TCurrency, TResult> dividend, MoneyImplementationBase<TCurrency, TResult> divisor)
		{
			return DivideMoney(dividend, divisor);
		}

		#endregion

		#region Constructors
		/// <summary>
		/// Inicializuje tøídu money s prázdními hodnotami (Amount i Currency jsou null).
		/// </summary>
		public MoneyImplementationBase()
			: base()
		{
		}

		/// <summary>
		/// Inicializuje tøídu money zadanými hodnotami.
		/// </summary>
		public MoneyImplementationBase(decimal? amount, TCurrency currency)
			: base(amount, currency)
		{
		}
		#endregion
	}
}
