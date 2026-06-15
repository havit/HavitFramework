using System.Linq.Expressions;
using System.Numerics;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;

internal static class QueryHelpers
{
	/// <summary>
	/// Vrátí expression ověřující zda <c>values</c> obsahuje hodnotu získanou z <c>propertyAccessor</c>.
	/// Obsahuje optimalizace:
	/// - Pokud je ve <c>values</c> 0 hodnot, vrátí expression vracející vždy false.
	/// - Pokud je ve <c>values</c> 1 hodnota, vrátí expression ověřující rovnost této hodnoty.
	/// </summary>
	/// <remarks>
	/// Pro celočíselné klíče (<see cref="IBinaryInteger{TSelf}" />) je k dispozici varianta <see cref="ContainsEffectiveInteger" />, která navíc umí optimalizaci na souvislou posloupnost (rozsah).
	/// </remarks>
	/// <typeparam name="TEntity">Typ entity.</typeparam>
	/// <typeparam name="TKey">Typ klíče.</typeparam>
	/// <param name="values">Hodnoty, ve kterých musí být hodnota <c>propertyAccessor</c>, aby byla podmínka Contains(Effective) splněna.</param>
	/// <param name="propertyAccessor">Property accessor k hodnotě, která se ověřuje, že je obsažena ve <c>values</c>.</param>
	/// <returns>Expression ověřující zda <c>values</c> obsahuje hodnotu získanou z <c>propertyAccessor</c></returns>
	internal static Expression<Func<TEntity, bool>> ContainsEffective<TEntity, TKey>(this List<TKey> values, Expression<Func<TEntity, TKey>> propertyAccessor)
	{
		ArgumentNullException.ThrowIfNull(values);

		// žádná hodnota -> item => false;
		if (values.Count == 0)
		{
			return (Expression<Func<TEntity, bool>>)Expression.Lambda(Expression.Constant(false), propertyAccessor.Parameters);
		}

		// jediný záznam - testujeme na rovnost
		if (values.Count == 1)
		{
			var singleValueHolder = new SingleValueHolder<TKey>(values[0]);

			// item => item == [0]
			return (Expression<Func<TEntity, bool>>)Expression.Lambda(
				Expression.Equal(
					propertyAccessor.Body,
					Expression.Property(Expression.Constant(singleValueHolder), nameof(SingleValueHolder<TKey>.Value))),
				propertyAccessor.Parameters);
		}

		// více záznamů - hledáme přes IN (...) resp. pomocí OPENJSON.
		return values.ContainsViaListContains(propertyAccessor);
	}

	/// <summary>
	/// Varianta <see cref="ContainsEffective" /> pro celočíselné klíče.
	/// Navíc obsahuje optimalizaci: pokud hodnoty tvoří souvislou posloupnost (např. 1,2,3,4), vrátí expression ověřující rozsah (&gt;= první a &lt;= poslední).
	/// </summary>
	internal static Expression<Func<TEntity, bool>> ContainsEffectiveInteger<TEntity, TKey>(this List<TKey> values, Expression<Func<TEntity, TKey>> propertyAccessor)
		where TKey : IBinaryInteger<TKey>
	{
		ArgumentNullException.ThrowIfNull(values);

		// Pokud jde o souvislou řadu hodnot (1, 2, 3, 4) bez přeskakování, použijeme porovnání >= a <=.
		if ((values.Count >= 2) && TryGetConsecutiveRange(values, out TKey minValue, out TKey maxValue))
		{
			var fromToValueHolder = new FromToValueHolder<TKey>(minValue, maxValue);

			return (Expression<Func<TEntity, bool>>)Expression.Lambda(
				Expression.AndAlso(
					Expression.GreaterThanOrEqual(propertyAccessor.Body, Expression.Property(Expression.Constant(fromToValueHolder), nameof(FromToValueHolder<TKey>.FromValue))),
					Expression.LessThanOrEqual(propertyAccessor.Body, Expression.Property(Expression.Constant(fromToValueHolder), nameof(FromToValueHolder<TKey>.ToValue)))),
				propertyAccessor.Parameters);
		}

		// jinak obecná varianta (0 / 1 / IN (...))
		return values.ContainsEffective(propertyAccessor);
	}

	private static Expression<Func<TEntity, bool>> ContainsViaListContains<TEntity, TKey>(this List<TKey> values, Expression<Func<TEntity, TKey>> propertyAccessor)
	{
		var listValuesHolder = new ListValuesHolder<TKey>(values);
		return (Expression<Func<TEntity, bool>>)Expression.Lambda(
			Expression.Call(
				Expression.Property(Expression.Constant(listValuesHolder), nameof(ListValuesHolder<TKey>.Values)),
				typeof(List<TKey>).GetMethod(nameof(List<TKey>.Contains), new[] { typeof(TKey) }),
				new List<Expression> { propertyAccessor.Body }),
			propertyAccessor.Parameters);
	}

	/// <summary>
	/// Ověří zda values obsahují souvislou posloupnost (bez přeskakování) v O(n) s jediným průchodem seznamu.
	/// </summary>
	/// <remarks>
	/// Aritmetika porovnání se provádí v <see cref="Int128" />, aby nedošlo k přetečení ani u krajních hodnot (např. plný rozsah <see cref="byte" /> nebo <see cref="ulong" />).
	/// </remarks>
	internal static bool TryGetConsecutiveRange<TKey>(List<TKey> values, out TKey minValue, out TKey maxValue)
		where TKey : IBinaryInteger<TKey>
	{
		minValue = default;
		maxValue = default;

		if ((values == null) || (values.Count == 0))
		{
			return false;
		}

		// Z výkonostních důvodů nechceme hledání posloupnosti provádět vždy, ale jen za splnění (triviální) nutné podmínky:
		// rozdíl hodnoty prvního a posledního prvku v nesetříděném seznamu musí být menší než počet prvků.
		// Příklad 1: Pro posloupnost 1, 2, 3, 4 je rozdíl prvního a posledního prvku 3 (Abs(1-4)) a ten je menší než počet prvků 4.
		// Příklad 2: Pro posloupnost 1, 2, 4, 5 je rozdíl prvního a posledního prvku 4 a ten již není menší než počet prvků 4.
		// Příklad 3: Pro posloupnost 1, 5, 5, 4 je podmínka splněná, tak dojde k ověření posloupnosti (což se nepotvrdí).
		Int128 first = Int128.CreateChecked(values[0]);
		Int128 last = Int128.CreateChecked(values[values.Count - 1]);
		if (Int128.Abs(last - first) >= values.Count)
		{
			return false;
		}

		TKey min = values[0];
		TKey max = values[0];
		var hashSet = new HashSet<TKey>(values.Count);

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

		// (max - min + 1) == počet různých hodnot  =>  jde o souvislou posloupnost.
		// Rozdíl počítáme v Int128 (min a max převedeme jednotlivě), aby nedošlo k přetečení v TKey.
		Int128 rangeSize = (Int128.CreateChecked(max) - Int128.CreateChecked(min)) + Int128.One;
		if (rangeSize == hashSet.Count)
		{
			minValue = min;
			maxValue = max;
			return true;
		}

		return false;
	}

	// SingleValueHolder.Value, FromToValueHolder.FromValue, FromToValueHolder.ToValue, ListValuesHolder.Values:
	// Názvy vlastností se propisují do názvu SQL Parametrů v databázovém dotazu.

	private struct SingleValueHolder<TKey>
	{
		public TKey Value { get; }

		public SingleValueHolder(TKey value)
		{
			Value = value;
		}
	}

	private struct FromToValueHolder<TKey>
	{
		public TKey FromValue { get; }
		public TKey ToValue { get; }

		public FromToValueHolder(TKey fromValue, TKey toValue)
		{
			FromValue = fromValue;
			ToValue = toValue;
		}
	}

	private struct ListValuesHolder<TKey>
	{
		public List<TKey> Values { get; }

		public ListValuesHolder(List<TKey> values)
		{
			Values = values;
		}
	}
}
