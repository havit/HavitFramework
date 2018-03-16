namespace Havit.Data.EFCore.Tests.ModelValidation.Infrastructure.Model
{
	public class NavigationPropertyWithForeignKeyClass
	{
		public int Id { get; set; }

		public NavigationPropertyWithForeignKeyClass NavigationProperty { get; set; }
		public int NavigationPropertyId { get; set; }
	}
}
