using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Havit.Data.Entity.Patterns.DataLoaders;
using Havit.Data.Entity.Patterns.DataLoaders.Internal;
using Havit.Data.Entity.Patterns.Tests.DataLoader.Model;
using Havit.Data.Entity.Patterns.Tests.Helpers;
using Havit.Data.Patterns.DataLoaders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.Entity.Patterns.Tests.DataLoader
{
	[TestClass]
	public class DbDataLoaderTests
	{
		[ClassCleanup]
		public static void CleanUp()
		{
			DeleteDatabaseHelper.DeleteDatabase<DataLoaderTestDbContext>();
		}

		[TestMethod]
		public void DbDataLoader_Load_LoadsNotLoadedEntities()
		{
			// Arrange
			SeedOneToManyTestData(false);

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			Child child = dbContext.Child.First();

			Assert.IsNull(child.Parent, "Pro ověření DbDataLoaderu se předpokládá, že hodnota child.Parent je null.");
			Assert.AreNotEqual(0, child.ParentId, "Pro ověření DbDataLoaderu se předpokládá, že hodnota child.ParentId není nula.");

			// Act
			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
			dataLoader.Load(child, item => item.Parent);

			// Assert
			Assert.IsNotNull(child.Parent, "DbDataLoader nenačetl hodnotu pro child.Parent.");
		}

		[TestMethod]
		public void DbDataLoader_Load_DoesNotLoadExcessEntities()
		{
			// Arrange
			SeedOneToManyTestData(false);

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			Child child = dbContext.Child.First();

			Assert.IsNull(child.Parent, "Pro ověření DbDataLoaderu se předpokládá, že hodnota child.Parent je null.");
			Assert.AreNotEqual(0, child.ParentId, "Pro ověření DbDataLoaderu se předpokládá, že hodnota child.ParentId není nula.");

			// Act
			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
			dataLoader.Load(child, item => item.Parent);

			// Assert
			Assert.AreEqual(1, dbContext.Master.Local.Count, "DbDataLoader načetl zbytečné objekty Master.");
			Assert.AreEqual(1, dbContext.Child.Local.Count, "DbDataLoader načetl zbytečné objekty Child.");
		}

		[TestMethod]
		public void DbDataLoader_Load_IncludesDeleted()
		{
			// Arrange
			SeedOneToManyTestData(true);

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			Child child = dbContext.Child.First();

			Assert.IsNull(child.Parent, "Pro ověření DbDataLoaderu se předpokládá, že hodnota child.Parent je null.");
			Assert.AreNotEqual(0, child.ParentId, "Pro ověření DbDataLoaderu se předpokládá, že hodnota child.ParentId není nula.");

			// Act
			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
			dataLoader.Load(child, item => item.Parent);

			// Assert
			Assert.IsNotNull(child.Parent, "DbDataLoader nenačetl hodnotu pro child.Parent.");
		}

		[TestMethod]
		public void DbDataLoader_Load_WithList_OneToMany_LoadsPartiallyInitializedCollections()
		{
			// Arrange
			SeedOneToManyTestData();

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			Master master = dbContext.Master.First();
			Child child = dbContext.Child.Where(item => item.ParentId == master.Id).First();

			Assert.IsNotNull(master.Children, "Pro ověření DbDataLoaderu se předpokládá, že hodnota master.Children je (částečně) initializovaná.");
			Assert.AreEqual(1, master.Children.Count, "Pro ověření DbDataLoaderu se předpokládá, že hodnota master.Children obsahuje jeden prvek.");

			// Act
			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
			dataLoader.Load(master, item => item.Children);

			// Assert
			Assert.AreEqual(5, master.Children.Count, "DbDataLoader nenačetl objekty do master.Children.");
		}

		[TestMethod]
		public void DbDataLoader_Load_WithList_OneToMany_LoadsNotLoadedCollections()
		{
			// Arrange
			SeedOneToManyTestData();

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			Master master = dbContext.Master.First();

			Assert.IsNull(master.Children, "Pro ověření DbDataLoaderu se předpokládá, že hodnota master.Children je null.");

			// Act
			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
			dataLoader.Load(master, item => item.Children);

			// Assert
			Assert.IsNotNull(master.Children, "DbDataLoader nenačetl hodnotu pro master.Children.");
			Assert.AreEqual(5, master.Children.Count, "DbDataLoader nenačetl objekty do master.Children.");
		}

		[TestMethod]
		public void DbDataLoader_Load_WithList_ManyToMany_LoadsNotLoadedCollections()
		{
			// Arrange
			SeedManyToManyTestData(false);

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			LoginAccount loginAccount = dbContext.LoginAccount.First();

			Assert.IsNull(loginAccount.Roles, "Pro ověření DbDataLoaderu se předpokládá, že hodnota loginAccount.Roles je null.");

			// Act
			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
			dataLoader.Load(loginAccount, item => item.Roles);

			// Assert
			Assert.IsNotNull(loginAccount.Roles, "DbDataLoader nenačetl hodnotu pro loginAccount.Roles.");
			Assert.AreEqual(1, loginAccount.Roles.Count, "DbDataLoader nenačetl objekty do loginAccount.Roles.");
		}

		[TestMethod]
		public void DbDataLoader_Load_WithObject_OneToMany_LoadsNotLoadedCollections()
		{
			// Arrange
			SeedOneToManyTestData();

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			Master master = dbContext.Master.First();

			Assert.IsNull(master.Children, "Pro ověření DbDataLoaderu se předpokládá, že hodnota master.Children je null.");

			// Act
			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));

			dataLoader.Load(master, item => item.Children);

			// Assert
			Assert.IsNotNull(master.Children, "DbDataLoader nenačetl hodnotu pro master.Children.");
			Assert.AreEqual(5, master.Children.Count, "DbDataLoader nenačetl objekty do master.Children.");
		}

		[TestMethod]
		public void DbDataLoader_Load_WithObject_ManyToMany_LoadsNotLoadedCollections()
		{
			// Arrange
			SeedManyToManyTestData(false);

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			LoginAccount loginAccount = dbContext.LoginAccount.First();

			Assert.IsNull(loginAccount.Roles, "Pro ověření DbDataLoaderu se předpokládá, že hodnota loginAccount.Roles je null.");

			// Act
			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
			dataLoader.Load(loginAccount, item => item.Roles);

			// Assert
			Assert.IsNotNull(loginAccount.Roles, "DbDataLoader nenačetl hodnotu pro loginAccount.Roles.");
			Assert.AreEqual(1, loginAccount.Roles.Count, "DbDataLoader nenačetl objekty do loginAccount.Roles.");
		}

		[TestMethod]
		public void DbDataLoader_Load_WithList_OneToMany_DoesNotLoadExcessEntities()
		{
			// Arrange
			SeedOneToManyTestData(false);

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			Master master = dbContext.Master.First();

			Assert.IsNull(master.Children, "Pro ověření DbDataLoaderu se předpokládá, že hodnota master.Children je null.");

			// Act
			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
			dataLoader.Load(master, item => item.Children);

			// Assert
			Assert.AreEqual(1, dbContext.Master.Local.Count, "DbDataLoader načetl zbytečné objekty Master.");
			Assert.AreEqual(5, dbContext.Child.Local.Count, "DbDataLoader načetl zbytečné objekty Child.");
		}

		[TestMethod]
		public void DbDataLoader_Load_WithList_ManyToMany_DoesNotLoadExcessEntities()
		{
			// Arrange
			SeedManyToManyTestData(false);

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			LoginAccount loginAccount = dbContext.LoginAccount.First();

			Assert.IsNull(loginAccount.Roles, "Pro ověření DbDataLoaderu se předpokládá, že hodnota loginAccount.Roles je null.");

			// Act
			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
			dataLoader.Load(loginAccount, item => item.Roles);

			// Assert
			Assert.AreEqual(1, dbContext.LoginAccount.Local.Count, "DbDataLoader načetl zbytečné objekty LoginAccount.");
			Assert.AreEqual(1, dbContext.Role.Local.Count, "DbDataLoader načetl zbytečné objekty Role.");
		}

		[TestMethod]
		public void DbDataLoader_Load_DoesNotLoadAlreadyLoaded()
		{
			// Arrange
			SeedOneToManyTestData(false);

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			Child child = dbContext.Child
				.Include(c => c.Parent)
				.First();

			Mock<IDbContext> mockDbContext = new Mock<IDbContext>();
			mockDbContext.Setup(m => m.IsEntityReferenceLoaded<Child>(It.IsAny<Child>(), It.IsAny<string>())).Returns<Child, string>((entity, propertyName) => ((IDbContext)dbContext).IsEntityReferenceLoaded(entity, propertyName));
			//mockDbContext.Setup(m => m.GetEntityState(child)).Returns(EntityState.Unchanged);

			// Act
			IDataLoader dataLoader = new DbDataLoader(mockDbContext.Object, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
			dataLoader.Load(child, item => item.Parent);

			// Assert
			// říkáme, že aby mohl DbDataLoader načíst data, musí si získat DbSet pomocí metody Set
			// takže testujeme, že na ní ani nešáhl
			mockDbContext.Verify(m => m.Set<Child>(), Times.Never);
			mockDbContext.Verify(m => m.Set<Master>(), Times.Never);
		}

		[TestMethod]
		public void DbDataLoader_Load_OneToMany_DoesNotLoadAlreadyLoaded()
		{
			// Arrange
			SeedManyToManyTestData(false);

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			LoginAccount loginAccount = dbContext.LoginAccount.First();

			Mock<IDbContext> mockDbContext = new Mock<IDbContext>();
			mockDbContext.Setup(m => m.Set<LoginAccount>()).Returns(dbContext.Set<LoginAccount>());
			mockDbContext.Setup(m => m.Set<Role>()).Returns(dbContext.Set<Role>());
			mockDbContext.Setup(m => m.IsEntityCollectionLoaded<LoginAccount>(It.IsAny<LoginAccount>(), It.IsAny<string>())).Returns<LoginAccount, string>((entity, propertyName) => ((IDbContext)dbContext).IsEntityCollectionLoaded(entity, propertyName));

			// Act + Assert
			IDataLoader dataLoader = new DbDataLoader(mockDbContext.Object, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));

			// Říkáme, že aby mohl DbDataLoader načíst data pro LoginAccount, musí si získat DbSet<LoginAccount> pomocí metody Set<LoginAccount>().
			// Takže testujeme, že se na ní ani šáhl pouze jedenkrát - zde.
			dataLoader.Load(loginAccount, item => item.Roles);
			mockDbContext.Verify(m => m.Set<LoginAccount>(), Times.Once);

			// Říkáme, že aby mohl DbDataLoader načíst data pro LoginAccount, musí si získat DbSet<LoginAccount> pomocí metody Set<LoginAccount>().
			// Takže testujeme, že se na ní ani šáhl pouze jedenkrát výše, a již vícekrát ne.
			dataLoader.Load(loginAccount, item => item.Roles);
			mockDbContext.Verify(m => m.Set<LoginAccount>(), Times.Once);

			// rolím nic nenačítáme, takže jen ověříme, že se nikdy na Set<Role> nešáhlo
			mockDbContext.Verify(m => m.Set<Role>(), Times.Never);
		}

		[TestMethod]
		public void DbDataLoader_LoadCollection_ManyToMany_DoesNotLoadAlreadyLoaded()
		{
			// Arrange
			SeedOneToManyTestData(false);

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			Master master = dbContext.Master.First();

			Mock<IDbContext> mockDbContext = new Mock<IDbContext>();
			mockDbContext.Setup(m => m.Set<Child>()).Returns(dbContext.Set<Child>());
			mockDbContext.Setup(m => m.Set<Master>()).Returns(dbContext.Set<Master>());
			mockDbContext.Setup(m => m.IsEntityCollectionLoaded<Master>(It.IsAny<Master>(), It.IsAny<string>())).Returns<Master, string>((entity, propertyName) => ((IDbContext)dbContext).IsEntityCollectionLoaded(entity, propertyName));
			DataLoaders.DbDataLoader dbDataLoader = new DbDataLoader(mockDbContext.Object, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));

			// Act + Assert

			dbDataLoader.Load(master, item => item.Children);

			// Říkáme, že aby mohl DbDataLoader načíst data pro Master, musí si získat DbSet<Master> pomocí metody Set<Master>().
			// Takže testujeme, že se na ní ani šáhl pouze jedenkrát - zde při prvním načtení kolekce.
			mockDbContext.Verify(m => m.Set<Master>(), Times.Once);

			dbDataLoader.Load(master, item => item.Children);

			// Říkáme, že aby mohl DbDataLoader načíst data pro Master, musí si získat DbSet<Master> pomocí metody Set<Master>().
			// Takže testujeme, že se na ní ani šáhl pouze jedenkrát výše, a již vícekrát ne.
			mockDbContext.Verify(m => m.Set<Master>(), Times.Once);

			// childy nenačítáme, takže jen ověříme, že se nikdy na Set<Child> nešáhlo
			mockDbContext.Verify(m => m.Set<Child>(), Times.Never);
		}

		[TestMethod]
		public void DbDataLoader_Load_LoadsChains()
		{
			// Arrange
			SeedOneToManyTestData();

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			DataLoaders.DbDataLoader dbDataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));

			Child child = dbContext.Child.First();

			// Act
			dbDataLoader.Load(child, c => c.Parent.Children.Select(item => item.Parent));

			// Assert
			Assert.IsNotNull(child.Parent);
			Assert.IsNotNull(child.Parent.Children);
			Assert.IsTrue(child.Parent.Children.All(item => item.Parent != null));
		}

		[TestMethod]
		public void DbDataLoader_Load_LoadsHierarchy()
		{
			// Arrange
			SeedHierarchyTestData();

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			DataLoaders.DbDataLoader dbDataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));

			HiearchyItem hiearchyItem = dbContext.HiearchyItem.Where(item => item.ParentId == null).First();

			// Act
			dbDataLoader.Load(hiearchyItem, hi => hi.Children.Select(hi2 => hi2.Children.Select(hi3 => hi3.Children)));

			// Assert
			Assert.IsNotNull(hiearchyItem.Children);
			Assert.AreEqual(3, hiearchyItem.Children.Count);
			Assert.AreEqual(3, hiearchyItem.Children.First().Children.Count);
			Assert.AreEqual(3, hiearchyItem.Children.First().Children.First().Children.Count);
		}

		[TestMethod]
		public async Task DbDataLoader_LoadAsync_LoadsChains()
		{
			// Arrange
			SeedOneToManyTestData();

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			DataLoaders.DbDataLoader dbDataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));

			Child child = await dbContext.Child.FirstAsync();

			// Act
			await dbDataLoader.LoadAsync(child, c => c.Parent.Children.Select(item => item.Parent));

			// Assert
			Assert.IsNotNull(child.Parent);
			Assert.IsNotNull(child.Parent.Children);
			Assert.IsTrue(child.Parent.Children.All(item => item.Parent != null));
		}

		[TestMethod]
		public void DbDataLoader_Load_LoadWithManyAreReentrant()
		{
			// Arrange
			SeedOneToManyTestData();

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));

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
			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));

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
			dbContext1.Database.Initialize(true);

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
			IDataLoader dataLoader = new DbDataLoader(dbContext2, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));

			dataLoader.LoadAll(childs, child => child.Parent.Children);

			// Assert - no exception was thrown
		}

		[TestMethod]
		public void DbDataLoader_Load_OneToMany_OnNewObject()
		{
			// Arrange
			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();

            Child child = new Child();

			dbContext.Child.Add(child);

		    IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));

            ExecuteWithDisabledDatabaseOperations(dbContext, () =>
		    {
		        // Act
		        dataLoader.Load(child, item => item.Parent);
            });

			// Assert
			// no exception was thrown
		}

	    [TestMethod]
	    public async Task DbDataLoader_LoadAsync_OneToMany_OnNewObject()
	    {
	        // Arrange
	        DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();

	        Child child = new Child();

	        dbContext.Child.Add(child);

	        await ExecuteWithDisabledDatabaseOperationsAsync(dbContext, async () =>
	        {
	            // Act
	            IDataLoaderAsync dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));

	            await dataLoader.LoadAsync(child, item => item.Parent);
	        });

	        // Assert
	        // no exception was thrown
	    }

	    [TestMethod]
	    public void DbDataLoader_Load_ManyToMany_OnNewObject_WithoutInitializedCollection()
	    {
	        // Arrange
	        DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
	        LoginAccount loginAccount = new LoginAccount();

	        dbContext.LoginAccount.Add(loginAccount);

	        IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));

	        // Precondition            
	        Assert.IsNull(loginAccount.Roles);

	        // Act
            ExecuteWithDisabledDatabaseOperations(dbContext, () =>
            {
    	        dataLoader.Load(loginAccount, item => item.Roles);
            });

	        // Assert
	        // no exception was thrown
	        Assert.IsNotNull(loginAccount.Roles);
	    }

	    [TestMethod]		
	    public async Task DbDataLoader_LoadAsync_ManyToMany_OnNewObject_WithoutInitializedCollection()
	    {
	        // Arrange
	        DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
	        LoginAccount loginAccount = new LoginAccount();

	        dbContext.LoginAccount.Add(loginAccount);

	        IDataLoaderAsync dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
		    
	        // Precondition            
	        Assert.IsNull(loginAccount.Roles);
			
	        // Act
	        await ExecuteWithDisabledDatabaseOperationsAsync(dbContext, async () =>
	        {
	            await dataLoader.LoadAsync(loginAccount, item => item.Roles);
	        });

	        // Assert
	        // no exception was thrown
	        Assert.IsNotNull(loginAccount.Roles);
	    }

        [TestMethod]
	    public void DbDataLoader_Load_ManyToMany_OnNewObject_WithInitializedCollection()
	    {
	        // Arrange
	        DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
	        LoginAccount loginAccount = new LoginAccount();
            List<Role> roles = new List<Role>();
	        loginAccount.Roles = roles;

	        dbContext.LoginAccount.Add(loginAccount);

	        IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));
	
            // Precondition            
            Assert.IsNotNull(loginAccount.Roles);
	
            // Act
	        ExecuteWithDisabledDatabaseOperations(dbContext, () =>
	        {
	            dataLoader.Load(loginAccount, item => item.Roles);
	        });

            // Assert
	        Assert.AreSame(roles, loginAccount.Roles); // instance nebyla vyměněna
	    }

        [TestMethod]
	    public async Task DbDataLoader_LoadAsync_ManyToMany_OnNewObject_WithInitializedCollection()
	    {
	        // Arrange
	        DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
	        LoginAccount loginAccount = new LoginAccount();
	        List<Role> roles = new List<Role>();
	        loginAccount.Roles = roles;

	        dbContext.LoginAccount.Add(loginAccount);

	        IDataLoaderAsync dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));

	        // Precondition            
	        Assert.IsNotNull(loginAccount.Roles);

	        // Act
	        await ExecuteWithDisabledDatabaseOperationsAsync(dbContext, async () =>
	        {
	            await dataLoader.LoadAsync(loginAccount, item => item.Roles);
	        });

	        // Assert
	        Assert.AreSame(roles, loginAccount.Roles); // instance nebyla vyměněna
	    }

        [TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void DbDataLoader_Load_ThrowsExceptionForNontrackedObjects()
		{
			// Arrange
			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolver(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()));

			// Act
			dataLoader.Load(new Child() /* nontracked object */, item => item.Parent);

			// Assert by method attribute 
		}

		private void SeedOneToManyTestData(bool deleted = false)
		{
			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			dbContext.Database.Initialize(true);

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
			dbContext.Database.Initialize(true);

			for (int i = 0; i < 5; i++)
			{
				Role role = new Role();
				if (deleted)
				{
					role.Deleted = DateTime.Now;
				}

				LoginAccount loginAccount = new LoginAccount();
				loginAccount.Roles = new List<Role> { role };
				if (deleted)
				{
					loginAccount.Deleted = DateTime.Now;
				}

				dbContext.LoginAccount.Add(loginAccount);
				dbContext.Role.Add(role);
			}

			dbContext.SaveChanges();
		}

		private void SeedHierarchyTestData()
		{
			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			dbContext.Database.Initialize(true);

			HiearchyItem root = new HiearchyItem();
			SeedHierarchyTestData_CreateItems(root, 0);

			dbContext.HiearchyItem.Add(root);
			dbContext.SaveChanges();
		}

		private void SeedHierarchyTestData_CreateItems(HiearchyItem root, int level)
		{
			if (level > 2)
			{
				return;
			}

			root.Children = new List<HiearchyItem>();
			for (int i = 0; i < 3; i++)
			{
				HiearchyItem hi = new HiearchyItem();
				SeedHierarchyTestData_CreateItems(hi, level + 1);
				root.Children.Add(hi);
			}
		}

	    private void ExecuteWithDisabledDatabaseOperations(DbContext dbContext, Action action)
	    {
	        var originalLog = dbContext.Database.Log;

            try
	        {
                dbContext.Database.Log = s => throw new InvalidOperationException("Databázové operace jsou zakázány.");
	            action();
	        }
	        finally
            {
                dbContext.Database.Log = originalLog;
            }
	    }

	    private async Task ExecuteWithDisabledDatabaseOperationsAsync(DbContext dbContext, Func<Task> action)
	    {
	        var originalLog = dbContext.Database.Log;

	        try
	        {
	            dbContext.Database.Log = s => throw new InvalidOperationException("Databázové operace jsou zakázány.");
	            await action();
	        }
	        finally
	        {
	            dbContext.Database.Log = originalLog;
	        }
	    }

    }
}
