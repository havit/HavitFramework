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

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Internal
{
	[TestClass]
	public class ReferencingCollectionsStoreTests
	{
		[TestMethod]
		public void ReferencingCollectionsStore_GetReferencingCollections_ContainsCollections()
		{
			// Arrange
			CachingTestDbContext dbContext = new CachingTestDbContext();
			ReferencingCollectionsStore referencingCollectionsStore = new ReferencingCollectionsStore(dbContext.CreateDbContextFactory());

			// Act
			var referencingCollections = referencingCollectionsStore.GetReferencingCollections(typeof(Membership));

			// Assert
			Assert.AreEqual(1, referencingCollections.Count);
			Assert.IsTrue(referencingCollections.Single().EntityType == typeof(LoginAccount));
			Assert.IsTrue(referencingCollections.Single().CollectionPropertyName == nameof(LoginAccount.Memberships));
		}

		[TestMethod]
		public void ReferencingCollectionsStore_GetReferencingCollections_DoesNotContainReferences()
		{
			// Arrange
			CachingTestDbContext dbContext = new CachingTestDbContext();
			ReferencingCollectionsStore referencingCollectionsStore = new ReferencingCollectionsStore(dbContext.CreateDbContextFactory());

			// Act
			var referencingCollections = referencingCollectionsStore.GetReferencingCollections(typeof(LoginAccount));

			// Assert
			Assert.AreEqual(0, referencingCollections.Count); // LoginAccount nikdo nereferencuje kolekcí.
		}

		[TestMethod]
		public void ReferencingCollectionsStore_GetReferencingCollections_DoesNotContainOneToOneNavigations()
		{
			// Arrange
			CachingTestDbContext dbContext = new CachingTestDbContext();
			ReferencingCollectionsStore referencingCollectionsStore = new ReferencingCollectionsStore(dbContext.CreateDbContextFactory());

			// Act
			var referencingCollections = referencingCollectionsStore.GetReferencingCollections(typeof(ClassB));

			// Assert
			Assert.AreEqual(0, referencingCollections.Count); // ClassB je referencováno jako OneToOne reference, nikoliv kolekce.
		}
	}
}
