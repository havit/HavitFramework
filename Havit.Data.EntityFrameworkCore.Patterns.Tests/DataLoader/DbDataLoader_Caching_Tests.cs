using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader.Model;
using Havit.Services.Caching;
using Havit.Services.TestHelpers.Caching;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader;

[TestClass]
public class DbDataLoader_Caching_Tests
{
	public TestContext TestContext { get; set; }

	/// <summary>
	/// Cílem je ověřit, že dojde k fixupu při použití objektu z cache.
	/// </summary>
	[TestMethod]
	public void DbDataLoader_Load_GetsObjectFromCache()
	{
		// Arrange
		DictionaryCacheService cacheService = new DictionaryCacheService();

		// uložíme objekt Role do cache
		Role role = new Role { Id = 1 };
		StoreRoleToCache(cacheService, role);

		// vytvoříme nový dbContext pro membership, do které načteme roli z cache
		DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();

		EntityCacheManager entityCacheManager = CachingTestHelper.CreateEntityCacheManager(dbContext: dbContext, cacheService: cacheService);
		DbDataLoader dataLoader = DataLoaderTestHelper.CreateDataLoader(dbContext: dbContext, entityCacheManager: entityCacheManager);

		// vytvoříme si objekt membership s odkazem na neexistující LoginAccount a Roli, tento objekt si připojíme k DbContextu jako existující (avšak není v databázi)
		Membership membership = new Membership { LoginAccountId = 1, RoleId = 1 };
		dbContext.Attach(membership);

		// Preconditions
		Assert.IsNull(membership.Role); // v tento okamžik musí být nenačtená, chceme ověřit dočítání

		// Act
		// Pokusíme se načíst objekt Role.
		// Není v databázi, takže jediná šance, jak jej odbavit je získat jej z cache.
		dataLoader.Load(membership, m => m.Role);

		// Assert
		Assert.IsNotNull(membership.Role);
		Assert.IsTrue(dbContext.Entry(membership).Reference(nameof(Membership.Role)).IsLoaded); // vlastnost je označena jako načtená
	}

	/// <summary>
	/// Cílem je ověřit, že dojde k fixupu při použití objektu z cache.
	/// </summary>
	/// <remarks>
	/// Bug 42346: Cachování padá po druhém requestu - dataLoader.Load() =The instance of entity type 'EventType' cannot be tracked because another instance with the same key value for {'Id'} is already being tracked. 
	/// </remarks>
	[TestMethod]
	public void DbDataLoader_Load_GetsObjectFromCacheForMultipleReferences()
	{
		// Arrange
		DictionaryCacheService cacheService = new DictionaryCacheService();

		// uložíme objekt Role do cache
		Role role = new Role { Id = 1 };
		StoreRoleToCache(cacheService, role);

		// vytvoříme nový dbContext pro membership, do které načteme roli z cache
		DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();

		EntityCacheManager entityCacheManager = CachingTestHelper.CreateEntityCacheManager(
			dbContext: dbContext,
			cacheService: cacheService);
		DbDataLoader dataLoader = DataLoaderTestHelper.CreateDataLoader(dbContext: dbContext, entityCacheManager: entityCacheManager);

		// vytvoříme si objekt membership s odkazem na neexistující LoginAccount a Roli, tento objekt si připojíme k DbContextu jako existující (avšak není v databázi)
		Membership membership1 = new Membership { LoginAccountId = 1, RoleId = 1 };
		Membership membership2 = new Membership { LoginAccountId = 2, RoleId = 1 };
		dbContext.Attach(membership1);
		dbContext.Attach(membership2);

		// Preconditions
		Assert.IsNull(membership1.Role); // v tento okamžik musí být nenačtená, chceme ověřit dočítání
		Assert.IsNull(membership2.Role); // v tento okamžik musí být nenačtená, chceme ověřit dočítání

		// Act
		// Pokusíme se načíst objekt Role.
		// Není v databázi, takže jediná šance, jak jej odbavit je získat jej z cache.
		dataLoader.LoadAll(new Membership[] { membership1, membership2 }, m => m.Role);

		// Assert
		Assert.IsNotNull(membership1.Role);
		Assert.IsNotNull(membership2.Role);
		Assert.IsTrue(dbContext.Entry(membership1).Reference(nameof(Membership.Role)).IsLoaded); // vlastnost je označena jako načtená
		Assert.IsTrue(dbContext.Entry(membership2).Reference(nameof(Membership.Role)).IsLoaded); // vlastnost je označena jako načtená
	}

	/// <summary>
	/// Cílem je ověřit, že dojde k fixupu při použití objektu z cache.
	/// </summary>
	[TestMethod]
	public async Task DbDataLoader_LoadAsync_GetsObjectFromCache()
	{
		// Arrange
		DictionaryCacheService cacheService = new DictionaryCacheService();

		// uložíme objekt Role do cache
		Role role = new Role { Id = 1 };
		StoreRoleToCache(cacheService, role);

		// vytvoříme nový dbContext pro membership, do které načteme roli z cache
		DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();

		EntityCacheManager entityCacheManager = CachingTestHelper.CreateEntityCacheManager(
			dbContext: dbContext,
			cacheService: cacheService);

		DbDataLoader dataLoader = DataLoaderTestHelper.CreateDataLoader(dbContext: dbContext, entityCacheManager: entityCacheManager);

		// vytvoříme si objekt membership s odkazem na neexistující LoginAccount a Roli, tento objekt si připojíme k DbContextu jako existující (avšak není v databázi)
		Membership membership = new Membership { LoginAccountId = 1, RoleId = 1 };
		dbContext.Attach(membership);

		// Preconditions
		Assert.IsNull(membership.Role); // v tento okamžik musí být nenačtená, chceme ověřit dočítání

		// Act
		// Pokusíme se načíst objekt Role.
		// Není v databázi, takže jediná šance, jak jej odbavit je získat jej z cache.
		await dataLoader.LoadAsync(membership, m => m.Role, TestContext.CancellationToken);

		// Assert
		Assert.IsNotNull(membership.Role);
		Assert.IsTrue(dbContext.Entry(membership).Reference(nameof(Membership.Role)).IsLoaded); // vlastnost je označena jako načtená
	}

	private static void StoreRoleToCache(ICacheService cacheService, Role role)
	{
		DataLoaderTestDbContext dbContextInitial = new DataLoaderTestDbContext();
		dbContextInitial.Database.DropCreate(); // smažeme obsah databáze

		// připojíme objekt Role k DbContextu jako existující (avšak není v databázi)
		dbContextInitial.Attach(role);

		IEntityCacheKeyPrefixStorage entityCacheKeyPrefixStorage = new EntityCacheKeyPrefixStorageBuilder(dbContextInitial).Build();
		EntityCacheKeyGenerator entityCacheKeyGenerator = new EntityCacheKeyGenerator(new EntityCacheKeyPrefixService(entityCacheKeyPrefixStorage));

		// a tento in memory objekt uložíme do cache (přestože není v databázi)
		EntityCacheManager entityCacheManagerInitial = CachingTestHelper.CreateEntityCacheManager(
			dbContext: dbContextInitial,
			cacheService: cacheService,
			entityCacheKeyGenerator: entityCacheKeyGenerator);

		entityCacheManagerInitial.StoreEntity(role);

		// nyní máme objekt Role v cache
		Assert.IsTrue(cacheService.Contains(entityCacheKeyGenerator.GetEntityCacheKey(role.GetType(), role.Id))); // pokud selže, nedostal se objekt do cache
	}

}
