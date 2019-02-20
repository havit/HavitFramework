using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader.Model;
using Havit.Data.Patterns.DataLoaders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader
{
	[TestClass]
	public class DbDataLoader_Chain_Tests : DbDataLoaderTestsBase
	{
		[TestMethod]
		public void DbDataLoader_Load_LoadsChains()
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
		public void DbDataLoader_LoadAndThenLoad_ManyToMany_LoadChains()
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
		public async Task DbDataLoader_LoadAsyncAndThenLoadAsync_ManyToMany_LoadChains()
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
	}
}
