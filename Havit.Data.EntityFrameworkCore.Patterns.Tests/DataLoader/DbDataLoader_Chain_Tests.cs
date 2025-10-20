using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.PropertyLambdaExpressions.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader.Model;
using Havit.Data.Patterns.DataLoaders;
using Havit.Data.Patterns.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader;

[TestClass]
public class DbDataLoader_Chain_Tests : DbDataLoaderTestsBase
{
	[TestMethod]
	public void DbDataLoader_Load_LoadsChains()
	{
		// Arrange
		SeedOneToManyTestData();

		DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();

		IDbEntityKeyAccessorStorage dbEntityKeyAccessorStorage = new DbEntityKeyAccessorStorageBuilder(dbContext).Build();
		IEntityKeyAccessor entityKeyAccessor = new DbEntityKeyAccessor(dbEntityKeyAccessorStorage);

		DbDataLoader dbDataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolverIncludingDeletedFilteringCollectionsSubstitution(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager(), entityKeyAccessor, new DbLoadedPropertyReaderWithMemory(dbContext), Mock.Of<ILogger<DbDataLoader>>(MockBehavior.Loose /* umožníme použití bez setupu */));

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

		IDbEntityKeyAccessorStorage dbEntityKeyAccessorStorage = new DbEntityKeyAccessorStorageBuilder(dbContext).Build();
		IEntityKeyAccessor entityKeyAccessor = new DbEntityKeyAccessor(dbEntityKeyAccessorStorage);

		DbDataLoader dbDataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolverIncludingDeletedFilteringCollectionsSubstitution(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager(), entityKeyAccessor, new DbLoadedPropertyReaderWithMemory(dbContext), Mock.Of<ILogger<DbDataLoader>>(MockBehavior.Loose /* umožníme použití bez setupu */));

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

		Assert.IsNull(loginAccount.Memberships, "Pro ověření DbDataLoaderu se předpokládá, že hodnota loginAccount.Roles je null.");

		IDbEntityKeyAccessorStorage dbEntityKeyAccessorStorage = new DbEntityKeyAccessorStorageBuilder(dbContext).Build();
		IEntityKeyAccessor entityKeyAccessor = new DbEntityKeyAccessor(dbEntityKeyAccessorStorage);

		// Act
		IDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolverIncludingDeletedFilteringCollectionsSubstitution(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager(), entityKeyAccessor, new DbLoadedPropertyReaderWithMemory(dbContext), Mock.Of<ILogger<DbDataLoader>>(MockBehavior.Loose /* umožníme použití bez setupu */));
		dataLoader.Load(loginAccount, item => item.Memberships).ThenLoad(membership => membership.Role);

		// Assert
		Assert.IsNotNull(loginAccount.Memberships, "DbDataLoader nenačetl hodnotu pro loginAccount.Roles.");
		Assert.HasCount(1, loginAccount.Memberships, "DbDataLoader nenačetl objekty do loginAccount.Roles.");
		Assert.IsNotNull(loginAccount.Memberships[0].Role, "DbDataLoader nenačetl hodnotu pro loginAccount.Roles.Role.");
	}

	[TestMethod]
	public async Task DbDataLoader_LoadAsyncAndThenLoadAsync_ManyToMany_LoadChains()
	{
		// Arrange
		SeedManyToManyTestData(false);

		DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();

		LoginAccount loginAccount = dbContext.LoginAccount.First();

		Assert.IsNull(loginAccount.Memberships, "Pro ověření DbDataLoaderu se předpokládá, že hodnota loginAccount.Roles je null.");

		IDbEntityKeyAccessorStorage dbEntityKeyAccessorStorage = new DbEntityKeyAccessorStorageBuilder(dbContext).Build();
		IEntityKeyAccessor entityKeyAccessor = new DbEntityKeyAccessor(dbEntityKeyAccessorStorage);

		// Act
		DbDataLoader dataLoader = new DbDataLoader(dbContext, new PropertyLoadSequenceResolverIncludingDeletedFilteringCollectionsSubstitution(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager(), entityKeyAccessor, new DbLoadedPropertyReaderWithMemory(dbContext), Mock.Of<ILogger<DbDataLoader>>(MockBehavior.Loose /* umožníme použití bez setupu */));
		await dataLoader.LoadAsync(loginAccount, item => item.Memberships).ThenLoadAsync(membership => membership.Role);

		// Assert
		Assert.IsNotNull(loginAccount.Memberships, "DbDataLoader nenačetl hodnotu pro loginAccount.Roles.");
		Assert.HasCount(1, loginAccount.Memberships, "DbDataLoader nenačetl objekty do loginAccount.Roles.");
		Assert.IsNotNull(loginAccount.Memberships[0].Role, "DbDataLoader nenačetl hodnotu pro loginAccount.Roles.Role.");
	}
}
