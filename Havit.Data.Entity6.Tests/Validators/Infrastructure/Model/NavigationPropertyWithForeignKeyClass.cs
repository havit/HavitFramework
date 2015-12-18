namespace Havit.Data.Entity.Tests.Validators.Infrastructure.Model
{
	public class NavigationPropertyWithForeignKeyClass
	{
		public int Id { get; set; }

		public NavigationPropertyWithForeignKeyClass NavigationProperty { get; set; }
		public int NavigationPropertyId { get; set; }
	}
}
