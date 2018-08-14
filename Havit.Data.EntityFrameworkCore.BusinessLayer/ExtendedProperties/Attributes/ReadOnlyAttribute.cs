using System.Collections.Generic;
using System.Reflection;

namespace Havit.Business.CodeMigrations.ExtendedProperties.Attributes
{
	public class ReadOnlyAttribute : ExtendedPropertiesAttribute
	{
		public override IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo) => new Dictionary<string, string>
		{
			{ "ReadOnly", "true" }
		};
	}
}