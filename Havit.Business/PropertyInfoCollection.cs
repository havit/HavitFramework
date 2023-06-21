using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Havit.Business;

/// <summary>
/// Kolekce objektů PropertyInfo.<br/>
/// Při opakovaném přidání property do kolekce se nic nestane (tj. 
/// property nebude do kolekce přidána podruhé a nedojde k chybě).
/// </summary>
public class PropertyInfoCollection : Collection<PropertyInfo>
{
	/// <summary>
	/// Vyvoří prázdnou kolekci.
	/// </summary>
	public PropertyInfoCollection()
	{
	}

	/// <summary>
	/// Vytvoří kolekci a vloží do ní zadané objekty PropertyInfo.
	/// </summary>
	public PropertyInfoCollection(params PropertyInfo[] properties)
		: this()
	{
		foreach (PropertyInfo propertyInfo in properties)
		{
			this.Add(propertyInfo);
		}
	}

	/// <summary>
	/// Přidá prvek do kolekce, pokud v kolekci již není.
	/// </summary>
	protected override void InsertItem(int index, PropertyInfo item)
	{
		if (this.Contains(item))
		{
			return;
		}

		base.InsertItem(index, item);
	}
}
