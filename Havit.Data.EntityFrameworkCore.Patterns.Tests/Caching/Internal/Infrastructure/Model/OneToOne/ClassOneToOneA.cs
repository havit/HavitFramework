namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Internal.Infrastructure.Model.OneToOne;

public class ClassOneToOneA
{
	public int Id { get; set; }
	public ClassOneToOneB ClassB { get; set; } // no foreign key - "backreference"
}
