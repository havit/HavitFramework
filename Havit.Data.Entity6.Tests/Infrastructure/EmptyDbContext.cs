using System.Data.Entity;
using Havit.Data.Entity.Helpers;

namespace Havit.Data.Entity.Tests.Infrastructure;

public class EmptyDbContext : DbContext
{
	static EmptyDbContext()
	{
		System.Data.Entity.Database.SetInitializer(new DropCreateDatabaseAlways<EmptyDbContext>());
	}

	public EmptyDbContext() : base(DatabaseNameHelper.GetDatabaseNameForUnitTest("Havit.Data.Entity6.Tests"))
	{
	}
}
