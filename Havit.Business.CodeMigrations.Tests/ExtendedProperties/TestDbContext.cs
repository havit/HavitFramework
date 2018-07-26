using System.Data.SqlClient;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Havit.Business.CodeMigrations.Tests.ExtendedProperties
{
	internal class TestDbContext : BusinessLayerDbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
			optionsBuilder.UseSqlServer(new SqlConnection("Database=Dummy"));
		}

		protected override void ApplyConventions(ModelBuilder modelBuilder)
		{ }
	}
}
