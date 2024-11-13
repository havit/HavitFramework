using System.Collections;

namespace Havit.Model.Collections.Generic;

/// <summary>
/// Kolekce filtrující data z jiné kolekce (resp. Listu).
/// </summary>
public class FilteringCollection<T> : ICollection<T>
{
	private readonly IList<T> _source;
	private readonly Func<T, bool> _filter;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	/// <param name="source">Podkladová kolekce, ve které jsou držena data.</param>
	/// <param name="filter">Filtr, kterým se podkladová kolekce filtruje.</param>
	public FilteringCollection(IList<T> source, Func<T, bool> filter)
	{
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
