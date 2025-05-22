using System.Runtime.CompilerServices;
using Havit.Data.EntityFrameworkCore.Metadata;
using Havit.Data.EntityFrameworkCore.Tests.Metadata.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Tests.Metadata.Infrastructure.Entity;

public class ApplicationEntityTestDbContext : DbContext
{
	private readonly string _databaseName;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	/// <param name="databaseName">
	/// Testy potřebujeme izolovat. Pro použití s InMemory databází je potřeba použít jiný název databáze pro každý test.
	/// Vzhledem k tomu, že "co test to databáze", hodí se však použít [CallerMemberName], který pak dostane jako hodnotu názvu databáze název metody, odkud je konstruktor zavolán.
	/// </param>
	public ApplicationEntityTestDbContext([CallerMemberName] string databaseName = default)
	{
		_databaseName = databaseName;
	}

	protected override void OnConfiguring(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder optionsBuilder)
	{
		base.OnConfiguring(optionsBuilder);
		optionsBuilder.UseInMemoryDatabase(_databaseName);
	}

	protected override void CustomizeModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder)
	{
		base.CustomizeModelCreating(modelBuilder);

		modelBuilder.Entity<DefaultApplicationEntity>();
		modelBuilder.Entity<ExplicitApplicationEntity>().HasAnnotation(ApplicationEntityAnnotationConstants.IsApplicationEntityAnnotationName, true);
		modelBuilder.Entity<NotApplicationEntity>().HasAnnotation(ApplicationEntityAnnotationConstants.IsApplicationEntityAnnotationName, false);
	}
}
