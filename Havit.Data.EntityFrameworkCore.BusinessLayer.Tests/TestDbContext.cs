using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests
{
    public class TestDbContext : BusinessLayerDbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
			optionsBuilder.ReplaceService<IModelCacheKeyFactory, NoCacheModelCacheKeyFactory>();
			optionsBuilder.UseSqlServer(new SqlConnection("Database=Dummy"));
			optionsBuilder.EnableServiceProviderCaching(false);
		}

		private class NoCacheModelCacheKeyFactory : IModelCacheKeyFactory
		{
			public object Create(Microsoft.EntityFrameworkCore.DbContext context) => context.GetHashCode();
		}
	}
}
