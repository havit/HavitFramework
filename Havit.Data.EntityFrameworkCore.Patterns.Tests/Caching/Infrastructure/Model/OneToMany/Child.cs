namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure.Model.OneToMany;

public class Child
{
	public int Id { get; set; }

	// Reference (nepovinná)
	public Master Parent { get; set; }
	public int? ParentId { get; set; }

	public DateTime? Deleted { get; set; }
}