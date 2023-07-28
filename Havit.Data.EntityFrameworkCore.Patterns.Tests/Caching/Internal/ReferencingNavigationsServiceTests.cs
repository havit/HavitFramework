using Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure.Model.ManyToMany;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure.Model.ManyToManyAsTwoOneToMany;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure.Model.OneToMany;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure.Model.OneToOne;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Internal;

[TestClass]
public class ReferencingNavigationsServiceTests
{
	[TestMethod]
	public void ReferencingNavigationsService_GetReferencingNavigations_ContainsOneToManyCollection()
	{
		// Arrange
		CachingTestDbContext dbContext = new CachingTestDbContext();
		ReferencingNavigationsService referencingNavigationsService = new ReferencingNavigationsService(new ReferencingNavigationsStorage(), dbContext);

		// Act		
		var childReferencingNavigations = referencingNavigationsService.GetReferencingNavigations(dbContext.Model.FindEntityType(typeof(Child)));
		var materReferencingNavigations = referencingNavigationsService.GetReferencingNavigations(dbContext.Model.FindEntityType(typeof(Master)));

		// Assert
		Assert.AreEqual(1, childReferencingNavigations.Count);
		Assert.AreSame(typeof(Master), childReferencingNavigations.Single().EntityType);
		Assert.AreEqual(nameof(Master.Children), childReferencingNavigations.Single().NavigationPropertyName);

		Assert.AreEqual(0, materReferencingNavigations.Count);
	}

	[TestMethod]
	public void ReferencingNavigationsService_GetReferencingNavigations_ContainsManyToManyCollection()
	{
		// Arrange
		CachingTestDbContext dbContext = new CachingTestDbContext();
		ReferencingNavigationsService referencingNavigationsService = new ReferencingNavigationsService(new ReferencingNavigationsStorage(), dbContext);

		// Act
		var skipNavigationEntityReferencingNavigations = referencingNavigationsService.GetReferencingNavigations(dbContext.Model.FindEntityType("ClassManyToManyA_Items"));
		var classManyToManyAReferencingNavigations = referencingNavigationsService.GetReferencingNavigations(dbContext.Model.FindEntityType(typeof(ClassManyToManyA)));

		// Assert
		Assert.AreEqual(1, skipNavigationEntityReferencingNavigations.Count);
		Assert.AreSame(typeof(ClassManyToManyA), skipNavigationEntityReferencingNavigations.Single().EntityType);
		Assert.AreEqual(nameof(ClassManyToManyA.Items), skipNavigationEntityReferencingNavigations.Single().NavigationPropertyName);

		Assert.AreEqual(0, classManyToManyAReferencingNavigations.Count);
	}

	[TestMethod]
	public void ReferencingNavigationsService_GetReferencingNavigations_ContainOneToOneNavigations()
	{
		// Arrange
		CachingTestDbContext dbContext = new CachingTestDbContext();
		ReferencingNavigationsService referencingNavigationsService = new ReferencingNavigationsService(new ReferencingNavigationsStorage(), dbContext);

		// Act
		var classOneToOneAReferencingNavigations = referencingNavigationsService.GetReferencingNavigations(dbContext.Model.FindEntityType(typeof(ClassOneToOneA)));
		var classOneToOneBReferencingNavigations = referencingNavigationsService.GetReferencingNavigations(dbContext.Model.FindEntityType(typeof(ClassOneToOneB)));

		// Assert
		Assert.AreEqual(1, classOneToOneBReferencingNavigations.Count);
		Assert.AreSame(typeof(ClassOneToOneA), classOneToOneBReferencingNavigations.Single().EntityType);
		Assert.AreEqual(nameof(ClassOneToOneA.ClassB), classOneToOneBReferencingNavigations.Single().NavigationPropertyName);

		Assert.AreEqual(0, classOneToOneAReferencingNavigations.Count);
	}

	[TestMethod]
	public void ReferencingNavigationsService_GetReferencingNavigations_DoesNotContainReferences()
	{
		// Arrange
		CachingTestDbContext dbContext = new CachingTestDbContext();
		ReferencingNavigationsService referencingNavigationsService = new ReferencingNavigationsService(new ReferencingNavigationsStorage(), dbContext);

		// Act
		var referencingNavigations = referencingNavigationsService.GetReferencingNavigations(dbContext.Model.FindEntityType(typeof(LoginAccount)));

		// Assert
		Assert.AreEqual(0, referencingNavigations.Count); // LoginAccount nikdo nereferencuje kolekcí ani one-to-one referencí.
	}
}
