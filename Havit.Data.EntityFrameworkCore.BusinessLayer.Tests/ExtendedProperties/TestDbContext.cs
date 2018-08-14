using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.ExtendedProperties
{
	internal class TestDbContext : BusinessLayerDbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
			optionsBuilder.ReplaceService<IModelCacheKeyFactory, NoCacheModelCacheKeyFactory>();
			optionsBuilder.UseSqlServer(new SqlConnection("Database=Dummy"));
		}

		protected override void ApplyConventions(ModelBuilder modelBuilder)
		{ }

		private class NoCacheModelCacheKeyFactory : IModelCacheKeyFactory
		{
			public object Create(DbContext context) => context.GetHashCode();
		}
	}
}
