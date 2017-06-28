using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.Entity.Tests.Infrastructure;
using Havit.Data.Entity.Tests.Infrastructure.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.Entity.Tests
{
	[TestClass]
	public class DbContextDatabaseTest
	{
		[TestMethod]
		public void DbContextDatabase_SqlQueryRaw()
		{
			// Arrange
			MasterChildDbContext context = new MasterChildDbContext();

			// Act
			DbRawSqlQuery<int> query = ((IDbContext)context).Database.SqlQueryRaw<int>("SELECT Count(MasterId) FROM dbo.Master WHERE MasterId < @p0", -5 /* nemá funkční význam, jen ukázka parametrizace */);
			int count = query.Single();

			// Assert
			Assert.AreEqual(0, count);
		}

		[TestMethod]
		public void DbContextDatabase_SqlQueryEntity()
		{
			// Arrange
			MasterChildDbContext context = new MasterChildDbContext();
			Master master1 = new Master();
			context.Set<Master>().Add(master1);
			context.SaveChanges();

			// Act
			DbSqlQuery<Master> query = ((IDbContext)context).Database.SqlQueryEntity<Master>("SELECT MasterId as Id FROM dbo.Master WHERE MasterId = @p0", master1.Id);
			Master master = query.Single();

			// Assert
			Assert.IsTrue(context.Entry(master).State == EntityState.Unchanged); // je trackovaný
		}

	}
}
