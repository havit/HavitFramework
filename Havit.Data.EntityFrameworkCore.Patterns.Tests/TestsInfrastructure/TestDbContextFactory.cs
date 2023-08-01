namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.TestsInfrastructure;

public class TestDbContextFactory : IDbContextFactory
{
	public IDbContext CreateDbContext()
	{
		return new TestDbContext();
	}
}
