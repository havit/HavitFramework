using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.Entity.Tests.Infrastructure.Model;

namespace Havit.Data.Entity.Tests.Infrastructure
{
	public class TwoContructorsDbContext : DbContext
	{
		public TwoContructorsDbContext() : base("Havit.Data.Entity6.Tests.TwoContructorsDbContext1")
		{
			
		}

		public TwoContructorsDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
		{
			
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.RegisterEntityType(typeof(Master));
			modelBuilder.RegisterEntityType(typeof(Child));
		}

	}
}
