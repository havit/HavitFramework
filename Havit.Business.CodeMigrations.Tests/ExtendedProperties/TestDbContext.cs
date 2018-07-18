using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Havit.Business.CodeMigrations.Tests.ExtendedProperties
{
	internal class TestDbContext : BusinessLayerDbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
			optionsBuilder.UseSqlServer(new SqlConnection("Database=DummyDatabase"));
		}
	}
}
