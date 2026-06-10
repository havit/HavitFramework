using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Benchmarks.Infrastructure;

public class BenchmarkDbContext : Havit.Data.EntityFrameworkCore.DbContext
{
	public DbSet<BenchmarkEntity> Entities { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		base.OnConfiguring(optionsBuilder);
		optionsBuilder.UseInMemoryDatabase(nameof(BenchmarkDbContext));
	}
}
