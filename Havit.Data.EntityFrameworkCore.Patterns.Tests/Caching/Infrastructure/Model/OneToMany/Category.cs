namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure.Model.OneToMany;

public class Category
{
	public int Id { get; set; }

	public int? ParentId { get; set; }
	public Category Parent { get; set; }

	public List<Category> Children { get; set; } = new List<Category>();
}
