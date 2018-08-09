using System.Collections.Generic;
using System.Reflection;

namespace Havit.Business.CodeMigrations.ExtendedProperties.Attributes
{
	public class IgnoredAttribute : ExtendedPropertiesAttribute
	{
		public static readonly string PropertyName = "Ignored";

		public override IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo) => new Dictionary<string, string>()
		{
			{ PropertyName, "true" },
		};
	}
}