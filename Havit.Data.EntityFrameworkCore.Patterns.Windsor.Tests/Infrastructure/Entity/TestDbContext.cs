using Havit.Data.EntityFrameworkCore.Patterns.Windsor.Tests.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Patterns.Windsor.Tests.Infrastructure.Entity
{
	internal class TestDbContext : DbContext
	{
		protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
		{
			base.CustomizeModelCreating(modelBuilder);

			modelBuilder.Entity<Language>();
		}
	}
}
