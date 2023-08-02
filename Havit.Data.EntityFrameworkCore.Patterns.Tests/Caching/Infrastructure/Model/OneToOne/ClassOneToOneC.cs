namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure.Model.OneToOne;

public class ClassOneToOneC
{
	public int Id { get; set; }

	// Otázkou je, jesti tento testovací scénář vůbec dává smysl.

	public ClassOneToOneC Direct { get; set; }
	public int DirectId { get; set; }

	public ClassOneToOneC Indirect { get; set; } // no foreign key - "backreference"

}
