using Havit.Model.Collections.Generic;

namespace Havit.EFCoreTests.Model;

public class Person
{
	public int Id { get; set; }

	public string Name { get; set; } = Guid.NewGuid().ToString();

	public Person Boss { get; set; }
	public int? BossId { get; set; }

	public List<Person> Subordinates { get; } = new List<Person>();

	public FilteringCollection<Person> SubordinatesIncludingDeleted { get; }

	// TODO JK (po merge s newgendbdataloader): Neudělal jsem migraci, mám v jiné branch tutéž upravu, databáze mi přežívá
	//public DateTime? Deleted { get; set; }

	public Person()
	{
		SubordinatesIncludingDeleted = new FilteringCollection<Person>(Subordinates, p => true);
	}
}
