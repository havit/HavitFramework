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

		modelBuilder.Entity(typeof(ItemWithDeleted));
		modelBuilder.Entity(typeof(ItemWithNullableProperty));
		modelBuilder.Entity(typeof(Language));
		modelBuilder.Entity(typeof(ManyToMany)).HasKey(nameof(ManyToMany.LanguageId), nameof(ManyToMany.ItemWithDeletedId));
	}
}
