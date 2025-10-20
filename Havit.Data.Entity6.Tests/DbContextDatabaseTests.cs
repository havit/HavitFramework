using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Havit.Data.Entity.Tests.Helpers;
using Havit.Data.Entity.Tests.Infrastructure;
using Havit.Data.Entity.Tests.Infrastructure.Model;

namespace Havit.Data.Entity.Tests;

[TestClass]
public class DbContextDatabaseTests
{
	[ClassCleanup]
	public static void CleanUp()
	{
		DeleteDatabaseHelper.DeleteDatabase<MasterChildDbContext>();
	}

	[TestMethod]
	public void DbContextDatabase_SqlQueryRaw()
	{
		// Arrange
		using (MasterChildDbContext dbContext = new MasterChildDbContext())
		{
			// Act
			DbRawSqlQuery<int> query = ((IDbContext)dbContext).Database.SqlQueryRaw<int>("SELECT Count(MasterId) FROM dbo.Master WHERE MasterId < @p0", -5 /* nemá funkční význam, jen ukázka parametrizace */);
			int count = query.Single();

			// Assert
			Assert.AreEqual(0, count);
		}
	}

	[TestMethod]
	public void DbContextDatabase_SqlQueryEntity()
	{
		// Arrange
		using (MasterChildDbContext dbContext = new MasterChildDbContext())
		{
			Master master1 = new Master();
			dbContext.Set<Master>().Add(master1);
			dbContext.SaveChanges();

			// Act
			DbSqlQuery<Master> query = ((IDbContext)dbContext).Database.SqlQueryEntity<Master>("SELECT MasterId as Id FROM dbo.Master WHERE MasterId = @p0", master1.Id);
			Master master = query.Single();

			// Assert
			Assert.AreEqual(EntityState.Unchanged, dbContext.Entry(master).State); // je trackovaný
		}
	}

}
