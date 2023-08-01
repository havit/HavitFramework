using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;

/// <summary>
/// Typ změny.
/// </summary>
public enum ChangeType
{
	/// <summary>
	/// Insert
	/// </summary>
	[DebuggerDisplay(nameof(Insert))]
	Insert = EntityState.Added,

	/// <summary>
	/// Update
	/// </summary>
	[DebuggerDisplay(nameof(Update))]
	Update = EntityState.Modified,

	/// <summary>
	/// Delete
	/// </summary>
	[DebuggerDisplay(nameof(Delete))]
	Delete = EntityState.Deleted
}
