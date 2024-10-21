using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;

namespace Havit.Data.EntityFrameworkCore.Patterns.Lookups;

/// <summary>
/// Zajistí invalidaci lookup služeb všechny změněné objekty.
/// </summary>
public class LookupDataInvalidationRunner : ILookupDataInvalidationRunner
{
	private readonly IEnumerable<ILookupDataInvalidationService> lookupInvalidationServices;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public LookupDataInvalidationRunner(IEnumerable<ILookupDataInvalidationService> lookupInvalidationServices)
	{
		this.lookupInvalidationServices = lookupInvalidationServices;
	}

	/// <summary>
	/// Zajistí  invalidaci lookup služeb všechny změněné objekty.
	/// </summary>
	public void Invalidate(Changes allKnownChanges)
	{
		// TODO EF Core 9: vícero iterování enumerable (+ toto navíc)!
		if (lookupInvalidationServices.Any())
		{
			foreach (var lookupService in lookupInvalidationServices)
			{
				lookupService.InvalidateAfterCommit(allKnownChanges);
			}
		}
	}
}
