using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.Entity.Tests.Infrastructure.Model;

namespace Havit.Data.Entity.Tests.Infrastructure
{
	internal class MasterChildDbContext : DbContext
	{
		static MasterChildDbContext()
		{
			System.Data.Entity.Database.SetInitializer(new DropCreateDatabaseAlways<MasterChildDbContext>());
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.RegisterEntityType(typeof(Master));
			modelBuilder.RegisterEntityType(typeof(Child));			
		}
	}
}
