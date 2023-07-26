namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Internal.Infrastructure.Model.ManyToMany;

public class ClassManyToManyA
{
	public int Id { get; set; }

	public List<ClassManyToManyB> Items { get; set; }
}
