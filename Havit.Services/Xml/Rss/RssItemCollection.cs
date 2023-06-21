using System.Collections;

namespace Havit.Services.Xml.Rss;

/// <summary>
/// Kolekce RssItem prvků.
/// </summary>
public class RssItemCollection : CollectionBase
{
	/// <summary>
	/// Přidá nový item do kolekce
	/// </summary>
	/// <param name="newItem">nový item</param>
	public virtual void Add(RssItem newItem)
	{
		this.List.Add(newItem);
	}

	/// <summary>
	/// Vykopíruje prvky kolekce do pole.
	/// </summary>
	/// <param name="array">cílové pole</param>
	/// <param name="index">startovní pozice v cílovém poli</param>
	public void CopyTo(RssItem[] array, int index)
	{
		((ICollection)this).CopyTo(array, index);
	}

	/// <summary>
	/// Vloží prvek do kolekce na zadanou pozici.
	/// </summary>
	/// <param name="index">pozice</param>
	/// <param name="value">prvek</param>
	public void Insert(int index, RssItem value)
	{
		((IList)this).Insert(index, value);
	}

	/// <summary>
	/// Zjistí pozici prvku v kolekci
	/// </summary>
	/// <param name="value">hledný prvek</param>
	/// <returns>pozice v kolekci</returns>
	public int IndexOf(RssItem value)
	{
		return ((IList)this).IndexOf(value);
	}

	/// <summary>
	/// Odebere prvke z kolekce.
	/// </summary>
	/// <param name="value">prvek k odebrání</param>
	public void Remove(RssItem value)
	{
		((IList)this).Remove(value);
	}

	/// <summary>
	/// Zjistí, zdali je prvek v kolekci.
	/// </summary>
	/// <param name="value">prvek</param>
	/// <returns>true, je-li prvek v kolekci, jinak false</returns>
	public bool Contains(RssItem value)
	{
		return ((IList)this).Contains(value);
	}

	/// <summary>
	/// Indexer na kolekci zpřístupňující prvek na požadované pozici.
	/// </summary>
	public virtual RssItem this[int index]
	{
		get
		{
			return (RssItem)this.List[index];
		}
	}
}
