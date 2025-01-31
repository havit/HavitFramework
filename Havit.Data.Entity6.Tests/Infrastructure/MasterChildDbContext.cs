using System.Data.Entity;
using Havit.Data.Entity.Helpers;
using Havit.Data.Entity.Tests.Infrastructure.Model;

namespace Havit.Data.Entity.Tests.Infrastructure;

internal class MasterChildDbContext : DbContext
{
	static MasterChildDbContext()
	{
		System.Data.Entity.Database.SetInitializer(new DropCreateDatabaseAlways<MasterChildDbContext>());
	}

	public MasterChildDbContext() : base(DatabaseNameHelper.GetDatabaseNameForUnitTest("Havit.Data.Entity6.Tests"))
	{
	}

	protected override void OnModelCreating(DbModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.RegisterEntityType(typeof(Master));
		modelBuilder.RegisterEntityType(typeof(Child));
	}
}
