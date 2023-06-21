using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;

namespace Havit.Data.EntityFrameworkCore.Patterns.Lookups;

/// <summary>
/// Zajistí  invalidaci lookup služeb všechny změněné objekty.
/// </summary>
public interface ILookupDataInvalidationRunner
{
	/// <summary>
	/// Zajistí  invalidaci lookup služeb všechny změněné objekty.
	/// </summary>
	void Invalidate(Changes allKnownChanges);
}