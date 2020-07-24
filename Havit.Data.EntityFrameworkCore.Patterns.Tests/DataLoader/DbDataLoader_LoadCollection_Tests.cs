using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.PropertyLambdaExpressions.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader.Model;
using Havit.Data.Patterns.DataLoaders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader
{
	[TestClass]
	public class DbDataLoader_LoadCollection_Tests : DbDataLoaderTestsBase
	{
		[TestMethod]
		public void DbDataLoader_Load_Collection_LoadsNotLoadedCollections()
		{
			// Arrange
			SeedOneToManyTestData();

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();

			Master master = dbContext.Master.First();

			Assert.IsFalse(master.Children.Any(), "Pro ověření DbDataLoaderu se předpokládá, že master.Children je prázdná.");

			// Act
			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolverIncludingDeletedFilteringCollectionsSubstitution(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager(), new DbEntityKeyAccessor(new DbEntityKeyAccessorStorage(), dbContext));
			dataLoader.Load(master, item => item.Children);

			// Assert
			Assert.IsNotNull(master.Children, "DbDataLoader nenačetl hodnotu pro master.Children.");
			Assert.AreEqual(5, master.Children.Count, "DbDataLoader nenačetl objekty do master.Children.");
		}

		[TestMethod]
		public void DbDataLoader_Load_Collection_ToManyToMany_LoadsNotLoadedCollections()
		{
			// Arrange
			SeedManyToManyTestData(false);

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();

			LoginAccount loginAccount = dbContext.LoginAccount.First();

			Assert.IsNull(loginAccount.Memberships, "Pro ověření DbDataLoaderu se předpokládá, že hodnota loginAccount.Memberships je null.");

			// Act
			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolverIncludingDeletedFilteringCollectionsSubstitution(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager(), new DbEntityKeyAccessor(new DbEntityKeyAccessorStorage(), dbContext));
			dataLoader.Load(loginAccount, item => item.Memberships);

			// Assert
			Assert.IsNotNull(loginAccount.Memberships, "DbDataLoader nenačetl hodnotu pro loginAccount.Memberships.");
			Assert.AreEqual(1, loginAccount.Memberships.Count, "DbDataLoader nenačetl objekty do loginAccount.Memberships.");
		}

		[TestMethod]
		public void DbDataLoader_Load_Collection_LoadsPartiallyInitializedCollections()
		{
			// Arrange
			SeedOneToManyTestData();

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();

			Master master = dbContext.Master.First();
			Child child = dbContext.Child.Where(item => item.ParentId == master.Id).First();

			Assert.IsNotNull(master.Children, "Pro ověření DbDataLoaderu se předpokládá, že hodnota master.Children je (částečně) initializovaná.");
			Assert.AreEqual(1, master.Children.Count, "Pro ověření DbDataLoaderu se předpokládá, že hodnota master.Children obsahuje jeden prvek.");

			// Act
			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolverIncludingDeletedFilteringCollectionsSubstitution(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager(), new DbEntityKeyAccessor(new DbEntityKeyAccessorStorage(), dbContext));
			dataLoader.Load(master, item => item.Children);

			// Assert
			Assert.AreEqual(5, master.Children.Count, "DbDataLoader nenačetl objekty do master.Children.");
		}

		[TestMethod]
		public void DbDataLoader_Load_Collection_DoesNotLoadExcessEntities()
		{
			// Arrange
			SeedOneToManyTestData(deleted: false);

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();

			Master master = dbContext.Master.First();

			Assert.IsFalse(master.Children.Any(), "Pro ověření DbDataLoaderu se předpokládá, že hodnota master.Children je prázdná.");

			// Act
			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolverIncludingDeletedFilteringCollectionsSubstitution(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager(), new DbEntityKeyAccessor(new DbEntityKeyAccessorStorage(), dbContext));
			dataLoader.Load(master, item => item.Children);

			// Assert
			Assert.AreEqual(1, dbContext.Master.Local.Count, "DbDataLoader načetl zbytečné objekty Master.");
			Assert.AreEqual(5, dbContext.Child.Local.Count, "DbDataLoader načetl zbytečné objekty Child.");
		}

		[TestMethod]
		public void DbDataLoader_Load_Collection_DoesNotLoadAlreadyLoaded()
		{
			// Arrange
			Mock<DataLoaderTestDbContext> dbContextMock = new Mock<DataLoaderTestDbContext>();
			dbContextMock.CallBase = true;

			SeedOneToManyTestData(dbContext: dbContextMock.Object);

			Master master = dbContextMock.Object.Master.First();

			DbDataLoader dbDataLoader = new DbDataLoader(dbContextMock.Object, new PropertyLoadSequenceResolverIncludingDeletedFilteringCollectionsSubstitution(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager(), new DbEntityKeyAccessor(new DbEntityKeyAccessorStorage(), dbContextMock.Object));

			// Act + Assert

			dbContextMock.Verify(m => m.Set<Child>(), Times.Never);

			dbDataLoader.Load(master, item => item.Children);

			// Říkáme, že aby mohl DbDataLoader načíst Children, musí si získat DbSet<Child> pomocí metody Set<Child>().
			// Takže testujeme, že se na ní ani šáhl pouze jedenkrát - zde při prvním načtení kolekce.
			dbContextMock.Verify(m => m.Set<Child>(), Times.Once);

			dbDataLoader.Load(master, item => item.Children);

			// Říkáme, že aby mohl DbDataLoader načíst Children, musí si získat DbSet<Child> pomocí metody Set<Child>().
			// Takže testujeme, že se na ní ani šáhl pouze jedenkrát výše, a již vícekrát ne.
			dbContextMock.Verify(m => m.Set<Child>(), Times.Once);

			// není důvod načítat mastery, takže jen ověříme, že se nikdy na Set<Master> nešáhlo
			dbContextMock.Verify(m => m.Set<Master>(), Times.Never);
		}

		[TestMethod]
		public void DbDataLoader_Load_Collection_InitializeCollectionWhenNoDataIsLoaded()
		{
			// Arrange
			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			dbContext.Database.DropCreate();

			DbDataLoader dbDataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolverIncludingDeletedFilteringCollectionsSubstitution(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager(), new DbEntityKeyAccessor(new DbEntityKeyAccessorStorage(), dbContext));

			// Act
			LoginAccount loginAccount = new LoginAccount { Id = 1 };
			dbContext.LoginAccount.Attach(loginAccount);

			Assert.IsNull(loginAccount.Memberships, "Pro správnou funkci testu se předpokládá, že kolekce je null.");

			dbDataLoader.Load(loginAccount, la => la.Memberships); // v databázi nejsou žádná data, objekt jsme vytvořili jen in memory a databáze je smazána

			// Assert
			Assert.IsNotNull(loginAccount.Memberships);
		}

		[TestMethod]
		public void DbDataLoader_Load_Collection_MarksPropertyAsLoadedWhenNoDataIsLoaded()
		{
			// Arrange
			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			dbContext.Database.DropCreate();

			DbDataLoader dbDataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolverIncludingDeletedFilteringCollectionsSubstitution(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager(), new DbEntityKeyAccessor(new DbEntityKeyAccessorStorage(), dbContext));

			// Act
			LoginAccount loginAccount = new LoginAccount { Id = 1 };
			dbContext.LoginAccount.Attach(loginAccount);

			Assert.IsFalse(dbContext.Entry(loginAccount).Collection(nameof(LoginAccount.Memberships)).IsLoaded, "Pro správnou funkci testu se předpokládá, že kolekce není načtena.");

			dbDataLoader.Load(loginAccount, la => la.Memberships); // v databázi nejsou žádná data, objekt jsme vytvořili jen in memory a databáze je smazána

			// Assert
			Assert.IsTrue(dbContext.Entry(loginAccount).Collection(nameof(LoginAccount.Memberships)).IsLoaded);
		}

		[TestMethod]
		public void DbDataLoader_Load_Collection_SupportsNullableForeignKeysInMemory()
		{
			// Arrange
			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			dbContext.Database.DropCreate();
			SeedOneToManyTestData();

			DbDataLoader dbDataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolverIncludingDeletedFilteringCollectionsSubstitution(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager(), new DbEntityKeyAccessor(new DbEntityKeyAccessorStorage(), dbContext));

			Master master = dbContext.Master.First();

			// Pro projevení problému s EF Core potřebujeme do identity mapy dostat objekt, který má ParentId null.
			Child child = new Child { Id = 5, ParentId = null };
			dbContext.Attach(child);

			// Act
			dbDataLoader.Load(master, m => m.Children);

			// Assert
			// No exception was thown
		}

		[TestMethod]
		public void DbDataLoader_Load_Collection_SupportsNullableForeignKeysInDatabase()
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

			List<Master> masters = dbContext2.Set<Master>().ToList();

			// Act
			IDataLoader dataLoader = new DbDataLoader(dbContext2, new PropertyLoadSequenceResolverIncludingDeletedFilteringCollectionsSubstitution(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager(), new DbEntityKeyAccessor(new DbEntityKeyAccessorStorage(), dbContext2));
			dataLoader.LoadAll(masters, m => m.Children);

			// Assert - no exception was thrown
		}


		[TestMethod]
		public void DbDataLoader_Load_Collection_LoadAreReentrant()
		{
			// Arrange
			SeedOneToManyTestData();

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();

			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolverIncludingDeletedFilteringCollectionsSubstitution(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager(), new DbEntityKeyAccessor(new DbEntityKeyAccessorStorage(), dbContext));

			Master master1 = dbContext.Master.OrderBy(m => m.Id).First();
			Master master2 = dbContext.Master.OrderBy(m => m.Id).Skip(1).First();

			// Act

			dataLoader.Load(master1, m => m.Children);
			dataLoader.LoadAll(new Master[] { master1, master2 }, m => m.Children);

			// Assert
			Assert.IsTrue(master1.Children.All(item => item != null), "Položky kolekce Children proměné master1 nejsou načteny.");
			Assert.IsTrue(master2.Children.All(item => item != null), "Položky kolekce Children proměné master2 nejsou načteny.");
		}

		[TestMethod]
		public void DbDataLoader_Load_Collection_LoadDependenciesFromNotSubstituedProperty()
		{
			// Arrange
			SeedOneToManyTestData(deleted: true);

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();

			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolverIncludingDeletedFilteringCollectionsSubstitution(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager(), new DbEntityKeyAccessor(new DbEntityKeyAccessorStorage(), dbContext));

			Master master = dbContext.Master.First();

			// Act
			DbFluentDataLoader<ICollection<Child>> fluentDataLoader = (DbFluentDataLoader<ICollection<Child>>)dataLoader.Load(master, m => m.Children);

			// Assert
			Assert.AreEqual(0, fluentDataLoader.Data.Count(), "Jsou vybráni smazané Child k načítání závislostí.");
		}
	}
}
