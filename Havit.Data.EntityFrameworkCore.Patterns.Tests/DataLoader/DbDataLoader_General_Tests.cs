﻿using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.PropertyLambdaExpressions.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader.Model;
using Havit.Data.Patterns.DataLoaders;
using Microsoft.Extensions.Logging;
using Moq;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader;

[TestClass]
public class DbDataLoader_General_Tests : DbDataLoaderTestsBase
{
	[TestMethod]
	public void DbDataLoader_Load_SkipsNullEntities()
	{
		// Arrange
		SeedOneToManyTestData();

		DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();

		IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolverIncludingDeletedFilteringCollectionsSubstitution(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager(), new DbEntityKeyAccessor(new DbEntityKeyAccessorStorage(), dbContext), Mock.Of<ILogger<DbDataLoader>>(MockBehavior.Loose /* umožníme použití bez setupu */));

		// Act
		dataLoader.Load((Child)null, c => c.Parent);
		dataLoader.LoadAll(new Child[] { null }, c => c.Parent);

		// Assert: No exception was thrown
	}
}
