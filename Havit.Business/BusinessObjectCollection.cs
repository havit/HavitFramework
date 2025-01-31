using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Havit.Collections;
using Havit.Data;
using Havit.Reflection;

namespace Havit.Business;

/// <summary>
/// Bázová třída pro všechny kolekce BusinessObjectBase (Layer SuperType)
/// </summary>
/// <remarks>
/// POZOR! Vnitřní implementace je závislá na faktu, že this.Items je List(Of T).
/// To je výchozí chování Collection(Of T), ale pro jistotu si to ještì vynucujeme
/// použitím wrappujícího constructoru.
/// </remarks>
/// <typeparam name="TItem">èlenský typ kolekce</typeparam>
/// <typeparam name="TCollection">typ používané business object kolekce</typeparam>
public class BusinessObjectCollection<TItem, TCollection> : Collection<TItem>, ICollection<TItem>, IDataBinderExtSetValue
	where TItem : BusinessObjectBase
	where TCollection : BusinessObjectCollection<TItem, TCollection>, new()
{
	/// <summary>
	/// Událost vyvolaná po jakékoliv zmìnì kolekce (Insert, Remove, Set, Clear).
	/// </summary>
	public event EventHandler CollectionChanged;

	/// <summary>
	/// Příznak, pokud je false znamená to,že LoadAll nemusí načítat objekty - v kolekci není žádný ghost.
	/// Pokud je true, v kolekci může (ale nemusí) být ghost.
	/// </summary>
	protected bool LoadAllRequired { get; set; }

	/// <summary>
	/// Provádí se jako volání události <see cref="CollectionChanged"/>.
	/// </summary>
	/// <param name="e">prázdné</param>
	protected void OnCollectionChanged(EventArgs e)
	{
		if (CollectionChanged != null)
		{
			CollectionChanged(this, e);
		}
	}

	/// <summary>
	/// Urèuje, zda je možné do kolekce vložit hodnotu, která již v kolekci je.
	/// Pokud je nastaveno na true, přidání hodnoty, která v kolekci již je, vyvolá výjimku.
	/// Pokud je nastaveno na false (výchozí), je možné hodnotu do kolekce přidat vícekrát.
	/// </summary>
	public bool AllowDuplicates
	{
		get
		{
			return _allowDuplicates;
		}
		set
		{
			if (_allowDuplicates && !value)
			{
				if (CheckDuplicates())
				{
					throw new InvalidOperationException("Kolekce obsahuje duplicity.");
				}
			}
			_allowDuplicates = value;
		}
	}

	private bool _allowDuplicates = true;

	/// <summary>
	/// Inserts an element into the <see cref="T:System.Collections.ObjectModel.Collection`1"></see> at the specified index.
	/// When AllowDuplicates is false, checks whether item already is in the collection. If so, throws an ArgumentException.
	/// </summary>
	/// <param name="index">The zero-based index at which item should be inserted.</param>
	/// <param name="item">The object to insert. The value can be null for reference types.</param>
	/// <exception cref="T:System.ArgumentOutOfRangeException">index is less than zero.-or-index is greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"></see>.</exception>
	protected override void InsertItem(int index, TItem item)
	{
		ThrowIfFrozen();

		if ((!_allowDuplicates) && (this.Contains(item)))
		{
			throw new ArgumentException("Položka v kolekci již existuje (a není povoleno vkládání duplicit).");
		}

		base.InsertItem(index, item);

		if ((item != null) && !item.IsLoaded)
		{
			LoadAllRequired = true;
		}

		OnCollectionChanged(EventArgs.Empty);
	}

	/// <summary>
	/// Removes the element at the specified index of the <see cref="T:System.Collections.ObjectModel.Collection`1"></see>.
	/// </summary>
	/// <param name="index">The zero-based index of the element to remove.</param>
	/// <exception cref="T:System.ArgumentOutOfRangeException">index is less than zero.-or-index is equal to or greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"></see>.</exception>
	protected override void RemoveItem(int index)
	{
		ThrowIfFrozen();
		base.RemoveItem(index);
		OnCollectionChanged(EventArgs.Empty);
	}

	/// <summary>
	/// Replaces the element at the specified index.
	/// When AllowDuplicates is false, checks whether item already is in the collection. If so, throws an ArgumentException.
	/// </summary>
	/// <param name="index">The zero-based index of the element to replace.</param>
	/// <param name="item">The new value for the element at the specified index. The value can be null for reference types.</param>
	/// <exception cref="T:System.ArgumentOutOfRangeException">index is less than zero.-or-index is greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"></see>.</exception>
	protected override void SetItem(int index, TItem item)
	{
		ThrowIfFrozen();

		// je zajištěno, že v režimu !AllowDuplicates kolekce neobsahuje duplikáty
		// potom mùžeme použít IndexOf na hledání výskytu (je garantováno, že prvek je v kolekci nejvýše jednou).
		if (!_allowDuplicates && (this.IndexOf(item) != index))
		{
			throw new ArgumentException("Položka v kolekci již existuje (a není povoleno vkládání duplicit).");
		}

		base.SetItem(index, item);

		if ((item != null) && !item.IsLoaded)
		{
			LoadAllRequired = true;
		}

		OnCollectionChanged(EventArgs.Empty);
	}

	/// <summary>
	/// Removes all elements from the <see cref="T:System.Collections.ObjectModel.Collection`1"></see>.
	/// </summary>
	protected override void ClearItems()
	{
		ThrowIfFrozen();
		if (this.Count > 0)
		{
			base.ClearItems();
			LoadAllRequired = false;
			OnCollectionChanged(EventArgs.Empty);
		}
	}

	/// <summary>
	/// Vytvoří novou instanci kolekce bez prvků - prázdnou.
	/// </summary>
	/// <remarks>
	/// Použit je wrappující constructor Collection(Of T), abychom si vynutili List(Of T).
	/// </remarks>
	public BusinessObjectCollection()
		: base(new List<TItem>())
	{
		LoadAllRequired = false;
	}

	/// <summary>
	/// Vytvoří novou instanci kolekce wrapnutím Listu prvků (neklonuje!).
	/// </summary>
	/// <remarks>
	/// Je to rychlé! Nikam se nic nekopíruje, ale pozor, ani neklonuje!
	/// </remarks>
	/// <param name="list">List prvkù</param>
	public BusinessObjectCollection(List<TItem> list)
		: base(list)
	{
		LoadAllRequired = this.Any(item => (item != null) && !item.IsLoaded);
	}

	/// <summary>
	/// Vytvoří novou instanci kolekce a zkopíruje do ní prvky z předané kolekce.
	/// </summary>
	/// <param name="collection">kolekce, jejíž prvky se mají do naší kolekce zkopírovat</param>
	public BusinessObjectCollection(IEnumerable<TItem> collection)
		: base(new List<TItem>(collection))
	{
		if ((collection is BusinessObjectCollection<TItem, TCollection>) && !((BusinessObjectCollection<TItem, TCollection>)collection).LoadAllRequired)
		{
			LoadAllRequired = false; // pokud ani původní kolekci nebylo třeba načíst, příznak LoadAllRequired zůstává false.
		}
		else
		{
			LoadAllRequired = this.Any(item => (item != null) && !item.IsLoaded);
		}
	}

	/// <summary>
	/// Prohledá kolekci a vrátí první nalezený prvek s požadovaným ID.
	/// </summary>
	/// <remarks>
	/// Vzhledem k tomu, že jsou prvky v kolekci obvykle unikátní, najde prostě zadané ID.
	/// </remarks>
	/// <param name="id">ID prvku</param>
	/// <returns>první nalezený prvek s požadovaným ID; null, pokud nic nenalezeno</returns>
	public TItem FindByID(int id)
	{
		List<TItem> innerList = (List<TItem>)Items;
		TItem result = null;
		result = innerList.Find(delegate (TItem item)
							  {
								  if (item.ID == id)
								  {
									  return true;
								  }
								  else
								  {
									  return false;
								  }
							  });

		return result;
	}

	/// <summary>
	/// Prohledá kolekci a vrátí první nalezený prvek odpovídající kritériu match.
	/// </summary>
	/// <remarks>
	/// Metoda pouze publikuje metodu Find() inner-Listu Items.
	/// </remarks>
	/// <param name="match">kritérium ve formì predikátu</param>
	/// <returns>kolekce všech prvkù odpovídajících kritériu match</returns>
	public virtual TItem Find(Predicate<TItem> match)
	{
		List<TItem> innerList = (List<TItem>)Items;
		return innerList.Find(match);
	}

	/// <summary>
	/// Prohledá kolekci a vrátí všechny prvky odpovídající kritériu match.
	/// </summary>
	/// <remarks>
	/// Metoda pouze publikuje metodu FindAll() inner-listu Items.
	/// </remarks>
	/// <param name="match">kritérium ve formì predikátu</param>
	/// <returns>kolekce všech prvků odpovídajících kritériu match</returns>
	public virtual TCollection FindAll(Predicate<TItem> match)
	{
		//Contract.Ensures(Contract.Result<TCollection>() != null);
		List<TItem> innerList = (List<TItem>)Items;
		List<TItem> found = innerList.FindAll(match);
		TCollection result = new TCollection();
		result.AllowDuplicates = this.AllowDuplicates; // ???
		result.AddRange(found);
		return result;
	}

	/// <summary>
	/// Spustí akci nad všemi prvky kolekce.
	/// </summary>
	/// <example>
	/// orders.ForEach(delegate(Order item)
	///		{
	///			item.Delete();
	///		});
	/// </example>
	/// <remarks>
	/// Je rychlejší, než <c>foreach</c>, protože neprochází enumerator, ale iteruje prvky ve for cyklu podle indexu.
	/// </remarks>
	/// <param name="action">akce, která má být spuštìna</param>
	public void ForEach(Action<TItem> action)
	{
		List<TItem> innerList = (List<TItem>)Items;
		innerList.ForEach(action);
	}

	/// <summary>
	/// Přidá do kolekce prvky předané kolekce.
	/// </summary>
	/// <param name="source">Kolekce, jejíž prvky mají být přidány.</param>
	public void AddRange(IEnumerable<TItem> source)
	{
		List<TItem> innerList = (List<TItem>)Items;
		int originalItemsCount = innerList.Count;
		innerList.AddRange(source);

		// vyvoláme událost informující o zmìnì kolekce, pokud se zmìnil poèet objektù v kolekci
		if (originalItemsCount != innerList.Count)
		{
			if ((source is BusinessObjectCollection<TItem, TCollection>) && !((BusinessObjectCollection<TItem, TCollection>)source).LoadAllRequired)
			{
				// NOOP - pokud ani původní kolekci nebylo třeba načíst, nemění se příznak LoadAllRequired.
			}
			else
			{
				if (!LoadAllRequired && source.Any(item => (item != null) && !item.IsLoaded)) /* optimalizujeme volání Any jen tehdy, když dosud není nastaven příznak LoadAllRequired */
				{
					LoadAllRequired = true;
				}
			}
			OnCollectionChanged(EventArgs.Empty);
		}
	}

	/// <summary>
	/// Odstraní z kolekce všechny prvky odpovídající kritériu match.
	/// </summary>
	/// <remarks>
	/// Metoda pouze publikuje metodu RemoveAll() inner-listu Items.
	/// </remarks>
	/// <param name="match">kritérium ve formì predikátu</param>
	/// <returns>poèet odstranìných prvkù</returns>
	public virtual int RemoveAll(Predicate<TItem> match)
	{
		List<TItem> innerList = (List<TItem>)Items;
		int itemsRemovedCount = innerList.RemoveAll(match);

		// vyvoláme událost informující o zmìnì kolekce, pokud nìjaké objekty byly z kolekce odstranìny
		if (itemsRemovedCount != 0)
		{
			OnCollectionChanged(EventArgs.Empty);
		}

		return itemsRemovedCount;
	}

	/// <summary>
	/// Odstraní z kolekce požadované prvky.
	/// </summary>
	/// <param name="items">prvky, které mají být z kolekce odstranìny</param>
	/// <returns>poèet prvkù, které byly skuteènì odstranìny</returns>
	public virtual int RemoveRange(IEnumerable<TItem> items)
	{
		List<TItem> innerList = (List<TItem>)Items;
		if (items == null)
		{
			throw new ArgumentNullException("items");
		}

		int count = 0;
		foreach (TItem item in items)
		{
			if (this.Remove(item))
			{
				count++;
			}
		}
		return count;
	}

	/// <summary>
	/// Seřadí prvky kolekce dle požadované property, která implementuje IComparable.
	/// </summary>
	/// <remarks>
	/// Používá <see cref="Havit.Collections.GenericPropertyComparer{T}"/>. K porovnávání podle property
	/// tedy dochází pomocí reflexe - relativnì pomalu. Pokud je potřeba vyšší výkon, je potřeba použít
	/// overload Sort(Generic Comparsion) s přímým přístupem k property.
	/// </remarks>
	/// <param name="propertyName">property, podle které se má řadit</param>
	/// <param name="ascending">true, pokud se má řadit vzestupnì, false, pokud sestupnì</param>
	[Obsolete]
	public virtual void Sort(string propertyName, bool ascending)
	{
		List<TItem> innerList = (List<TItem>)Items;
		innerList.Sort(new GenericPropertyComparer<TItem>(new SortItem(propertyName, ascending ? SortDirection.Ascending : SortDirection.Descending)));
	}

	/// <summary>
	/// Seřadí prvky kolekce dle požadované property, která implementuje IComparable.
	/// </summary>
	/// <remarks>
	/// Používá <see cref="Havit.Collections.GenericPropertyComparer{T}"/>. K porovnávání podle property
	/// tedy dochází pomocí reflexe - relativnì pomalu. Pokud je potřeba vyšší výkon, je potřeba použít
	/// overload Sort(Generic Comparsion) s přímým přístupem k property.
	/// </remarks>
	/// <param name="propertyInfo">Property, podle které se má řadit.</param>
	/// <param name="sortDirection">Smìr řazení.</param>
	public virtual void Sort(PropertyInfo propertyInfo, SortDirection sortDirection)
	{
		List<TItem> innerList = (List<TItem>)Items;
		innerList.Sort(new GenericPropertyComparer<TItem>(new SortItem(propertyInfo.PropertyName, sortDirection)));
	}

	/// <summary>
	/// Seřadí prvky kolekce dle zadaného srovnání. Publikuje metodu Sort inner-Listu.
	/// </summary>
	/// <param name="comparsion">Srovnání, podle kterého mají být prvky seřazeny.</param>
	public virtual void Sort(Comparison<TItem> comparsion)
	{
		List<TItem> innerList = (List<TItem>)Items;
		innerList.Sort(comparsion);
	}

	/// <summary>
	/// Seřadí prvky kolekce dle zadaného srovnání. Publikuje metodu Sort inner-Listu.
	/// </summary>
	/// <param name="comparer">Comparer, podle kterého mají být prvky seřazeny.</param>
	public virtual void Sort(IComparer<TItem> comparer)
	{
		List<TItem> innerList = (List<TItem>)Items;
		innerList.Sort(comparer);
	}

	/// <summary>
	/// Uloží všechny prvky kolekce, v transakci (pokud je null, založí si samo novou).
	/// </summary>
	/// <param name="transaction">transakce <see cref="DbTransaction"/>, v které mají být prvky uloženy</param>
	public virtual void SaveAll(DbTransaction transaction)
	{
		DbConnector.Default.ExecuteTransaction(innerTransaction =>
			{
				ForEach(delegate (TItem item)
					{
						item.Save(innerTransaction);
					});
			}, transaction);
	}

	/// <summary>
	/// Uloží všechny prvky kolekce (v transakci, kterou si samo vytvoří).
	/// </summary>
	public virtual void SaveAll()
	{
		SaveAll(null);
	}

	/// <summary>
	/// Zamkne kolekci vůči změnám. Od toho okamžiku není možné změnit položky v kolekci.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public void Freeze()
	{
		isFrozen = true;
	}
	private bool isFrozen = false;

	/// <summary>
	/// Pokud je nastaven příznak isFrozen, vyhodí výjimku InvalidOperationException.
	/// </summary>
	private void ThrowIfFrozen()
	{
		if (this.isFrozen)
		{
			throw new InvalidOperationException("Kolekce je zamčena, nelze ji modifikovat.");
		}
	}

	/// <summary>
	/// Vrátí pole hodnot ID všech prvkù kolekce.
	/// </summary>
	/// <returns>pole hodnot ID všech prvkù kolekce</returns>
	public int[] GetIDs()
	{
		int[] array = new int[this.Count];
		List<TItem> innerList = (List<TItem>)Items;
		for (int i = 0; i < innerList.Count; i++)
		{
			array[i] = innerList[i].ID;
		}
		return array;
	}

	/***********************************************************************/

	/// <summary>
	/// Nastaví kontrolu duplikátu (ale na rozdíl od nastaveí vlastnosti AllowDuplicated neprovede kontrolu duplicit).
	/// </summary>
	[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
	public void DisallowDuplicatesWithoutCheckDuplicates()
	{
		_allowDuplicates = false;
	}

	/// <summary>
	/// Vrací true, pokud kolekce obsahuje duplicity.
	/// </summary>		
	private bool CheckDuplicates()
	{
		// obsahuje-li kolekce ménì než dva prvky, nemùže obsahovat duplicity.
		if (Items.Count < 2)
		{
			return false;
		}

		// otestujeme duplicity uložených objektù
		List<TItem> savedObjects = this.Where(item => !item.IsNew).ToList();
		int distinctSavedObjectsCount = 0;
		if (savedObjects.Count > 1) // kolekce obsahuje uložený objekt
		{
			distinctSavedObjectsCount = savedObjects.Select(item => item.ID).Distinct().Count();
			if ((savedObjects.Count > 1) && (distinctSavedObjectsCount != savedObjects.Count))
			{
				return true;
			}
		}

		if (distinctSavedObjectsCount != this.Count) // kolekce obsahuje i neuložený objekt
		{
			// otestujeme duplicity neuložených objektù
			List<TItem> newObjects = this.Where(item => item.IsNew).ToList();
			if (newObjects.Count > 1)
			{
				int distinctNewObjectsCount = newObjects.Distinct().Count();
				if (distinctNewObjectsCount != newObjects.Count)
				{
					return true;
				}
			}
		}

		// nenašli jsme duplicitu
		return false;
	}

	[SuppressMessage("Havit.StyleCop.Rules.HavitRules", "HA0002:MembersOrder", Justification = "Související kód ohledně readonly kolekcí je pohromadě.")]
	bool ICollection<TItem>.IsReadOnly
	{
		get
		{
			return isFrozen;
		}
	}

	/***********************************************************************/

	void IDataBinderExtSetValue.SetValue(object value)
	{
		if (value == null)
		{
			this.Clear();
		}
		else if (value is IEnumerable<BusinessObjectBase>)
		{
			this.Clear();
			this.AddRange(((IEnumerable<BusinessObjectBase>)value).Cast<TItem>().ToList());
		}
		else if (value is IEnumerable<TItem>)
		{
			this.Clear();
			this.AddRange((IEnumerable<TItem>)value);
		}
		else if (value is TItem)
		{
			this.Clear();
			this.Add((TItem)value);
		}
		else
		{
			throw new NotSupportedException("Nepodařilo se nastavit hodnotu.");
		}
	}
}
