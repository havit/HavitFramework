using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Tests.Infrastructure.Entity
{
	public class EmptyDbContext : DbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer("fake");

			base.OnConfiguring(optionsBuilder);
		}
	}
}
