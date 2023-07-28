namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure.Model.OneToOne;

public class ClassOneToOneB
{
	public int Id { get; set; }

	public ClassOneToOneA ClassA { get; set; }
	public int ClassAId { get; set; }
}
