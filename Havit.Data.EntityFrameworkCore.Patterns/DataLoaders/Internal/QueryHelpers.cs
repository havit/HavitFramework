using System.Linq.Expressions;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;

internal static class QueryHelpers
{
	/// <summary>
	/// Vrátí expression ověřující zda <c>values</c> obsahuje hodnotu získanou z <c>propertyAccessor</c>.
	/// Obsahuje optimalizace:
	/// - Pokud je ve <c>values</c> 0 hodnot, vrátí expression vracející vždy false.
	/// - Pokud je ve <c>values</c> 1 hodnota, vrátí expression ověřující rovnost této hodnoty.
	/// - Pokud je ve <c>values</c> posloupnost hodnot a jedná se o nepřerušenou posloupnost čísel (např. 1,2,3,4), vrátí expression ověřující rozsah (&gt;= první a =&lt; poslední).
	/// </summary>
	/// <typeparam name="TEntity">Typ entity.</typeparam>
	/// <param name="values">Hodnoty, ve kterých musí být hodnota <c>propertyAccessor</c>, aby byla podmínka Contains(Effective) splněna.</param>
	/// <param name="propertyAccessor">Property accessor k hodnotě, která se ověřuje, že je obsažena ve <c>values</c>.</param>
	/// <returns>Expression ověřující zda <c>values</c> obsahuje hodnotu získanou z <c>propertyAccessor</c></returns>
	internal static Expression<Func<TEntity, bool>> ContainsEffective<TEntity>(this List<int> values, Expression<Func<TEntity, int>> propertyAccessor)
	{
		ArgumentNullException.ThrowIfNull(values);

		// žádný hodnota -> item => false;
		if (values.Count == 0)
		{
			return (Expression<Func<TEntity, bool>>)Expression.Lambda(Expression.Constant(false), propertyAccessor.Parameters);
		}

		// jediný záznam - testujeme na rovnost
		if (values.Count == 1)
		{
			var singleValueHolder = new SingleValueHolder(values.Single());

			// item => item == [0]
			return (Expression<Func<TEntity, bool>>)Expression.Lambda(
				Expression.Equal(
					propertyAccessor.Body,
					Expression.Property(Expression.Constant(singleValueHolder), nameof(SingleValueHolder.Value))),
				propertyAccessor.Parameters);
		}

		// více záznamů

		// Pokud jde o řadu IDček (1, 2, 3, 4) bez přeskakování, pak použijeme porovnání >= a  <=.
		if (ContainsConsecutiveSequence(values, out int minValue, out int maxValue))
		{
			var fromToValueHolder = new FromToValueHolder(minValue, maxValue);

			return (Expression<Func<TEntity, bool>>)Expression.Lambda(
				Expression.AndAlso(
					Expression.GreaterThanOrEqual(propertyAccessor.Body, Expression.Property(Expression.Constant(fromToValueHolder), nameof(FromToValueHolder.FromValue))),
					Expression.LessThanOrEqual(propertyAccessor.Body, Expression.Property(Expression.Constant(fromToValueHolder), nameof(FromToValueHolder.ToValue)))),
				propertyAccessor.Parameters);
		}

		// V obecném případě hledáme přes IN (...) resp. pomocí OPENJSON.
		var listValuesHolder = new ListValuesHolder(values);
		return (Expression<Func<TEntity, bool>>)Expression.Lambda(
			Expression.Call(
				//allowEfParameter
				//	? Expression.Call(null, s_efParameterGenericMethodInfo, Expression.Property(Expression.Constant(listValuesHolder), nameof(ListValuesHolder.Values)))
				//	:
				Expression.Property(Expression.Constant(listValuesHolder), nameof(ListValuesHolder.Values)),
				typeof(List<int>).GetMethod("Contains"),
				new List<Expression> { propertyAccessor.Body }),
			propertyAccessor.Parameters);
	}

	/// <summary>
	/// Ověří zda values obsahuje sekvenci v O(n) s jediným průchodem seznamu.
	/// </summary>
	internal static bool ContainsConsecutiveSequence(List<int> values, out int minValue, out int maxValue)
	{
		if ((values == null)
			|| (values.Count == 0)
			// Z výkonostních důvodů ale nechceme hledání posloupnosti provádět vždy, ale jen za splnění (triviální) podmínky "nutné" k tomu,
			// aby vůbec mohlo jít o posloupnost IDček.
			// Podmínku stanovíme tak, že rozdíl hodnoty prvního a posledního prvku v nesetříděném seznamu musí být menší
			// než počet prvků tohoto seznamu.
			// Příklad 1: Pro posloupnost 1, 2, 3, 4 je rozdíl prvního a posledního prvku 3 (Abs(1-4)) a ten je menší než počet prvků 4.
			// Příklad 2: Pro posloupcnost 1, 2, 4, 5 je rozdíl prvního a posledního prvku 4 a ten již není menší než počet prvků 4.
			// Příklad 3: Pro posloupnost 1, 5, 5, 4 je podmínka splněná, tak dojde k ověření posloupnosti (což se nepotvrdí).		
			|| (!(Math.Abs(values[values.Count - 1] - values[0]) < values.Count)))
		{
			minValue = default;
			maxValue = default;
			return false;
		}

		int min = int.MaxValue;
		int max = int.MinValue;
		var hashSet = new HashSet<int>(values.Count);

		foreach (var item in values)
		{
			hashSet.Add(item);

			if (item < min)
			{
				min = item;
			}

			if (item > max)
			{
				max = item;
			}
		}

		if ((max - min + 1) == hashSet.Count)
		{
			minValue = min;
			maxValue = max;
			return true;
		}

		minValue = default;
		maxValue = default;
		return false;
	}

	// SingleValueHolder.Value, FromToValueHolder.FromValue, FromToValueHolder.ToValue, ListValuesHolder.Values:
	// Názvy vlastností se propisují do názvu SQL Parametrů v databázovém dotazu.

	private struct SingleValueHolder
	{
		public int Value { get; }

		public SingleValueHolder(int value)
		{
			Value = value;
		}
	}

	private struct FromToValueHolder
	{
		public int FromValue { get; }
		public int ToValue { get; }

		public FromToValueHolder(int fromValue, int toValue)
		{
			FromValue = fromValue;
			ToValue = toValue;
		}
	}

	private struct ListValuesHolder
	{
		public List<int> Values { get; }

		public ListValuesHolder(List<int> values)
		{
			Values = values;
		}
	}
}
