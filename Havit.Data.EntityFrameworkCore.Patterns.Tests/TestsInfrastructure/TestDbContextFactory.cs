using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.TestsInfrastructure
{
	public class TestDbContextFactory : IDbContextFactory
	{
		public IDbContext CreateDbContext()
		{
			return new TestDbContext();
		}
	}
}
