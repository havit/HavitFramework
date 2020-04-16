using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching
{
	/// <summary>
	/// Mapování typu entity na klíč v cache.
	/// </summary>
	public class EntityCacheKeyGeneratorStorage : IEntityCacheKeyGeneratorStorage
	{
		/// <summary>
		/// Mapování typu entity na klíč v cache.
		/// </summary>
		public Dictionary<Type, string> Value { get; set; }
	}
}
