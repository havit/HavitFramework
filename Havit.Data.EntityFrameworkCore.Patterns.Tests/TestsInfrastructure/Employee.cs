namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.TestsInfrastructure;

public class Employee
{
	public int Id { get; set; }

	public Employee Boss { get; set; }
	public int? BossId { get; set; }

	public List<Employee> Subordinates { get; set; }

	public DateTime? Deleted { get; set; }
}
