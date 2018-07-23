using System.Collections.Generic;

namespace Havit.Business.CodeMigrations.ExtendedProperties.Attributes
{
	public class NamespaceAttribute : ExtendedPropertyAttribute
	{
		private readonly string _name;

		public NamespaceAttribute(string name)
		{
			_name = name;
		}

		public override IDictionary<string, string> ExtendedProperties => new Dictionary<string, string>()
		{
			{ "Namespace", _name },
		};
	}
}