using System.Data.Entity;
using Havit.Data.Entity.Helpers;

namespace Havit.Data.Entity.Patterns.Tests.Infrastructure;

public class TestDbContext : Havit.Data.Entity.DbContext
{
	public TestDbContext() : base(DatabaseNameHelper.GetDatabaseNameForUnitTest("Havit.Data.Entity6.Patterns.Tests"))
	{
		Database.SetInitializer(new DropCreateDatabaseAlways<TestDbContext>());
	}

	protected override void OnModelCreating(DbModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.RegisterEntityType(typeof(ItemWithDeleted));
		modelBuilder.RegisterEntityType(typeof(ItemWithNullableProperty));
		modelBuilder.RegisterEntityType(typeof(Language));
		modelBuilder.RegisterEntityType(typeof(ChildEntity));
		modelBuilder.RegisterEntityType(typeof(ParentEntity));
	}
}
