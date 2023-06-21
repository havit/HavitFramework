using Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure.Model;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure.Model.OneToOne;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Internal;

[TestClass]
public class ReferencingCollectionsServiceTests
{
	[TestMethod]
	public void ReferencingCollectionsService_GetReferencingCollections_ContainsCollections()
	{
		// Arrange
		CachingTestDbContext dbContext = new CachingTestDbContext();
		ReferencingCollectionsService referencingCollectionsService = new ReferencingCollectionsService(new ReferencingCollectionsStorage(), dbContext);

		// Act
		var referencingCollections = referencingCollectionsService.GetReferencingCollections(typeof(Membership));

		// Assert
		Assert.AreEqual(1, referencingCollections.Count);
		Assert.IsTrue(referencingCollections.Single().EntityType == typeof(LoginAccount));
		Assert.IsTrue(referencingCollections.Single().CollectionPropertyName == nameof(LoginAccount.Memberships));
	}

	[TestMethod]
	public void ReferencingCollectionsService_GetReferencingCollections_DoesNotContainReferences()
	{
		// Arrange
		CachingTestDbContext dbContext = new CachingTestDbContext();
		ReferencingCollectionsService referencingCollectionsService = new ReferencingCollectionsService(new ReferencingCollectionsStorage(), dbContext);

		// Act
		var referencingCollections = referencingCollectionsService.GetReferencingCollections(typeof(LoginAccount));

		// Assert
		Assert.AreEqual(0, referencingCollections.Count); // LoginAccount nikdo nereferencuje kolekcí.
	}

	[TestMethod]
	public void ReferencingCollectionsService_GetReferencingCollections_DoesNotContainOneToOneNavigations()
	{
		// Arrange
		CachingTestDbContext dbContext = new CachingTestDbContext();
		ReferencingCollectionsService referencingCollectionsService = new ReferencingCollectionsService(new ReferencingCollectionsStorage(), dbContext);

		// Act
		var referencingCollections = referencingCollectionsService.GetReferencingCollections(typeof(ClassB));

		// Assert
		Assert.AreEqual(0, referencingCollections.Count); // ClassB je referencováno jako OneToOne reference, nikoliv kolekce.
	}
}
