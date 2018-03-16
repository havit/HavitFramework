namespace Havit.Data.EFCore.Tests.ModelValidation.Infrastructure.Model
{
	public class NavigationPropertyWithoutForeignKeyClass
	{
		public int Id { get; set; }

		public NavigationPropertyWithoutForeignKeyClass NavigationProperty { get; set; }
	}
}
