using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
		if (lookupInvalidationServices.Any())
		{
			foreach (var lookupService in lookupInvalidationServices)
			{
				lookupService.InvalidateAfterCommit(allKnownChanges);
			}
		}
	}
}
