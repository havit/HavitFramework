using System.Collections.Generic;

namespace Havit.Business.CodeMigrations.ExtendedProperties.Attributes
{
	public class ReadOnlyAttribute : ExtendedPropertiesAttribute
	{
		public override IDictionary<string, string> ExtendedProperties => new Dictionary<string, string>
		{
			{ "ReadOnly", "true" }
		};
	}
}