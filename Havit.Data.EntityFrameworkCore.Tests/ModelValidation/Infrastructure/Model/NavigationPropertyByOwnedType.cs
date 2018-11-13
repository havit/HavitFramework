namespace Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure.Model
{
	public class NavigationPropertyByOwnedType
	{
		public int Id { get; set; }

		public OwnedType Owned { get; set; }
	}
}
