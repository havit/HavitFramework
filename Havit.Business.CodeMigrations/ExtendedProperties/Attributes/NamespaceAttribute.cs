using System.Collections.Generic;
using System.Reflection;

namespace Havit.Business.CodeMigrations.ExtendedProperties.Attributes
{
	public class NamespaceAttribute : ExtendedPropertiesAttribute
	{
		public const string PropertyName = "Namespace";

		public override IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo) => null;
	}
}