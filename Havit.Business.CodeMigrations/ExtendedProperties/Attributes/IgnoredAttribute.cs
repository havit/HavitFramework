using System.Collections.Generic;

namespace Havit.Business.CodeMigrations.ExtendedProperties.Attributes
{
	public class IgnoredAttribute : ExtendedPropertyAttribute
	{
		public override IDictionary<string, string> ExtendedProperties => new Dictionary<string, string>()
		{
			{ "Ignored", "true" },
		};
	}
}