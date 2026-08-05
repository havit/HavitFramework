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

/// <summary>
/// Testy načítání vlastností entit, které jsou typovány interface (typem, který není entitou modelu).
/// </summary>
[TestClass]
public class DbDataLoader_InterfaceTypedEntities_Tests : DbDataLoaderTestsBase
{
	public TestContext TestContext { get; set; }

	[TestMethod]
	public void DbDataLoader_Load_Reference_SupportsInterfaceTypedEntity()
	{
		// Arrange
		SeedOneToManyTestData();

		DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();

		IChildWithParent child = dbContext.Child.First();

		Assert.IsNull(child.Parent, "Pro ověření DbDataLoaderu se předpokládá, že hodnota child.Parent je null.");

		IDataLoader dataLoader = CreateDataLoader(dbContext);

		// Act
		dataLoader.Load(child, item => item.Parent);

		// Assert
		Assert.IsNotNull(child.Parent, "DbDataLoader nenačetl hodnotu pro child.Parent.");
		Assert.IsTrue(dbContext.GetEntry((Child)child, suppressDetectChanges: false).Reference(nameof(Model.Child.Parent)).IsLoaded, "DbContext nepovažuje vlastnost za načtenou.");
	}

	[TestMethod]
	public void DbDataLoader_Load_Reference_SupportsGenericTypeParameterConstrainedByInterface()
	{
		// Arrange
		SeedOneToManyTestData();

		DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();

		Child child = dbContext.Child.First();

		Assert.IsNull(child.Parent, "Pro ověření DbDataLoaderu se předpokládá, že hodnota child.Parent je null.");

		IDataLoader dataLoader = CreateDataLoader(dbContext);

		// Vlastnost Parent se v expression tree naváže na interface IChildWithParent, přestože TEntity je typu Child (není interface),
		// takže nedojde k substituci typu parametru jako u entit typovaných interfacem.
		void LoadParent<TEntity>(TEntity entity)
			where TEntity : class, IChildWithParent
		{
			dataLoader.Load(entity, item => item.Parent);
		}

		// Act
		LoadParent(child);

		// Assert
		Assert.IsNotNull(child.Parent, "DbDataLoader nenačetl hodnotu pro child.Parent.");
		Assert.IsTrue(dbContext.GetEntry(child, suppressDetectChanges: false).Reference(nameof(Model.Child.Parent)).IsLoaded, "DbContext nepovažuje vlastnost za načtenou.");
	}

	[TestMethod]
	public async Task DbDataLoader_LoadAsync_Reference_SupportsInterfaceTypedEntity()
	{
		// Arrange
		SeedOneToManyTestData();

		DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();

		IChildWithParent child = dbContext.Child.First();

		Assert.IsNull(child.Parent, "Pro ověření DbDataLoaderu se předpokládá, že hodnota child.Parent je null.");

		IDataLoader dataLoader = CreateDataLoader(dbContext);

		// Act
		await dataLoader.LoadAsync(child, item => item.Parent, TestContext.CancellationToken);

		// Assert
		Assert.IsNotNull(child.Parent, "DbDataLoader nenačetl hodnotu pro child.Parent.");
	}

	[TestMethod]
	public void DbDataLoader_LoadAll_Reference_SupportsInterfaceTypedEntities()
	{
		// Arrange
		SeedOneToManyTestData();

		DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();

		List<IChildWithParent> children = dbContext.Child.ToList().Cast<IChildWithParent>().ToList();

		Assert.IsGreaterThan(1, children.Count, "Pro ověření DbDataLoaderu se předpokládá více entit.");
		Assert.IsTrue(children.All(item => item.Parent == null), "Pro ověření DbDataLoaderu se předpokládá, že hodnoty Parent jsou null.");

		IDataLoader dataLoader = CreateDataLoader(dbContext);

		// Act
		dataLoader.LoadAll(children, item => item.Parent);

		// Assert
		Assert.IsTrue(children.All(item => item.Parent != null), "DbDataLoader nenačetl hodnoty pro Parent.");
	}

	[TestMethod]
	public void DbDataLoader_Load_Collection_SupportsInterfaceTypedEntity()
	{
		// Arrange
		SeedOneToManyTestData();

		DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();

		IMasterWithChildren master = dbContext.Master.First();

		Assert.IsFalse(master.Children.Any(), "Pro ověření DbDataLoaderu se předpokládá, že master.Children je prázdná.");

		IDataLoader dataLoader = CreateDataLoader(dbContext);

		// Act
		dataLoader.Load(master, item => item.Children);

		// Assert
		Assert.IsNotNull(master.Children, "DbDataLoader nenačetl hodnotu pro master.Children.");
		Assert.AreEqual(5, master.Children.Count, "DbDataLoader nenačetl objekty do master.Children.");
	}

	[TestMethod]
	public async Task DbDataLoader_LoadAsync_Collection_SupportsInterfaceTypedEntity()
	{
		// Arrange
		SeedOneToManyTestData();

		DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();

		IMasterWithChildren master = dbContext.Master.First();

		Assert.IsFalse(master.Children.Any(), "Pro ověření DbDataLoaderu se předpokládá, že master.Children je prázdná.");

		IDataLoader dataLoader = CreateDataLoader(dbContext);

		// Act
		await dataLoader.LoadAsync(master, item => item.Children, TestContext.CancellationToken);

		// Assert
		Assert.AreEqual(5, master.Children.Count, "DbDataLoader nenačetl objekty do master.Children.");
	}

	[TestMethod]
	public void DbDataLoader_Load_Collection_SubstitutesIncludingDeletedWhenCalledViaInterface()
	{
		// Arrange
		SeedOneToManyTestData(deleted: true);

		DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();

		IMasterWithChildren master = dbContext.Master.First();

		IDataLoader dataLoader = CreateDataLoader(dbContext);

		// Act
		dataLoader.Load(master, item => item.Children);

		// Assert
		// Substituce Children -> ChildrenIncludingDeleted musí proběhnout i při volání přes interface,
		// tj. načteny jsou i smazané objekty (filtrující kolekce Children je pak nezobrazuje).
		Assert.AreEqual(0, master.Children.Count, "Filtrující kolekce Children nemá smazané objekty zobrazovat.");
		Assert.HasCount(5, ((Master)master).ChildrenIncludingDeleted, "DbDataLoader nenačetl objekty do master.ChildrenIncludingDeleted (substituce neproběhla).");
	}

	[TestMethod]
	public void DbDataLoader_Load_ChainedPath_SupportsInterfaceTypedEntity()
	{
		// Arrange
		SeedOneToManyTestData();

		DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();

		IChildWithParent child = dbContext.Child.First();

		IDataLoader dataLoader = CreateDataLoader(dbContext);

		// Act
		dataLoader.Load(child, item => item.Parent.Children);

		// Assert
		Assert.IsNotNull(child.Parent, "DbDataLoader nenačetl hodnotu pro child.Parent.");
		Assert.IsTrue(child.Parent.Children.Any(), "DbDataLoader nenačetl objekty do child.Parent.Children.");
	}

	[TestMethod]
	public void DbDataLoader_LoadAll_SupportsEmptyEnumerableOfInterfaceTypedEntities()
	{
		// Arrange
		DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();
		dbContext.Database.DropCreate();

		IDataLoader dataLoader = CreateDataLoader(dbContext);

		// Act
		var fluentDataLoader = dataLoader.LoadAll(new List<IChildWithParent>(), item => item.Parent);

		// Assert
		// žádná výjimka, není co načítat (a není z čeho určit skutečný typ entit)
		Assert.IsNotNull(fluentDataLoader);
	}

	[TestMethod]
	public void DbDataLoader_Load_ThrowsForExplicitlyImplementedInterfaceProperty()
	{
		// Arrange
		SeedOneToManyTestData();

		DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();

		IChildWithParentExplicit child = dbContext.Child.First();

		IDataLoader dataLoader = CreateDataLoader(dbContext);

		// Act + Assert
		Assert.ThrowsExactly<InvalidOperationException>(() =>
		{
			dataLoader.Load(child, item => item.ParentExplicit);
		});
	}

	[TestMethod]
	public void DbDataLoader_LoadAll_ThrowsForMixedEntityTypes()
	{
		// Arrange
		SeedOneToManyTestData();

		DataLoaderTestDbContext dbContext = new DataLoaderTestDbContext();

		Child child = dbContext.Child.First();
		AnotherChild anotherChild = new AnotherChild();
		dbContext.AnotherChild.Add(anotherChild);

		IDataLoader dataLoader = CreateDataLoader(dbContext);

		// Act + Assert
		Assert.ThrowsExactly<InvalidOperationException>(() =>
		{
			dataLoader.LoadAll(new IChildWithParent[] { child, anotherChild }, item => item.Parent);
		});
	}

	private static IDataLoader CreateDataLoader(DataLoaderTestDbContext dbContext)
	{
		IDbEntityKeyAccessorStorage dbEntityKeyAccessorStorage = new DbEntityKeyAccessorStorageBuilder(dbContext).Build();
		IEntityKeyAccessor entityKeyAccessor = new DbEntityKeyAccessor(dbEntityKeyAccessorStorage);

		return new DbDataLoader(dbContext, new PropertyLoadSequenceResolverIncludingDeletedFilteringCollectionsSubstitution(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), new NoCachingEntityCacheManager(), entityKeyAccessor, new DbLoadedPropertyReaderWithMemory(dbContext), Mock.Of<ILogger<DbDataLoader>>(MockBehavior.Loose /* umožníme použití bez setupu */));
	}
}
