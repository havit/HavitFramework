namespace Havit.Data.Entity.Tests.ModelValidation.Infrastructure.Model
{
	public class NavigationPropertyWithoutForeignKeyClass
	{
		public int Id { get; set; }

		public NavigationPropertyWithoutForeignKeyClass NavigationProperty { get; set; }
	}
}
