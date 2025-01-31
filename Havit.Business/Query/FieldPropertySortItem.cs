using Havit.Collections;

namespace Havit.Business.Query;

/// <summary>
/// Reprezentuje položku řazení.
/// </summary>
public class FieldPropertySortItem : SortItem
{
	/// <summary>
	/// Vytvoří nenastavenou položku řazení podle.
	/// </summary>
	[Obsolete]
	public FieldPropertySortItem()
	{
	}

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
}
