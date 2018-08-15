﻿using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Havit.Data.Entity.Conventions;
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

		protected override IEnumerable<IModelConvention> GetModelConventions()
		{
			// no base call
			return Enumerable.Empty<IModelConvention>();
		}

		private class NoCacheModelCacheKeyFactory : IModelCacheKeyFactory
		{
			public object Create(DbContext context) => context.GetHashCode();
		}
	}
}