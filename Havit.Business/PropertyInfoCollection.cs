using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Havit.Business
{
	/// <summary>
	/// Kolekce objektù PropertyInfo.<br/>
	/// Pøi opakovaném pøidání property do kolekce se nic nestane (tj. 
	/// property nebude do kolekce pøidána podruhé a nedojde k chybì).
	/// </summary>
	[Serializable]
	public class PropertyInfoCollection : Collection<PropertyInfo>
	{
		/// <summary>
		/// Pøidá prvek do kolekce, pokud v kolekci již není.
		/// </summary>
		protected override void InsertItem(int index, PropertyInfo item)
		{
			if (this.Contains(item))
				return;

			base.InsertItem(index, item);
		}
	}
}
