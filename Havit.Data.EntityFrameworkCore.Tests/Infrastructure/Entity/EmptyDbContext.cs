using Microsoft.EntityFrameworkCore;

namespace Havit.Data.Entity.Tests.Infrastructure.Entity
{
	public class EmptyDbContext : Data.Entity.DbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);

			optionsBuilder.UseInMemoryDatabase(typeof(EmptyDbContext).FullName);
		}
	}
}
