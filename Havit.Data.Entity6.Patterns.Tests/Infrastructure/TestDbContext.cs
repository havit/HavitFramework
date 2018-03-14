using System.Data.Entity;

namespace Havit.Data.Entity.Patterns.Tests.Infrastructure
{
	public class TestDbContext : DbContext
	{
		public TestDbContext() : base("Havit.Data.Entity6.Patterns.Tests")
		{
			Database.SetInitializer(new DropCreateDatabaseAlways<TestDbContext>());
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.RegisterEntityType(typeof(ItemWithDeleted));
			modelBuilder.RegisterEntityType(typeof(ItemWithNullableProperty));
			modelBuilder.RegisterEntityType(typeof(Language));
		}
	}
}
