using System;
using System.Collections.Generic;
using System.Reflection;
using Havit.Services.Caching;

namespace Havit.Business.CodeMigrations.ExtendedProperties.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public class CacheAttribute : ExtendedPropertiesAttribute
	{
		public bool SuppressPreload { get; set; }

		public CacheItemPriority Priority { get; set; }

		public int AbsoluteExpiration { get; set; }

		public int SlidingExpiration { get; set; }

		public override IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo) => new Dictionary<string, string>
			{
				{ "Cache", "true" }
			}.AddIfNotDefault("Cache_Priority", Priority)
			.AddIfNotDefault("Cache_AbsoluteExpiration", AbsoluteExpiration)
			.AddIfNotDefault("Cache_SlidingExpiration", SlidingExpiration);
	}
}