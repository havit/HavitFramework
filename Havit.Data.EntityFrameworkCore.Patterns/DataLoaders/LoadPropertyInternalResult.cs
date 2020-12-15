using System;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataLoaders
{
	/// <summary>
	/// Interní použití v DbDataLoaderu.
	/// </summary>
	internal struct LoadPropertyInternalResult
	{
		public Array Entities { get; set; }
		public object FluentDataLoader { get; set; }
	}
}