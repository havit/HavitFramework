using System;
using System.Collections.Generic;

namespace Havit.Business.CodeMigrations.ExtendedProperties.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public class CacheAttribute : ExtendedPropertiesAttribute
	{
		public override IDictionary<string, string> ExtendedProperties => new Dictionary<string, string>
		{
			{ "Cache", "true" }
		};
	}
}