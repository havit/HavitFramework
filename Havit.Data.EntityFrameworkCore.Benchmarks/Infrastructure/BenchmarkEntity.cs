namespace Havit.Data.EntityFrameworkCore.Benchmarks.Infrastructure;

public class BenchmarkEntity
{
	public int Id { get; set; }
	public string Name { get; set; }

	public List<BenchmarkEntity> Children { get; set; } = new List<BenchmarkEntity>();
}
