using System.Collections;

namespace Havit.Model.Collections.Generic;

/// <summary>
/// Kolekce filtrující data z jiné kolekce (resp. Listu).
/// </summary>
/// <remarks>
/// Pozor na nesymetrickou sémantiku filtru, která záměrně porušuje obvyklý kontrakt <see cref="ICollection{T}"/>:
/// <list type="bullet">
/// <item><description>Mutace (<see cref="Add"/>, <see cref="AddRange"/>, <see cref="Remove"/>, <see cref="RemoveAll"/>, <see cref="Clear"/>) pracují přímo nad podkladovou kolekcí <b>bez ohledu na filtr</b>.</description></item>
/// <item><description>Dotazy (<see cref="Count"/>, <see cref="Contains"/>, <see cref="CopyTo"/>, <see cref="GetEnumerator"/>, <see cref="ForEach"/>) <b>filtr aplikují</b>.</description></item>
/// </list>
/// Důsledkem je, že <see cref="Add"/> může přidat objekt, který <see cref="Contains"/> následně nevidí, a <see cref="Remove"/> může odebrat objekt, který <see cref="Contains"/> hlásí jako neobsažený.
/// </remarks>
public class FilteringCollection<T> : ICollection<T>
{
	private readonly IList<T> _source;
	private readonly Func<T, bool> _filter;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	/// <param name="source">Podkladová kolekce, ve které jsou držena data.</param>
	/// <param name="filter">Filtr, kterým se podkladová kolekce filtruje.</param>
	/// <exception cref="ArgumentNullException">Vyhozena, pokud je <paramref name="source"/> nebo <paramref name="filter"/> <c>null</c>.</exception>
	public FilteringCollection(IList<T> source, Func<T, bool> filter)
	{
		if (source == null)
		{
			throw new ArgumentNullException(nameof(source));
		}
		if (filter == null)
		{
			throw new ArgumentNullException(nameof(filter));
		}

		this._source = source;
		this._filter = filter;
	}

	/// <inheritdoc />
	public IEnumerator<T> GetEnumerator()
	{
		return _source.Where(_filter).GetEnumerator();
	}

	/// <inheritdoc />
	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	/// <summary>
	/// Přidá objekt do podkladové kolekce (bez ohledu na filtr).
	/// </summary>
	public void Add(T item)
	{
		_source.Add(item);
	}

	/// <summary>
	/// Přidá objekty do podkladové kolekce (bez ohledu na filtr).
	/// </summary>
	public void AddRange(IEnumerable<T> collection)
	{
		if (_source is List<T> list)
		{
			list.AddRange(collection);
		}
		else
		{
			foreach (var item in collection)
			{
				_source.Add(item);
			}
		}
	}

	/// <summary>
	/// Odstraní všechny objekty z podkladové kolekce (bez ohledu na filtr).
	/// </summary>
	public void Clear()
	{
		_source.Clear();
	}

	/// <summary>
	/// Vrací true, pokud podkladová kolekce po aplikování filtru obsahuje daný objekt.
	/// </summary>
	public bool Contains(T item)
	{
		return _source.Where(_filter).Contains(item);
	}

	/// <summary>
	/// Kopíruje podkladovou kolekci po aplikování filtru do cílového pole.
	/// </summary>
	public void CopyTo(T[] array, int arrayIndex)
	{
		_source.Where(_filter).ToList().CopyTo(array, arrayIndex);
	}

	/// <summary>
	/// Odebere objekt z podkladové kolekce (bez ohledu na filtr).
	/// </summary>
	public bool Remove(T item)
	{
		return _source.Remove(item);
	}

	/// <summary>
	/// Odebere objekty z podkladové kolekce (bez ohledu na filtr), tj. predicate se bude volat na všechny objekty podkladové kolekce, bez ohledu na filtr.
	/// </summary>
	public int RemoveAll(Predicate<T> predicate)
	{
		if (_source is List<T> list)
		{
			return list.RemoveAll(predicate);
		}
		else
		{
			int count = 0;
			for (int i = _source.Count - 1; i >= 0; i--)
			{
				if (predicate(_source[i]))
				{
					_source.RemoveAt(i);
					count++;
				}
			}
			return count;
		}
	}

	/// <summary>
	/// Spustí action nad každým objektem v podkladové kolekci po aplikování filtru.
	/// </summary>
	public void ForEach(Action<T> action)
	{
		_source.Where(_filter).ToList().ForEach(action);
	}

	/// <summary>
	/// Vrací počet objektů z podkladové kolekci po aplikování filtru.
	/// Nemá konstantní, ale lineární (vzhledem k velikosti kolekce) časovou složitost!
	/// </summary>
	public int Count => _source.Where(_filter).Count();

	/// <inheritdoc />
	public bool IsReadOnly => false;
}
