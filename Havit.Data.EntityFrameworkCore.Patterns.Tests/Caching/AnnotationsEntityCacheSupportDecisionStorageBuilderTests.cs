using Havit.Data.EntityFrameworkCore.Metadata.Conventions;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure.Model.ManyToManyAsTwoOneToMany;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching;

[TestClass]
public class AnnotationsEntityCacheSupportDecisionStorageBuilderTests
{
	[TestMethod]
	public void AnnotationsEntityCacheSupportDecisionStorageBuilder_UsesCorrectAnnotationsForEntitiesAndAllKeys()
	{
		// Arrange
		using var dbContext = new AnnotatedCachingTestDbContext();

		// Act
		IAnnotationsEntityCacheSupportDecisionStorage storage = new AnnotationsEntityCacheSupportDecisionStorageBuilder(dbContext).Build();

		// Assert
		Assert.IsTrue(storage.ShouldCacheEntities[typeof(LoginAccount)]);
		Assert.IsFalse(storage.ShouldCacheAllKeys[typeof(LoginAccount)]);
	}

	private sealed class AnnotatedCachingTestDbContext : Infrastructure.CachingTestDbContext
	{
		public AnnotatedCachingTestDbContext() : base(nameof(AnnotatedCachingTestDbContext))
		{
		}

		protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
		{
			base.CustomizeModelCreating(modelBuilder);

			modelBuilder.Entity<LoginAccount>()
				.HasAnnotation(CacheAttributeToAnnotationConvention.CacheEntitiesAnnotationName, true)
				.HasAnnotation(CacheAttributeToAnnotationConvention.CacheAllKeysAnnotationName, false);
		}
	}
}
