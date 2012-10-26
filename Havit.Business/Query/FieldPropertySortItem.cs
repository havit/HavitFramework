using System;
using System.Collections.Generic;
using System.Text;
using Havit.Collections;

namespace Havit.Business.Query
{
	/// <summary>
	/// Reprezentuje položku řazení.
	/// </summary>
	[Serializable]
	public class FieldPropertySortItem : SortItem
	{
		#region Constructor (obsolete)
		/// <summary>
		/// Vytvoří nenastavenou položku řazení podle.
		/// </summary>
		[Obsolete]
		public FieldPropertySortItem()
			: base()
		{
		}
		
		#endregion

		#region Constructors
		/// <summary>
		/// Vytvoří položku řazení podle sloupce, vzestupné pořadí.
		/// </summary>
		public FieldPropertySortItem(FieldPropertyInfo property)
			: this(property, SortDirection.Ascending)
		{
		}
		
		/// <summary>
		/// Vytvoří položku řazení podle sloupce a daného pořadí.
		/// </summary>
		public FieldPropertySortItem(FieldPropertyInfo property, SortDirection direction)
			: base("[" + property.FieldName + "]", direction)
		{
		}
		#endregion
	}
}
