using Havit.Data.EntityFrameworkCore.Attributes;

namespace Havit.EFCoreTests.Model;

[Cache]
public class BusinessCase
{
	public int Id { get; set; }

	public List<Modelation> Modelations { get; } = new List<Modelation>();
}
