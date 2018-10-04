using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Collections.Generic
{
	/// <summary>
	/// ILookup s možností přidávání a odebírání položek.
	/// Vnitřně implementován pomocí Dictionary&lt;TKey, List&lt;TElement&gt;&gt;.
	/// </summary>
	/// <typeparam name="TKey">Typ klíče.</typeparam>
	/// <typeparam name="TElement">Typ položek.</typeparam>
	public class Lookup<TKey, TElement> : ILookup<TKey, TElement>
	{
		private readonly Dictionary<TKey, List<TElement>> _dictionary;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public Lookup() : this(null, null)
		{
		}

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public Lookup(IEqualityComparer<TKey> equalityComparer) : this(null, equalityComparer)
		{
		}

		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="source">Zdrojová data, kterými se Lookup naplní.</param>
		public Lookup(ILookup<TKey, TElement> source) : this(source, null)
		{
		}

		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="source">Zdrojová data, kterými se Lookup naplní.</param>
		/// <param name="equalityComparer">Comparer pro porovnání objektů.</param>
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
		/// Vrací počet položek, kontrétně počet klíčů.
		/// </summary>
		public int Count
		{
			get
			{
				return _dictionary.Count;
			}
		}

		/// <summary>
		/// Vrátí seznam objektů uložených pod daným klíčem.
		/// </summary>
		/// <param name="key">Klíč</param>
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
		/// Indikuje, zda je v Lookup obsažen požadovaný klíč.
		/// </summary>
		/// <param name="key">Klíč</param>
		public bool Contains(TKey key)
		{
			return _dictionary.ContainsKey(key);
		}

		/// <summary>
		/// Odstraní z kolekce daný klíč (a s tím i k němu uložené hodnoty).
		/// </summary>
		/// <param name="key">Klíč.</param>
		/// <returns>True, pokud byl klíč odebrán, jinak false (klíč se v kolekci nevyskytoval).</returns>
		public bool RemoveKey(TKey key)
		{
			return _dictionary.Remove(key);
		}

		/// <summary>
		/// Pod daný klíč přidá daný objekt (element).
		/// </summary>
		/// <param name="key">Klíč.</param>
		/// <param name="element">Přidávaný objekt.</param>
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
		/// Odebere daný objekt od daného klíče.
		/// Pokud je odebrán poslední objekt daného klíče, není klíč odebírán a zůstává v kolekci stále přítomen. Tj. Contains(key) bude stále vracet true! Pokud má být klíč odebrán, je potřeba zavolat metodu RemoveKey.
		/// </summary>
		/// <param name="key">Klíč.</param>
		/// <param name="element">Odebíraný objekt-</param>
		/// <returns>True, pokud byl objekt odebrán, jinak false (klíč není registrován nebo pod sebou nemá odebíraný objekt).</returns>
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
}