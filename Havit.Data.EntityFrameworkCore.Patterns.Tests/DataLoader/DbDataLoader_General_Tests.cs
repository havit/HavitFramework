using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader.Model;
using Havit.Data.Patterns.DataLoaders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader
{
	[TestClass]
	public class DbDataLoader_General_Tests : DbDataLoaderTestsBase
	{
		[TestMethod]
		public void DbDataLoader_Load_SkipsNullEntities()
		{
			// Arrange
			SeedOneToManyTestData();

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager());

			// Act
			dataLoader.Load((Child)null, c => c.Parent);
			dataLoader.LoadAll(new Child[] { null }, c => c.Parent);

			// Assert: No exception was thrown
		}

		[TestMethod]
		public void DbDataLoader_Load_SupportsNullValues()
		{
			// Arrange
			DataLoaderTestDbContext dbContext1 = new DataLoaderTestDbContext();
			dbContext1.Database.DropCreate();

			Child child1 = new Child();
			Child child2 = new Child();
			Master master = new Master();
			child1.Parent = master;

			dbContext1.Set<Child>().Add(child1);
			dbContext1.Set<Child>().Add(child2);
			dbContext1.Set<Master>().Add(master);

			dbContext1.SaveChanges();

			DataLoaderTestDbContext dbContext2 = new DataLoaderTestDbContext();
			List<Child> childs = dbContext2.Set<Child>().ToList();

			// Act
			IDataLoader dataLoader = new DbDataLoader(dbContext2, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager());

			dataLoader.LoadAll(childs, child => child.Parent.Children);

			// Assert - no exception was thrown
		}
	}
}
