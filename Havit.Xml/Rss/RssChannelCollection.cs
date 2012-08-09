using System;
using System.Collections;

namespace Havit.Xml.Rss
{
	/// <summary>
	/// Kolekce RssChannel prvků.
	/// </summary>
	public class RssChannelCollection : CollectionBase
	{
		/// <summary>
		/// Přidá nový kanál do kolekce
		/// </summary>
		/// <param name="channel">kanál k přidání</param>
		public virtual void Add(RssChannel channel)
		{
			this.List.Add(channel);
		}

		/// <summary>
		/// Zkopíruje kanály do array, na pozici index
		/// </summary>
		/// <param name="array">cílové pole</param>
		/// <param name="index">pozice v cílovém poli</param>
		public void CopyTo(RssChannel[] array, int index)
		{
			((ICollection)this).CopyTo(array, index);
		}

		/// <summary>
		/// Vloží na určené místo v kolekci kanál.
		/// </summary>
		/// <param name="index">pozice</param>
		/// <param name="value">kanál</param>
		public void Insert(int index, RssChannel value) 
		{
			((IList)this).Insert(index, value);
		}        

		/// <summary>
		/// Vyhledá pozici kanálu v kolekci
		/// </summary>
		/// <param name="value">kanák</param>
		/// <returns>pozice kanálu v kolekci</returns>
		public int IndexOf(RssChannel value) 
		{
			return ((IList)this).IndexOf(value);
		}

		/// <summary>
		/// Odstraní kanál z kolekce.
		/// </summary>
		/// <param name="value">kanál k odstranění</param>
		public void Remove(RssChannel value) 
		{
			((IList)this).Remove( value);
		}

		/// <summary>
		/// Zjistí, zda-li se kanál nachází v kolekci
		/// </summary>
		/// <param name="value">kanál</param>
		/// <returns>true, pokud již kanál v kolekci existuje, jinak false</returns>
		public bool Contains(RssChannel value) 
		{
			return ((IList)this).Contains( value);
		}

		/// <summary>
		/// Indexer zpřístupňující kanál na dané pozici kolekce.
		/// </summary>
		public virtual RssChannel this[int index]
		{
			get
			{
				return (RssChannel)this.List[index];
			}
		}
	}
}
