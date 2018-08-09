using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.Entity.Helpers;

namespace Havit.Data.Entity.Tests.Infrastructure
{
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
}
