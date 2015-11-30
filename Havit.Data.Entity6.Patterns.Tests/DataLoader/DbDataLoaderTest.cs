using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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
			IDbDataLoader dataLoader = new DbDataLoader(dbContext);
			dataLoader.For(child).Load(item => item.Parent);

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
			IDbDataLoader dataLoader = new DbDataLoader(dbContext);
			dataLoader.For(child).Load(item => item.Parent);

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
			IDbDataLoader dataLoader = new DbDataLoader(dbContext);
			dataLoader.For(child).Load(item => item.Parent);

			// Assert
			Assert.IsNotNull(child.Parent, "DbDataLoader nenačetl hodnotu pro child.Parent.");
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
			IDbDataLoader dataLoader = new DbDataLoader(dbContext);
			dataLoader.For(master).Load(item => item.Children);

			// Assert
			Assert.IsNotNull(master.Children, "DbDataLoader nenačetl hodnotu pro master.Children.");
			Assert.AreEqual(1, master.Children.Count, "DbDataLoader nenačetl objekty do master.Children.");
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
			IDbDataLoader dataLoader = new DbDataLoader(dbContext);
			dataLoader.For(loginAccount).Load(item => item.Roles);

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
			IDbDataLoader dataLoader = new DbDataLoader(dbContext);

			dataLoader.For(master).Load(item => item.Children);

			// Assert
			Assert.IsNotNull(master.Children, "DbDataLoader nenačetl hodnotu pro master.Children.");
			Assert.AreEqual(1, master.Children.Count, "DbDataLoader nenačetl objekty do master.Children.");
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
			IDbDataLoader dataLoader = new DbDataLoader(dbContext);
			dataLoader.For(loginAccount).Load(item => item.Roles);

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
			IDbDataLoader dataLoader = new DbDataLoader(dbContext);
			dataLoader.For(master).Load(item => item.Children);

			// Assert
			Assert.AreEqual(1, dbContext.Master.Local.Count, "DbDataLoader načetl zbytečné objekty Master.");
			Assert.AreEqual(1, dbContext.Child.Local.Count, "DbDataLoader načetl zbytečné objekty Child.");
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
			IDbDataLoader dataLoader = new DbDataLoader(dbContext);
			dataLoader.For(loginAccount).Load(item => item.Roles);

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
			IDbDataLoader dataLoader = new DbDataLoader(mockDbContext.Object);
			dataLoader.For(child).Load(item => item.Parent);

			// Assert
			// říkáme, že aby mohl DataLoader načíst data, musí si získat DbSet pomocí metody Set
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
			LoginAccount loginAccount = dbContext.LoginAccount
				.Include(la => la.Roles)
				.First();

			Mock<IDbContext> mockDbContext = new Mock<IDbContext>();
			//mockDbContext.Setup(m => m.GetEntityState(loginAccount)).Returns(EntityState.Unchanged);

			// Act
			IDbDataLoader dataLoader = new DbDataLoader(mockDbContext.Object);
			dataLoader.For(loginAccount).Load(item => item.Roles);

			// Assert
			// říkáme, že aby mohl DataLoader načíst data, musí si získat DbSet pomocí metody Set
			// takže testujeme, že na ní ani nešáhl
			mockDbContext.Verify(m => m.Set<LoginAccount>(), Times.Never);
			mockDbContext.Verify(m => m.Set<Role>(), Times.Never);
		}

		[TestMethod]
		public void DbDataLoader_LoadCollection_ManyToMany_DoesNotLoadAlreadyLoaded()
		{
			// Arrange
			SeedOneToManyTestData(false);

			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			Master master = dbContext.Master
				.Include(m => m.Children)
				.First();

			// Act
			Mock<IDbContext> mockDbContext = new Mock<IDbContext>();
			
			IDbDataLoader dataLoader = new DbDataLoader(mockDbContext.Object);
			dataLoader.For(master).Load(item => item.Children);

			// Assert
			// říkáme, že aby mohl DataLoader načíst data, musí si získat DbSet pomocí metody Set
			// takže testujeme, že na ní ani nešáhl
			mockDbContext.Verify(m => m.Set<Child>(), Times.Never);
			mockDbContext.Verify(m => m.Set<Master>(), Times.Never);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void DbDataLoader_Load_ThrowsExceptionForNontrackedObjects()
		{
			// Arrange
			DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
			IDbDataLoader dataLoader = new DbDataLoader(dbContext);
	
			// Act
			dataLoader.For(new Child() /* nontracked object */).Load(item => item.Parent);

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

				Child child = new Child();
				child.Parent = master;
				if (deleted)
				{
					child.Deleted = DateTime.Now;
				}

				dbContext.Master.Add(master);
				dbContext.Child.Add(child);
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
