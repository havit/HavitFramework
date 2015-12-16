using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Havit.Data.Entity.Patterns.DataLoaders;
using Havit.Data.Entity.Patterns.Tests.DataLoader.Model;
using Havit.Data.Patterns.DataLoaders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.Entity.Patterns.Tests.DataLoader
{
	[TestClass]
	public class DbDataLoaderTest
	{
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
			IDataLoader dataLoader = new DbDataLoader(dbContext);
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
			IDataLoader dataLoader = new DbDataLoader(dbContext);
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
			IDataLoader dataLoader = new DbDataLoader(dbContext);
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
			IDataLoader dataLoader = new DbDataLoader(dbContext);
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
			IDataLoader dataLoader = new DbDataLoader(dbContext);
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
			IDataLoader dataLoader = new DbDataLoader(dbContext);
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
			IDataLoader dataLoader = new DbDataLoader(dbContext);

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
			IDataLoader dataLoader = new DbDataLoader(dbContext);
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
			IDataLoader dataLoader = new DbDataLoader(dbContext);
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
			IDataLoader dataLoader = new DbDataLoader(dbContext);
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
			//mockDbContext.Setup(m => m.GetEntityState(child)).Returns(EntityState.Unchanged);

			// Act
			IDataLoader dataLoader = new DbDataLoader(mockDbContext.Object);
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
			//mockDbContext.Setup(m => m.GetEntityState(loginAccount)).Returns(EntityState.Unchanged);

			// Act + Assert
			IDataLoader dataLoader = new DbDataLoader(mockDbContext.Object);

			// Říkáme, že aby mohl DbDataLoader načíst data pro LoginAccount, musí si získat DbSet<LoginAccount> pomocí metody Set<LoginAccount>().
			// Takže testujeme, že se na ní ani šáhl pouze jedenkrát - zde.
			dataLoader.Load(loginAccount, item => item.Roles);
			mockDbContext.Verify(m => m.Set<LoginAccount>(), Times.Once);

			// Říkáme, že aby mohl DbDataLoader načíst data pro LoginAccount, musí si získat DbSet<LoginAccount> pomocí metody Set<LoginAccount>().
			// Takže testujeme, že se na ní ani šáhl pouze jedenkrát výše, a již vícekrát ne.
			dataLoader.Load(loginAccount, item => item.Roles);
			mockDbContext.Verify(m => m.Set<LoginAccount>(), Times.Once);

			// rolím nic nenačítáme, takže jen ověříme, že se nikdy na Set<Rike> nešáhlo
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
			DataLoaders.DbDataLoader dbDataLoader = new DbDataLoader(mockDbContext.Object);

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
			DataLoaders.DbDataLoader dbDataLoader = new DbDataLoader(dbContext);

			Child child = dbContext.Child.First();

			// Act
			dbDataLoader.Load(child, c => c.Parent.Children.Unwrap().Parent);

			// Assert
			Assert.IsNotNull(child.Parent);
			Assert.IsNotNull(child.Parent.Children);
			Assert.IsTrue(child.Parent.Children.All(item => item.Parent != null));
		}

		[TestMethod]
		public async Task DbDataLoader_LoadAsync_LoadsChains()
		{
			// Arrange
			SeedOneToManyTestData();

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			DataLoaders.DbDataLoader dbDataLoader = new DbDataLoader(dbContext);

			Child child = await dbContext.Child.FirstAsync();

			// Act
			await dbDataLoader.LoadAsync(child, c => c.Parent.Children.Unwrap().Parent);

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
			IDataLoader dataLoader = new DbDataLoader(dbContext);

			Master master1 = dbContext.Master.OrderBy(m => m.Id).First();
			Master master2 = dbContext.Master.OrderBy(m => m.Id).Skip(1).First();

			// Act

			dataLoader.Load(master1, m => m.Children.Unwrap());
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
			IDataLoader dataLoader = new DbDataLoader(dbContext);

			// Act
			dataLoader.Load((Child)null, c => c.Parent);
			dataLoader.LoadAll(new Child[] { null }, c => c.Parent);

			// Assert: No exception was thrown
		}

		[TestMethod]			
		public void DbDataLoader_Load_SupportsFluentApi()
		{
			// Arrange
			SeedOneToManyTestData();

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			IDataLoader dataLoader = new DbDataLoader(dbContext);

			// Act
			dataLoader.Load((Child)null, c => c.Parent).Load(m => m.Children.Unwrap()).Load(c => c.Parent);
			dataLoader.Load((Master)null, m => m.Children.Unwrap()).Load(c => c.Parent).Load(m => m.Children);

			// Assert: No exception was thrown
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void DbDataLoader_Load_ThrowsExceptionForNontrackedObjects()
		{
			// Arrange
			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			IDataLoader dataLoader = new DbDataLoader(dbContext);

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
	}
}
