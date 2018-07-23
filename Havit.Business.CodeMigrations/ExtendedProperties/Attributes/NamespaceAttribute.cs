using System.Collections.Generic;

namespace Havit.Business.CodeMigrations.ExtendedProperties.Attributes
{
	public class NamespaceAttribute : ExtendedPropertiesAttribute
	{
		public const string PropertyName = "Namespace";
		
		public override IDictionary<string, string> ExtendedProperties => null;
	}
}