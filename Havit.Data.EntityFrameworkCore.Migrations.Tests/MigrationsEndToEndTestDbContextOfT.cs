using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Migrations.Tests;

public class MigrationsEndToEndTestDbContext<TEntity> : MigrationsEndToEndTestDbContext
	where TEntity : class
{
	public MigrationsEndToEndTestDbContext(Action<ModelBuilder> onModelCreating = default)
		: base(onModelCreating)
	{ }

	public DbSet<TEntity> Entities { get; }
}
