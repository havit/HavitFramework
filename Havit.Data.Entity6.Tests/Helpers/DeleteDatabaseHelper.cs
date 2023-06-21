using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Entity.Tests.Helpers;

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
