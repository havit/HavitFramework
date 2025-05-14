using Havit.Model.Collections.Generic;

namespace Havit.EFCoreTests.Model;

public class Person
{
	public int Id { get; set; }

	public string Name { get; set; } = Guid.NewGuid().ToString();

	public Person Boss { get; set; }
	public int? BossId { get; set; }

	public FilteringCollection<Person> Subordinates { get; }

	public List<Person> SubordinatesIncludingDeleted { get; } = new List<Person>();

	public DateTime? Deleted { get; set; }

	public Person()
	{
		Subordinates = new FilteringCollection<Person>(SubordinatesIncludingDeleted, p => Deleted == null);
	}
}
