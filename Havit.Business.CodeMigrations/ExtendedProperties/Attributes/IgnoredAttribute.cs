using System.Collections.Generic;
using System.Reflection;

namespace Havit.Business.CodeMigrations.ExtendedProperties.Attributes
{
	public class IgnoredAttribute : ExtendedPropertiesAttribute
	{
		public override IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo) => new Dictionary<string, string>()
		{
			{ "Ignored", "true" },
		};
	}
}