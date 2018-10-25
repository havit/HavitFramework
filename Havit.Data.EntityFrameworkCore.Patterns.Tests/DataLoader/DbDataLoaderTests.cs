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
	public class DbDataLoaderTests
	{
		[TestMethod]
		public void DbDataLoader_Load_LoadsNotLoadedEntities()
		{
			// Arrange
			SeedOneToManyTestData(deleted: false);

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			Child child = dbContext.Child.First();

			Assert.IsNull(child.Parent, "Pro ověření DbDataLoaderu se předpokládá, že hodnota child.Parent je null.");
			Assert.AreNotEqual(0, child.ParentId, "Pro ověření DbDataLoaderu se předpokládá, že hodnota child.ParentId není nula.");

			// Act
			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager());
			dataLoader.Load(child, item => item.Parent);

			// Assert
			Assert.IsNotNull(child.Parent, "DbDataLoader nenačetl hodnotu pro child.Parent.");
		}

		[TestMethod]
		public void DbDataLoader_Load_DoesNotLoadExcessEntities()
		{
			// Arrange
			SeedOneToManyTestData(deleted: false);

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			Child child = dbContext.Child.First();

			Assert.IsNull(child.Parent, "Pro ověření DbDataLoaderu se předpokládá, že hodnota child.Parent je null.");
			Assert.AreNotEqual(0, child.ParentId, "Pro ověření DbDataLoaderu se předpokládá, že hodnota child.ParentId není nula.");

			// Act
			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager());
			dataLoader.Load(child, item => item.Parent);

			// Assert
			Assert.AreEqual(1, dbContext.Master.Local.Count, "DbDataLoader načetl zbytečné objekty Master.");
			Assert.AreEqual(1, dbContext.Child.Local.Count, "DbDataLoader načetl zbytečné objekty Child.");
		}

		[TestMethod]
		public void DbDataLoader_Load_IncludesDeleted()
		{
			// Arrange
			SeedOneToManyTestData(deleted: true);

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			Child child = dbContext.Child.First();

			Assert.IsNull(child.Parent, "Pro ověření DbDataLoaderu se předpokládá, že hodnota child.Parent je null.");
			Assert.AreNotEqual(0, child.ParentId, "Pro ověření DbDataLoaderu se předpokládá, že hodnota child.ParentId není nula.");

			// Act
			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager());
			dataLoader.Load(child, item => item.Parent);

			// Assert
			Assert.IsNotNull(child.Parent, "DbDataLoader nenačetl hodnotu pro child.Parent.");
		}

		[TestMethod]
		public void DbDataLoader_Load_OneToMany_LoadsPartiallyInitializedCollections()
		{
			// Arrange
			SeedOneToManyTestData();

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			Master master = dbContext.Master.First();
			Child child = dbContext.Child.Where(item => item.ParentId == master.Id).First();

			Assert.IsNotNull(master.Children, "Pro ověření DbDataLoaderu se předpokládá, že hodnota master.Children je (částečně) initializovaná.");
			Assert.AreEqual(1, master.Children.Count, "Pro ověření DbDataLoaderu se předpokládá, že hodnota master.Children obsahuje jeden prvek.");

			// Act
			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager());
			dataLoader.Load(master, item => item.Children);

			// Assert
			Assert.AreEqual(5, master.Children.Count, "DbDataLoader nenačetl objekty do master.Children.");
		}

		[TestMethod]
		public void DbDataLoader_Load_OneToMany_LoadsNotLoadedCollections()
		{
			// Arrange
			SeedOneToManyTestData();

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			Master master = dbContext.Master.First();

			Assert.IsNull(master.Children, "Pro ověření DbDataLoaderu se předpokládá, že hodnota master.Children je null.");

			// Act
			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager());
			dataLoader.Load(master, item => item.Children);

			// Assert
			Assert.IsNotNull(master.Children, "DbDataLoader nenačetl hodnotu pro master.Children.");
			Assert.AreEqual(5, master.Children.Count, "DbDataLoader nenačetl objekty do master.Children.");
		}
		
		[TestMethod]
		public void DbDataLoader_Load_OneToMany_DoesNotLoadExcessEntities()
		{
			// Arrange
			SeedOneToManyTestData(deleted: false);

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			Master master = dbContext.Master.First();

			Assert.IsNull(master.Children, "Pro ověření DbDataLoaderu se předpokládá, že hodnota master.Children je null.");

			// Act
			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager());
			dataLoader.Load(master, item => item.Children);

			// Assert
			Assert.AreEqual(1, dbContext.Master.Local.Count, "DbDataLoader načetl zbytečné objekty Master.");
			Assert.AreEqual(5, dbContext.Child.Local.Count, "DbDataLoader načetl zbytečné objekty Child.");
		}

		[TestMethod]
		public void DbDataLoader_Load_ManyToMany_LoadsNotLoadedCollections()
		{
			// Arrange
			SeedManyToManyTestData(false);

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			LoginAccount loginAccount = dbContext.LoginAccount.First();

			Assert.IsNull(loginAccount.Roles, "Pro ověření DbDataLoaderu se předpokládá, že hodnota loginAccount.Roles je null.");

			// Act
			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager());
			dataLoader.Load(loginAccount, item => item.Roles);

			// Assert
			Assert.IsNotNull(loginAccount.Roles, "DbDataLoader nenačetl hodnotu pro loginAccount.Roles.");
			Assert.AreEqual(1, loginAccount.Roles.Count, "DbDataLoader nenačetl objekty do loginAccount.Roles.");
		}

		[TestMethod]
		public void DbDataLoader_Load_DoesNotLoadAlreadyLoaded()
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
			IDataLoader dataLoader = new DbDataLoader(dbContextMock.Object, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager());
			dataLoader.Load(child, item => item.Parent);

			// Assert
			// říkáme, že aby mohl DbDataLoader načíst data, musí si získat DbSet pomocí metody Set
			// takže testujeme, že na ní ani nešáhl
			dbContextMock.Verify(m => m.Set<Child>(), Times.Never);
			dbContextMock.Verify(m => m.Set<Master>(), Times.Never);
		}

		[TestMethod]
		public void DbDataLoader_LoadCollection_OneToMany_DoesNotLoadAlreadyLoaded()
		{
			// Arrange
			Mock<DataLoaderTestDbContext> dbContextMock = new Mock<DataLoaderTestDbContext>();
			dbContextMock.CallBase = true;
			
			SeedOneToManyTestData(dbContext: dbContextMock.Object);

			Master master = dbContextMock.Object.Master.First();

			DbDataLoader dbDataLoader = new DbDataLoader(dbContextMock.Object, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager());

			// Act + Assert

			dbContextMock.Verify(m => m.Set<Master>(), Times.Never);

			dbDataLoader.Load(master, item => item.Children);

			// Říkáme, že aby mohl DbDataLoader načíst data pro Master, musí si získat DbSet<Master> pomocí metody Set<Master>().
			// Takže testujeme, že se na ní ani šáhl pouze jedenkrát - zde při prvním načtení kolekce.
			dbContextMock.Verify(m => m.Set<Master>(), Times.Once);

			dbDataLoader.Load(master, item => item.Children);

			// Říkáme, že aby mohl DbDataLoader načíst data pro Master, musí si získat DbSet<Master> pomocí metody Set<Master>().
			// Takže testujeme, že se na ní ani šáhl pouze jedenkrát výše, a již vícekrát ne.
			dbContextMock.Verify(m => m.Set<Master>(), Times.Once);

			// childy nenačítáme, takže jen ověříme, že se nikdy na Set<Child> nešáhlo
			dbContextMock.Verify(m => m.Set<Child>(), Times.Never);
		}

		[TestMethod]
		public void DbDataLoader_Load_OneToMany_LoadsChains()
		{
			// Arrange
			SeedOneToManyTestData();

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			DbDataLoader dbDataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager());

			Child child = dbContext.Child.First();

			// Act
			dbDataLoader.Load(child, c => c.Parent.Children);

			// Assert
			Assert.IsNotNull(child.Parent);
			Assert.IsNotNull(child.Parent.Children);
		}

		[TestMethod]
		public void DbDataLoader_Load_ManyToMany_LoadChains()
		{
			// Arrange
			SeedManyToManyTestData(false);

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			LoginAccount loginAccount = dbContext.LoginAccount.First();

			Assert.IsNull(loginAccount.Roles, "Pro ověření DbDataLoaderu se předpokládá, že hodnota loginAccount.Roles je null.");

			// Act
			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager());
			dataLoader.Load(loginAccount, item => item.Roles).ThenLoad(membership => membership.Role);

			// Assert
			Assert.IsNotNull(loginAccount.Roles, "DbDataLoader nenačetl hodnotu pro loginAccount.Roles.");
			Assert.AreEqual(1, loginAccount.Roles.Count, "DbDataLoader nenačetl objekty do loginAccount.Roles.");
			Assert.IsNotNull(loginAccount.Roles[0].Role, "DbDataLoader nenačetl hodnotu pro loginAccount.Roles.Role.");
		}

		[TestMethod]
		public void DbDataLoader_LoadAndThenLoad_LoadsChains()
		{
			// Arrange
			SeedOneToManyTestData();

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			DbDataLoader dbDataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager());

			Child child = dbContext.Child.First();

			// Act
			dbDataLoader.Load(child, c => c.Parent).ThenLoad(p => p.Children).ThenLoad(c => c.Parent);

			// Assert
			Assert.IsNotNull(child.Parent);
			Assert.IsNotNull(child.Parent.Children);
			Assert.IsTrue(child.Parent.Children.All(item => item.Parent != null));
		}

		[TestMethod]
		public async Task DbDataLoader_LoadAsyncAndThenLoadAsync_LoadsChains()
		{
			// Arrange
			SeedOneToManyTestData();

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			DbDataLoader dbDataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager());

			Child child = dbContext.Child.First();

			// Act
			await dbDataLoader.LoadAsync(child, c => c.Parent).ThenLoadAsync(p => p.Children).ThenLoadAsync(c => c.Parent);

			// Assert
			Assert.IsNotNull(child.Parent);
			Assert.IsNotNull(child.Parent.Children);
			Assert.IsTrue(child.Parent.Children.All(item => item.Parent != null));
		}

		[TestMethod]
		public async Task DbDataLoader_LoadAsync_OneToMany_LoadsChains()
		{
			// Arrange
			SeedOneToManyTestData();

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			DbDataLoader dbDataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager());

			Child child = await dbContext.Child.FirstAsync();

			// Act
			await dbDataLoader.LoadAsync(child, c => c.Parent.Children);

			// Assert
			Assert.IsNotNull(child.Parent);
			Assert.IsNotNull(child.Parent.Children);
		}

		[TestMethod]
		public async Task DbDataLoader_Load_ManyToMany_ManyToMany_LoadChains()
		{
			// Arrange
			SeedManyToManyTestData(false);

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			LoginAccount loginAccount = dbContext.LoginAccount.First();

			Assert.IsNull(loginAccount.Roles, "Pro ověření DbDataLoaderu se předpokládá, že hodnota loginAccount.Roles je null.");

			// Act
			DbDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager());
			await dataLoader.LoadAsync(loginAccount, item => item.Roles).ThenLoadAsync(membership => membership.Role);

			// Assert
			Assert.IsNotNull(loginAccount.Roles, "DbDataLoader nenačetl hodnotu pro loginAccount.Roles.");
			Assert.AreEqual(1, loginAccount.Roles.Count, "DbDataLoader nenačetl objekty do loginAccount.Roles.");
			Assert.IsNotNull(loginAccount.Roles[0].Role, "DbDataLoader nenačetl hodnotu pro loginAccount.Roles.Role.");
		}

		// TODO JK: Dodělat async podporu
		//[TestMethod]
		//public async Task DbDataLoader_LoadAsyncAndThenLoadAsync_LoadsChains()
		//{
		//	// Arrange
		//	SeedOneToManyTestData();

		//	DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
		//	DbDataLoader dbDataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));

		//	Child child = await dbContext.Child.FirstAsync();

		//	// Act
		//	await dbDataLoader.LoadAsync(child, c => c.Parent).ThenLoadAsync(p => p.Children).ThenLoadAsync(c => c.Parent);

		//	// Assert
		//	Assert.IsNotNull(child.Parent);
		//	Assert.IsNotNull(child.Parent.Children);
		//	Assert.IsTrue(child.Parent.Children.All(item => item.Parent != null));
		//}

		[TestMethod]
		public void DbDataLoader_Load_LoadWithManyAreReentrant()
		{
			// Arrange
			SeedOneToManyTestData();

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager());

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
		public void DbDataLoader_Load_SkipsNull()
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
		public void DbDataLoader_Load_SupportsNullValuesInData()
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

		[TestMethod]
		public void DbDataLoader_Load_OneToMany_OnNewObject()
		{
			// Arrange
			Mock<DataLoaderTestDbContext> dbContextMock = new Mock<DataLoaderTestDbContext>();
			dbContextMock.CallBase = true;

			dbContextMock.Object.Database.DropCreate();

            Child child = new Child();

			dbContextMock.Object.Child.Add(child);

			DbDataLoader dataLoader = new DbDataLoader(dbContextMock.Object, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager());

			// Act
			dataLoader.Load(child, item => item.Parent);
            
			// Assert
			dbContextMock.Verify(m => m.Set<Child>(), Times.Never);
		}

		[TestMethod]
	    public async Task DbDataLoader_LoadAsync_OneToMany_OnNewObject()
	    {
		    // Arrange
		    Mock<DataLoaderTestDbContext> dbContextMock = new Mock<DataLoaderTestDbContext>();
		    dbContextMock.CallBase = true;

		    dbContextMock.Object.Database.DropCreate();

		    Child child = new Child();

		    dbContextMock.Object.Child.Add(child);

		    DbDataLoader dataLoader = new DbDataLoader(dbContextMock.Object, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager());

		    // Act
		    await dataLoader.LoadAsync(child, item => item.Parent);

		    // Assert
		    dbContextMock.Verify(m => m.Set<Child>(), Times.Never);
		}

		//[TestMethod]
		//public void DbDataLoader_Load_ManyToMany_OnNewObject_WithoutInitializedCollection()
		//{
		//    // Arrange
		//    DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
		//    dbContext.Database.DropCreate();
		//    LoginAccount loginAccount = new LoginAccount();

		//    dbContext.LoginAccount.Add(loginAccount);

		//    IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));

		//    // Precondition            
		//    Assert.IsNull(loginAccount.Roles);

		//    // Act
		//       ExecuteWithDisabledDatabaseOperations(dbContext, () =>
		//       {
		//        dataLoader.Load(loginAccount, item => item.Roles);
		//       });

		//    // Assert
		//    // no exception was thrown
		//    Assert.IsNotNull(loginAccount.Roles);
		//}

	    //[TestMethod]		
	    //public async Task DbDataLoader_LoadAsync_ManyToMany_OnNewObject_WithoutInitializedCollection()
	    //{
	    //    // Arrange
	    //    DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
		//    dbContext.Database.DropCreate();
	    //    LoginAccount loginAccount = new LoginAccount();

	    //    dbContext.LoginAccount.Add(loginAccount);

	    //    IDataLoaderAsync dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
		    
	    //    // Precondition            
	    //    Assert.IsNull(loginAccount.Roles);
			
	    //    // Act
	    //    await ExecuteWithDisabledDatabaseOperationsAsync(dbContext, async () =>
	    //    {
	    //        await dataLoader.LoadAsync(loginAccount, item => item.Roles);
	    //    });

	    //    // Assert
	    //    // no exception was thrown
	    //    Assert.IsNotNull(loginAccount.Roles);
	    //}

		//   [TestMethod]
		//public void DbDataLoader_Load_ManyToMany_OnNewObject_WithInitializedCollection()
		//{
		//    // Arrange
		//    DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
		//    dbContext.Database.DropCreate();
		//    LoginAccount loginAccount = new LoginAccount();
		//       List<Role> roles = new List<Role>();
		//    loginAccount.Roles = roles;

		//    dbContext.LoginAccount.Add(loginAccount);

		//    IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
	
		//       // Precondition            
		//       Assert.IsNotNull(loginAccount.Roles);
	
		//       // Act
		//    ExecuteWithDisabledDatabaseOperations(dbContext, () =>
		//    {
		//        dataLoader.Load(loginAccount, item => item.Roles);
		//    });

		//       // Assert
		//    Assert.AreSame(roles, loginAccount.Roles); // instance nebyla vyměněna
		//}

		//   [TestMethod]
		//public async Task DbDataLoader_LoadAsync_ManyToMany_OnNewObject_WithInitializedCollection()
		//{
		//    // Arrange
		//    DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
		//    dbContext.Database.DropCreate();
		//    LoginAccount loginAccount = new LoginAccount();
		//    List<Role> roles = new List<Role>();
		//    loginAccount.Roles = roles;

		//    dbContext.LoginAccount.Add(loginAccount);

		//    IDataLoaderAsync dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));

		//    // Precondition            
		//    Assert.IsNotNull(loginAccount.Roles);

		//    // Act
		//    await ExecuteWithDisabledDatabaseOperationsAsync(dbContext, async () =>
		//    {
		//        await dataLoader.LoadAsync(loginAccount, item => item.Roles);
		//    });

		//    // Assert
		//    Assert.AreSame(roles, loginAccount.Roles); // instance nebyla vyměněna
		//}

        [TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void DbDataLoader_Load_ThrowsExceptionForNontrackedObjects()
		{
			// Arrange
			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
		    dbContext.Database.DropCreate();
			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager());

			// Act
			dataLoader.Load(new Child() /* nontracked object */, item => item.Parent);

			// Assert by method attribute 
		}

		private void SeedOneToManyTestData(DataLoaderTestDbContext dbContext = null, bool deleted = false)
		{
			if (dbContext == null)
			{
				dbContext = new DataLoaderTestDbContext();
			}

			dbContext.Database.DropCreate();

			for (int i = 0; i < 5; i++)
			{
				Master master = new Master();
				if (deleted)
				{
					master.Deleted = DateTime.Now;
				}

				for (int j = 0; j < 5; j++)
				{
					Child child = new Child();
					child.Parent = master;
					if (deleted)
					{
						child.Deleted = DateTime.Now;
					}

					dbContext.Master.Add(master);
					dbContext.Child.Add(child);
				}
			}

			dbContext.SaveChanges();
		}

		private void SeedManyToManyTestData(bool deleted = false)
		{
			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			dbContext.Database.DropCreate();

			for (int i = 0; i < 5; i++)
			{
				Role role = new Role();
				if (deleted)
				{
					role.Deleted = DateTime.Now;
				}

				LoginAccount loginAccount = new LoginAccount();
				loginAccount.Roles = new List<Membership> { new Membership { LoginAccount = loginAccount, Role = role } };
				if (deleted)
				{
					loginAccount.Deleted = DateTime.Now;
				}

				dbContext.LoginAccount.Add(loginAccount);
				dbContext.Role.Add(role);
			}

			dbContext.SaveChanges();
		}
	}
}
