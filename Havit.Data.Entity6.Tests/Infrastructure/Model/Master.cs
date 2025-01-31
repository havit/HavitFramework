namespace Havit.Data.Entity.Tests.Infrastructure.Model;

public class Master
{
	public int Id { get; set; }

	// pro účely testu nemá setter
	public List<Child> Children { get; } = new List<Child>();
}
