using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.TestsInfrastructure;

public class TestDbContext : DbContext
{
	private bool useInMemoryDatabase = false;

	public TestDbContext()
	{
		useInMemoryDatabase = true;
	}

	public TestDbContext(DbContextOptions options) : base(options)
	{
		useInMemoryDatabase = false;
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		base.OnConfiguring(optionsBuilder);

		if (useInMemoryDatabase)
		{
			optionsBuilder.UseInMemoryDatabase(nameof(TestDbContext));
		}
	}

	protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
	{
		base.CustomizeModelCreating(modelBuilder);

		modelBuilder.Entity<ItemWithDeleted>();
		modelBuilder.Entity<ItemWithNullableProperty>();
		modelBuilder.Entity<Language>();
		modelBuilder.Entity<Employee>().HasMany(e => e.Subordinates).WithOne(e => e.Boss).HasForeignKey(e => e.BossId);
		modelBuilder.Entity<ManyToMany>().HasKey(nameof(ManyToMany.LanguageId), nameof(ManyToMany.ItemWithDeletedId));
	}
}
