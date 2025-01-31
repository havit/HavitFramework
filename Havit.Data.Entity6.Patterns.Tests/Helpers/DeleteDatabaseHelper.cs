namespace Havit.Data.Entity.Patterns.Tests.Helpers;

public static class DeleteDatabaseHelper
{
	public static void DeleteDatabase<TDbContext>()
		where TDbContext : DbContext, new()
	{
		using (TDbContext dbContext = new TDbContext())
		{
			dbContext.Database.Delete();
		}
	}
}
