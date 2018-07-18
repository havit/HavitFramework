using Havit.Business.CodeMigrations.ExtendedProperties;

namespace Havit.Business.CodeMigrations.Tests.ExtendedProperties
{
	internal class TestExtendedPropertyAttribute : ExtendedPropertyAttribute
	{
		public override string Name { get; }
		public override string Value { get; }

		public TestExtendedPropertyAttribute(string name, string value)
		{
			Name = name;
			Value = value;
		}
	}
}
