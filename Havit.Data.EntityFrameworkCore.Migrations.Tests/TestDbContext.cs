using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Migrations.Tests
{
    public class TestDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseModelExtensions(
                builder => builder
                    .UseStoredProcedures()
                    .UseViews());
            optionsBuilder.UseSqlServer(new SqlConnection("Database=Dummy"));
			optionsBuilder.EnableServiceProviderCaching(false);
		}

    }
}
