﻿using Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Internal.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Internal.Infrastructure.Model.ManyToMany;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Internal.Infrastructure.Model.ManyToManyAsTwoOneToMany;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Internal.Infrastructure.Model.OneToMany;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Internal.Infrastructure.Model.OneToOne;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.TestsInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Internal;

[TestClass]
public class ReferencingNavigationsServiceTests
{
	[TestMethod]
	public void ReferencingNavigationsService_GetReferencingCollections_ContainsOneToManyCollection()
	{
		// Arrange
		ReferencingNavigationsTestDbContext dbContext = new ReferencingNavigationsTestDbContext();
		ReferencingNavigationsService referencingNavigationsService = new ReferencingNavigationsService(new ReferencingNavigationsStorage(), dbContext);

		// Act		
		var referencingNavigations = referencingNavigationsService.GetReferencingNavigations(dbContext.Model.FindEntityType(typeof(Child)));

		// Assert
		Assert.AreEqual(1, referencingNavigations.Count);
		Assert.IsTrue(referencingNavigations.Single().EntityType == typeof(Master));
		Assert.IsTrue(referencingNavigations.Single().NavigationPropertyName == nameof(Master.Children));
	}

	[TestMethod]
	public void ReferencingNavigationsService_GetReferencingCollections_ContainsManyToManyCollection()
	{
		// Arrange
		ReferencingNavigationsTestDbContext dbContext = new ReferencingNavigationsTestDbContext();
		ReferencingNavigationsService referencingNavigationsService = new ReferencingNavigationsService(new ReferencingNavigationsStorage(), dbContext);

		// Act
		var referencingNavigations = referencingNavigationsService.GetReferencingNavigations(dbContext.Model.FindEntityType("ClassManyToManyA_Items"));

		// Assert
		Assert.AreEqual(1, referencingNavigations.Count);
		Assert.IsTrue(referencingNavigations.Single().EntityType == typeof(ClassManyToManyA));
		Assert.IsTrue(referencingNavigations.Single().NavigationPropertyName == nameof(ClassManyToManyA.Items));
	}

	[TestMethod]
	public void ReferencingNavigationsService_GetReferencingCollections_ContainOneToOneNavigations()
	{
		// Arrange
		ReferencingNavigationsTestDbContext dbContext = new ReferencingNavigationsTestDbContext();
		ReferencingNavigationsService referencingNavigationsService = new ReferencingNavigationsService(new ReferencingNavigationsStorage(), dbContext);

		// Act
		var referencingNavigations = referencingNavigationsService.GetReferencingNavigations(dbContext.Model.FindEntityType(typeof(ClassOneToOneB)));

		// Assert
		Assert.AreEqual(1, referencingNavigations.Count);
		Assert.IsTrue(referencingNavigations.Single().EntityType == typeof(ClassOneToOneA));
		Assert.IsTrue(referencingNavigations.Single().NavigationPropertyName == nameof(ClassOneToOneA.ClassB));
	}

	[TestMethod]
	public void ReferencingNavigationsService_GetReferencingCollections_DoesNotContainReferences()
	{
		// Arrange
		ReferencingNavigationsTestDbContext dbContext = new ReferencingNavigationsTestDbContext();
		ReferencingNavigationsService referencingNavigationsService = new ReferencingNavigationsService(new ReferencingNavigationsStorage(), dbContext);

		// Act
		var referencingNavigations = referencingNavigationsService.GetReferencingNavigations(dbContext.Model.FindEntityType(typeof(LoginAccount)));

		// Assert
		Assert.AreEqual(0, referencingNavigations.Count); // LoginAccount nikdo nereferencuje kolekcí ani one-to-one referencí.
	}
}
