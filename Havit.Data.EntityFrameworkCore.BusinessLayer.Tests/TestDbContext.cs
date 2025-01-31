using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests;

public class TestDbContext : BusinessLayerDbContext
{
	private readonly Action<ModelBuilder> onModelCreating;

	public TestDbContext(Action<ModelBuilder> onModelCreating = default)
	{
		this.onModelCreating = onModelCreating;
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		base.OnConfiguring(optionsBuilder);

		optionsBuilder.ReplaceService<IModelCacheKeyFactory, NoCacheModelCacheKeyFactory>();
		optionsBuilder.UseSqlServer("Database=Dummy");
		optionsBuilder.EnableServiceProviderCaching(false);
	}

	protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
	{
		base.CustomizeModelCreating(modelBuilder);
		onModelCreating?.Invoke(modelBuilder);
	}
}
