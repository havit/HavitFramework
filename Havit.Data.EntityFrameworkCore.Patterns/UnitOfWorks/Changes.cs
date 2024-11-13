namespace Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;

/// <summary>
/// Změny objektů v UnitOfWork.
/// </summary>
public class Changes
{
	/// <summary>
	/// Seznam změn.
	/// </summary>
	/// <remarks>Oproti dřívější implementaci, kdy Changes samotné bylo IEnumerable, se takto v toolingu lépe hledají enumerace změn.</remarks>
	public List<Change> Items { get; }

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public Changes(List<Change> items)
	{
		Items = items.ToList();
	}

	/// <summary>
	/// Seznam změn seskupený podle CLR typu.
	/// Záznamy s CLR typem null nejsou obsaženy.
	/// </summary>
	public ILookup<Type, Change> GetChangesByClrType()
	{
		return _itemsGroupByClrType ??= Items
			.Where(change => change.ClrType != null)
			.ToLookup(change => change.ClrType);
	}
	private ILookup<Type, Change> _itemsGroupByClrType;
}