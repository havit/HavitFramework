namespace Havit.Data.Entity.Tests.Validators.Infrastructure.Model
{
	public class NavigationPropertyWithoutForeignKeyClass
	{
		public int Id { get; set; }

		public NavigationPropertyWithoutForeignKeyClass NavigationProperty { get; set; }
	}
}
