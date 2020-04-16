using System;
using System.Collections.Generic;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching
{
	/// <summary>
	/// Úložiště informací k rozhodnutí o cachování entit.
	/// </summary>
	public interface IAnnotationsEntityCacheSupportDecisionStorage
	{
		/// <summary>
		/// Indikuje ke každému typu, zda má cachovat entity.
		/// </summary>
		Dictionary<Type, bool> ShouldCacheEntities { get; set; }

		/// <summary>
		/// Indikuje ke každému typu, zda má cachovat "all keys".
		/// </summary>
		Dictionary<Type, bool> ShouldCacheAllKeys { get; set; }
	}
}
