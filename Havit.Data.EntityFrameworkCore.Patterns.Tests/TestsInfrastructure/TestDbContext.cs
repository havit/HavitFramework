using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.TestsInfrastructure;

public class TestDbContext : DbContext
{
	private readonly string _databaseName;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	/// <param name="databaseName">
	/// Testy potřebujeme izolovat. Pro použití s InMemory databází je potřeba použít jiný název databáze pro každý test.
	/// Vzhledem k tomu, že "co test to databáze", hodí se však použít [CallerMemberName], který pak dostane jako hodnotu názvu databáze název metody, odkud je konstruktor zavolán.
	/// </param>
	public TestDbContext([CallerMemberName] string databaseName = default)
	{
		_databaseName = databaseName;
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		base.OnConfiguring(optionsBuilder);

		optionsBuilder.UseInMemoryDatabase(_databaseName);
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
