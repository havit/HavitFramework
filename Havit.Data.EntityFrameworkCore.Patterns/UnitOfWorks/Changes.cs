using System.Collections;

namespace Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;

/// <summary>
/// Změny objektů v UnitOfWork.
/// </summary>
public class Changes : IEnumerable<Change>
{
	private List<Change> _changes;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public Changes(IEnumerable<Change> changes)
	{
		_changes = changes.ToList();
	}

	#region IEnumerable<Change> implementation
	IEnumerator<Change> IEnumerable<Change>.GetEnumerator() => _changes.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => _changes.GetEnumerator();
	#endregion
}