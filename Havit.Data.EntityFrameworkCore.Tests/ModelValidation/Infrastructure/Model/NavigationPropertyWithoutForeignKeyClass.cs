namespace Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure.Model
{
	public class NavigationPropertyWithoutForeignKeyClass
	{
		public int Id { get; set; }

		public NavigationPropertyWithoutForeignKeyClass NavigationProperty { get; set; }
	}
}
