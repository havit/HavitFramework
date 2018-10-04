using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Infrastructure
{
	public class TestDbContext : DbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);

			optionsBuilder.UseInMemoryDatabase(nameof(TestDbContext));
		}

		protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
		{
			base.CustomizeModelCreating(modelBuilder);

			modelBuilder.Model.AddEntityType(typeof(ItemWithDeleted));
			modelBuilder.Model.AddEntityType(typeof(ItemWithNullableProperty));
            modelBuilder.Model.AddEntityType(typeof(Language));
		}
	}
}
