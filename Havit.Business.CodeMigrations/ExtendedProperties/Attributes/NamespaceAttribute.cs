namespace Havit.Business.CodeMigrations.ExtendedProperties.Attributes
{
	public class NamespaceAttribute : ExtendedPropertyAttribute
	{
		public NamespaceAttribute(string name)
		{
			Value = name;
		}

		public override string Name => "Namespace";

		public override string Value { get; }
	}
}