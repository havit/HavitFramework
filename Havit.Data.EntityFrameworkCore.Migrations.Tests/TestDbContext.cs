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
            optionsBuilder.UseExtendedMigrations(
                builder => builder
                    .UseStoredProcedures()
                    .UseViews());
            optionsBuilder.ReplaceService<IModelCacheKeyFactory, NoCacheModelCacheKeyFactory>();
            optionsBuilder.UseSqlServer(new SqlConnection("Database=Dummy"));
        }

        private class NoCacheModelCacheKeyFactory : IModelCacheKeyFactory
        {
            public object Create(DbContext context) => context.GetHashCode();
        }
    }
}
