namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.TestsInfrastructure;

public class ItemWithDeleted
{
	public int Id { get; set; }
	public DateTime? Deleted { get; set; }
	public string Symbol { get; set; }
}