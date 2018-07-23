namespace Havit.Business.CodeMigrations.ExtendedProperties.Attributes
{
	public class IgnoredAttribute : ExtendedPropertyAttribute
	{
		public override string Name => "Ignored";

		public override string Value => "true";
	}
}