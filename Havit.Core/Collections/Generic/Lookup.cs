using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Collections.Generic;

/// <summary>
/// ILookup with the ability to add and remove items.
/// Internally implemented using Dictionary&lt;TKey, List&lt;TElement&gt;&gt;.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <typeparam name="TElement">The type of the elements.</typeparam>
public class Lookup<TKey, TElement> : ILookup<TKey, TElement>
{
	private readonly Dictionary<TKey, List<TElement>> _dictionary;

	/// <summary>
	/// Constructor.
	/// </summary>
	public Lookup() : this(null, null)
	{
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	public Lookup(IEqualityComparer<TKey> equalityComparer) : this(null, equalityComparer)
	{
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="source">The source data with which the Lookup is populated.</param>
	public Lookup(ILookup<TKey, TElement> source) : this(source, null)
	{
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="source">The source data with which the Lookup is populated.</param>
	/// <param name="equalityComparer">The comparer used to compare objects.</param>
	public Lookup(ILookup<TKey, TElement> source, IEqualityComparer<TKey> equalityComparer)
	{
		_dictionary = new Dictionary<TKey, List<TElement>>(equalityComparer);

		if (source != null)
		{
			foreach (IGrouping<TKey, TElement> grouping in source)
			{
				_dictionary.Add(grouping.Key, grouping.ToList());
			}
		}
	}

	/// <summary>
	/// Returns the number of items, specifically the number of keys.
	/// </summary>
	public int Count
	{
		get
		{
			return _dictionary.Count;
		}
	}

	/// <summary>
	/// Returns a list of objects stored under the given key.
	/// </summary>
	/// <param name="key">The key.</param>
	public IEnumerable<TElement> this[TKey key]
	{
		get
		{
			List<TElement> list;
			if (_dictionary.TryGetValue(key, out list))
			{
				return list.AsReadOnly();
			}

			return Enumerable.Empty<TElement>();
		}
	}

	/// <summary>
	/// Indicates whether the desired key is contained in the Lookup.
	/// </summary>
	/// <param name="key">The key.</param>
	public bool Contains(TKey key)
	{
		return _dictionary.ContainsKey(key);
	}

	/// <summary>
	/// Removes the given key from the collection (and with it the values stored under it).
	/// </summary>
	/// <param name="key">The key.</param>
	/// <returns>True if the key was removed, otherwise false (the key was not present in the collection).</returns>
	public bool RemoveKey(TKey key)
	{
		return _dictionary.Remove(key);
	}

	/// <summary>
	/// Adds the given object (element) under the given key.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <param name="element">The object being added.</param>
	public void Add(TKey key, TElement element)
	{
		List<TElement> list;

		if (!_dictionary.TryGetValue(key, out list))
		{
			list = new List<TElement>();
			_dictionary.Add(key, list);
		}

		list.Add(element);
	}

	/// <summary>
	/// Removes the given object from the given key.
	/// If the last object of the given key is removed, the key is not removed and remains present in the collection. That is, Contains(key) will still return true! If the key is to be removed, the RemoveKey method must be called.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <param name="element">The object being removed.</param>
	/// <returns>True if the object was removed, otherwise false (the key is not registered or does not have the object being removed under it).</returns>
	public bool Remove(TKey key, TElement element)
	{
		List<TElement> list;

		if (_dictionary.TryGetValue(key, out list))
		{
			return list.Remove(element);
		}

		return false;
	}

	/// <summary>
	/// Returns an enumerator that iterates through a collection.
	/// </summary>
	public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
	{
		return GetGroupings().GetEnumerator();
	}

	/// <summary>
	/// Returns an enumerator that iterates through a collection.
	/// </summary>
	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
	{
		return (GetGroupings() as System.Collections.IEnumerable).GetEnumerator();
	}

	private IEnumerable<IGrouping<TKey, TElement>> GetGroupings()
	{
		return _dictionary.Keys.Select(key => new LookupDictionaryGrouping
		{
			Key = key,
			Elements = _dictionary[key]
		});
	}

	internal class LookupDictionaryGrouping : IGrouping<TKey, TElement>
	{
		public TKey Key { get; set; }

		public IEnumerable<TElement> Elements { get; set; }

		public IEnumerator<TElement> GetEnumerator()
		{
			return Elements.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (Elements as System.Collections.IEnumerable).GetEnumerator();
		}
	}
}
