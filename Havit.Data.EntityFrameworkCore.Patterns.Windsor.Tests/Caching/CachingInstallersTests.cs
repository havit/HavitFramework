using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors;
using Havit.Data.EntityFrameworkCore.Patterns.Windsor.Installers;
using Havit.Data.EntityFrameworkCore.Patterns.Windsor.Tests.Infrastructure.DataLayer;
using Havit.Data.EntityFrameworkCore.Patterns.Windsor.Tests.Infrastructure.Entity;
using Havit.Data.EntityFrameworkCore.Patterns.Windsor.Tests.Infrastructure.Model;
using Havit.Data.Patterns.DataLoaders;
using Havit.Data.Patterns.Localizations;
using Havit.Data.Patterns.UnitOfWorks;
using Havit.Services;
using Havit.Services.TimeServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Havit.Data.EntityFrameworkCore.Patterns.Windsor.Tests.Caching
{
	[TestClass]
	public class CachingInstallersTests
	{
		[TestMethod]
		public void CachingInstallersTests_NoCachingEntityCacheManager()
		{
			// Arrange
			var container = Helpers.CreateAndSetupWindsorContainer(new ComponentRegistrationOptions { GeneralLifestyle = lf => lf.Singleton }.ConfigureNoCaching());

			// Act
			IEntityCacheManager entityCacheManager = container.Resolve<IEntityCacheManager>();

			// Assert			
			Assert.IsInstanceOfType(entityCacheManager, typeof(NoCachingEntityCacheManager));
		}

		[TestMethod]
		public void CachingInstallersTests_SlidingCachingEntityCacheManager()
		{
			// Arrange
			var container = Helpers.CreateAndSetupWindsorContainer(new ComponentRegistrationOptions { GeneralLifestyle = lf => lf.Singleton }.ConfigureCacheAllEntitiesWithDefaultSlidingExpirationCaching(TimeSpan.FromMinutes(5)));

			// Act
			IEntityCacheManager entityCacheManager = container.Resolve<IEntityCacheManager>();
			IEntityCacheSupportDecision entityCacheSupportDecision = container.Resolve<IEntityCacheSupportDecision>();
			IEntityCacheOptionsGenerator entityCacheOptionsGenerator = container.Resolve<IEntityCacheOptionsGenerator>();

			// Assert			
			Assert.IsInstanceOfType(entityCacheManager, typeof(EntityCacheManager));
			Assert.IsInstanceOfType(entityCacheSupportDecision, typeof(CacheAllEntitiesEntityCacheSupportDecision));
			Assert.IsInstanceOfType(entityCacheOptionsGenerator, typeof(AnnotationsWithDefaultsEntityCacheOptionsGenerator));
		}

		[TestMethod]
		public void CachingInstallersTests_DefaultConfiguration()
		{
			// Arrange
			var container = Helpers.CreateAndSetupWindsorContainer();

			// Act
			using (container.BeginScope())
			{
				container.Resolve<IEntityCacheManager>();
			}

			// Assert			
			// no exception was thrown
		}
	}
}
