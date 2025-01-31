namespace Havit.Data.Entity.Tests.Infrastructure.Model;

public class Child
{
	public int Id { get; set; }

	public int MasterId { get; set; }
	public Master Master { get; set; }
}
