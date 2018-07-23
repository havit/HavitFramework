using System.Collections.Generic;
using Havit.Business.CodeMigrations.ExtendedProperties;

namespace Havit.Business.CodeMigrations.Tests.ExtendedProperties
{
	internal class TestExtendedPropertyAttribute : ExtendedPropertiesAttribute
	{
		private readonly IDictionary<string, string> _extendedProperties;

		private TestExtendedPropertyAttribute()
		{
			_extendedProperties = new Dictionary<string, string>();
		}

		public TestExtendedPropertyAttribute(string name, string value)
			: this()
		{
			_extendedProperties.Add(name, value);
		}

		public TestExtendedPropertyAttribute(string name1, string value1, string name2, string value2)
			: this()
		{
			_extendedProperties.Add(name1, value1);
			_extendedProperties.Add(name2, value2);
		}

		public override IDictionary<string, string> ExtendedProperties => _extendedProperties;
	}
}
