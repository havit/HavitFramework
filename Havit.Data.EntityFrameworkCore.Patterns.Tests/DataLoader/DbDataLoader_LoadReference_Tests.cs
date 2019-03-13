using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.PropertyLambdaExpressions.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader.Model;
using Havit.Data.Patterns.DataLoaders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader
{
	[TestClass]
	public class DbDataLoader_LoadReference_Tests : DbDataLoaderTestsBase
	{
		[TestMethod]
		public void DbDataLoader_Load_Reference_LoadsNotLoadedReferences()
		{
			// Arrange
			SeedOneToManyTestData(deleted: false);

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();

			Child child = dbContext.Child.First();

			Assert.IsNull(child.Parent, "Pro ověření DbDataLoaderu se předpokládá, že hodnota child.Parent je null.");
			Assert.AreNotEqual(0, child.ParentId, "Pro ověření DbDataLoaderu se předpokládá, že hodnota child.ParentId není nula.");

			// Act
			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolverWithDeletedFilteringCollectionsSubstitution(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager(), new DbEntityKeyAccessor(dbContext.CreateDbContextFactory()));
			dataLoader.Load(child, item => item.Parent);

			// Assert
			Assert.IsNotNull(child.Parent, "DbDataLoader nenačetl hodnotu pro child.Parent.");
			Assert.IsTrue(dbContext.GetEntry(child, suppressDetectChanged: false).Reference(nameof(Child.Parent)).IsLoaded, "DbContext nepovažuje vlastnost za načtenou.");
		}

		[TestMethod]
		public void DbDataLoader_Load_Reference_LoadNotLoadedReferences_OnNewObject()
		{
			// Arrange
			SeedOneToManyTestData();

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();

			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolverWithDeletedFilteringCollectionsSubstitution(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager(), new DbEntityKeyAccessor(dbContext.CreateDbContextFactory()));

			Child child1 = dbContext.Child.First();
			Child child2 = new Child { ParentId = child1.ParentId };
			dbContext.Child.Add(child2); // Added

			// Act
			dataLoader.Load(child2, c => c.Parent);

			// Assert
			Assert.IsNotNull(child2.Parent, "Child2.Parent není načtena, ačkoliv bylo o načtení požádáno.");
			Assert.IsNotNull(child1.Parent, "Child1.Parent není načtena, ačkoliv se předpokládá, že došlo k fixupu.");
		}

		[TestMethod]
		public void DbDataLoader_Load_Reference_DoesNotLoadExcessEntities()
		{
			// Arrange
			SeedOneToManyTestData(deleted: false);

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();

			Child child = dbContext.Child.First();

			Assert.IsNull(child.Parent, "Pro ověření DbDataLoaderu se předpokládá, že hodnota child.Parent je null.");
			Assert.AreNotEqual(0, child.ParentId, "Pro ověření DbDataLoaderu se předpokládá, že hodnota child.ParentId není nula.");

			// Act
			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolverWithDeletedFilteringCollectionsSubstitution(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager(), new DbEntityKeyAccessor(dbContext.CreateDbContextFactory()));
			dataLoader.Load(child, item => item.Parent);

			// Assert
			Assert.AreEqual(1, dbContext.Master.Local.Count, "DbDataLoader načetl zbytečné objekty Master.");
			Assert.AreEqual(1, dbContext.Child.Local.Count, "DbDataLoader načetl zbytečné objekty Child.");
		}

		[TestMethod]
		public void DbDataLoader_Load_Reference_IncludesDeletedReferences()
		{
			// Arrange
			SeedOneToManyTestData(deleted: true);

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();

			Child child = dbContext.Child.First();

			Assert.IsNull(child.Parent, "Pro ověření DbDataLoaderu se předpokládá, že hodnota child.Parent je null.");
			Assert.AreNotEqual(0, child.ParentId, "Pro ověření DbDataLoaderu se předpokládá, že hodnota child.ParentId není nula.");

			// Act
			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolverWithDeletedFilteringCollectionsSubstitution(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager(), new DbEntityKeyAccessor(dbContext.CreateDbContextFactory()));
			dataLoader.Load(child, item => item.Parent);

			// Assert
			Assert.IsNotNull(child.Parent, "DbDataLoader nenačetl hodnotu pro child.Parent.");
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void DbDataLoader_Load_Reference_ThrowsExceptionForNontrackedObjects()
		{
			// Arrange
			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			dbContext.Database.DropCreate();
			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolverWithDeletedFilteringCollectionsSubstitution(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager(), new DbEntityKeyAccessor(dbContext.CreateDbContextFactory()));

			// Act
			dataLoader.Load(new Child() /* nontracked object */, item => item.Parent);

			// Assert by method attribute 
		}

		[TestMethod]
		public void DbDataLoader_Load_Reference_DoesNotLoadAlreadyLoadedReferences()
		{
			// Arrange
			Mock<DataLoaderTestDbContext> dbContextMock = new Mock<DataLoaderTestDbContext>();
			dbContextMock.CallBase = true;

			dbContextMock.Object.Database.DropCreate();

			Child child = new Child
			{
				Id = 1,
				Parent = new Master
				{
					Id = 1
				}
			};

			dbContextMock.Object.Attach(child);

			// Act
			IDataLoader dataLoader = new DbDataLoader(dbContextMock.Object, new PropertyLoadSequenceResolverWithDeletedFilteringCollectionsSubstitution(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager(), new DbEntityKeyAccessor(dbContextMock.Object.CreateDbContextFactory()));
			dataLoader.Load(child, item => item.Parent);

			// Assert
			// říkáme, že aby mohl DbDataLoader načíst data, musí si získat DbSet pomocí metody Set
			// takže testujeme, že na ní ani nešáhl
			dbContextMock.Verify(m => m.Set<Child>(), Times.Never);
			dbContextMock.Verify(m => m.Set<Master>(), Times.Never);
		}

	}
}
