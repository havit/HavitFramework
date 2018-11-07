using System;
using System.Collections.Generic;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes.ExtendedProperties
{
	/// <summary>
	/// ExtendedProperties pro řízení cache business objektů (Cache = true, Cache_Priority, Cache_AbsoluteExpiration, Cache_SlidingExpiration).
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class CacheAttribute : ExtendedPropertiesAttribute
	{
		/// <summary>
		/// Potlačení přednačtení položek třídy do cache.
		/// </summary>
		public bool SuppressPreload { get; set; } = false;

		/// <summary>
		/// Priorita položek v cache.
		/// </summary>
		public CacheItemPriority Priority { get; set; } = CacheItemPriority.Normal;

		/// <summary>
		/// Absolutní expirace položek v cache.
		/// </summary>
		public int AbsoluteExpiration { get; set; } = 0;

		/// <summary>
		/// Sliding expirace položek v cache.
		/// </summary>
		public int SlidingExpiration { get; set; } = 0;

		/// <inheritdoc />
		public override IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo) => new Dictionary<string, string>
			{
				{ "Cache", "true" }
			}
			.AddIfNotDefault("Cache_Priority", Priority, CacheItemPriority.Normal)
			.AddIfNotDefault("Cache_SuppressPreload", SuppressPreload, false)
			.AddIfNotDefault("Cache_AbsoluteExpiration", AbsoluteExpiration, 0)
			.AddIfNotDefault("Cache_SlidingExpiration", SlidingExpiration, 0);
	}
}