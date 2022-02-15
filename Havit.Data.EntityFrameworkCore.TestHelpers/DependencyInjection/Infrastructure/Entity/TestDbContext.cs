using Havit.Data.EntityFrameworkCore.TestHelpers.DependencyInjection.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.TestHelpers.DependencyInjection.Infrastructure.Entity
{
	public class TestDbContext : DbContext
	{
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {			
        }

		protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
		{
			base.CustomizeModelCreating(modelBuilder);

			modelBuilder.Entity<Language>();
		}
	}
}
