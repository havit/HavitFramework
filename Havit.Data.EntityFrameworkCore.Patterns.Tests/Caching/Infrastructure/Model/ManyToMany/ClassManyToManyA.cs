namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure.Model.ManyToMany;

public class ClassManyToManyA
{
	public int Id { get; set; }

	public List<ClassManyToManyB> Items { get; set; }
}
