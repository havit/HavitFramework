﻿using Havit.Data.Patterns.DataLoaders;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;

/// <summary>
/// Interní použití v DbDataLoaderu.
/// </summary>
internal struct LoadPropertyInternalResult
{
	public IEnumerable Entities { get; set; }
	public object FluentDataLoader { get; set; }
}