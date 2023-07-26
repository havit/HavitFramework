namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Internal.Infrastructure.Model.OneToMany;

public class Master
{
	public int Id { get; set; }

	public List<Child> Children { get; set; }
}
