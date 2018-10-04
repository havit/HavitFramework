﻿using System;
using System.Collections;

namespace Havit.Services.Xml.Rss
{
	/// <summary>
	/// Kolekce RssChannel prvků.
	/// </summary>
	public class RssChannelCollection : CollectionBase
	{
		#region Add
		/// <summary>
		/// Přidá nový kanál do kolekce
		/// </summary>
		/// <param name="channel">kanál k přidání</param>
		public virtual void Add(RssChannel channel)
		{
			this.List.Add(channel);
		}
		#endregion

		#region CopyTo
		/// <summary>
		/// Zkopíruje kanály do array, na pozici index
		/// </summary>
		/// <param name="array">cílové pole</param>
		/// <param name="index">pozice v cílovém poli</param>
		public void CopyTo(RssChannel[] array, int index)
		{
			((ICollection)this).CopyTo(array, index);
		}
		#endregion

		#region Insert
		/// <summary>
		/// Vloží na určené místo v kolekci kanál.
		/// </summary>
		/// <param name="index">pozice</param>
		/// <param name="value">kanál</param>
		public void Insert(int index, RssChannel value)
		{
			((IList)this).Insert(index, value);
		}
		#endregion        

		#region IndexOf
		/// <summary>
		/// Vyhledá pozici kanálu v kolekci
		/// </summary>
		/// <param name="value">kanák</param>
		/// <returns>pozice kanálu v kolekci</returns>
		public int IndexOf(RssChannel value)
		{
			return ((IList)this).IndexOf(value);
		}
		#endregion

		#region Remove
		/// <summary>
		/// Odstraní kanál z kolekce.
		/// </summary>
		/// <param name="value">kanál k odstranění</param>
		public void Remove(RssChannel value)
		{
			((IList)this).Remove(value);
		}
		#endregion

		#region Contains
		/// <summary>
		/// Zjistí, zdali se kanál nachází v kolekci
		/// </summary>
		/// <param name="value">kanál</param>
		/// <returns>true, pokud již kanál v kolekci existuje, jinak false</returns>
		public bool Contains(RssChannel value)
		{
			return ((IList)this).Contains(value);
		}
		#endregion

		#region Indexer this[int]
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
		#endregion
	}
}
