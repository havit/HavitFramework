using System.Collections.Generic;

namespace Havit.Business.CodeMigrations.ExtendedProperties.Attributes
{
	public class IgnoredAttribute : ExtendedPropertiesAttribute
	{
		public override IDictionary<string, string> ExtendedProperties => new Dictionary<string, string>()
		{
			{ "Ignored", "true" },
		};
	}
}