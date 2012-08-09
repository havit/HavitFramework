using System;
using System.Collections.Generic;
using System.Text;
using Havit.Collections;

namespace Havit.Business.Query
{
	/// <summary>
	/// Reprezentuje položku øazení.
	/// </summary>
	[Serializable]
	public class FieldPropertySortItem: SortItem
	{
		#region Constructor (obsolete)
		/// <summary>
		/// Vytvoøí nenastavenou položku øazení podle.
		/// </summary>
		[Obsolete]
		public FieldPropertySortItem()
			: base()
		{
		}
		
		#endregion

		#region Constructors
		/// <summary>
		/// Vytvoøí položku øazení podle sloupce, vzestupné poøadí.
		/// </summary>
		public FieldPropertySortItem(FieldPropertyInfo property)
			: this(property, SortDirection.Ascending)
		{
		}
		
		/// <summary>
		/// Vytvoøí položku øazení podle sloupce a daného poøadí.
		/// </summary>
		public FieldPropertySortItem(FieldPropertyInfo property, SortDirection direction)
			: base(property.FieldName, direction)
		{
		}
		#endregion
	}
}
