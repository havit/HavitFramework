using System;
using System.Collections.Generic;
using System.Text;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;

namespace Havit.Data.EntityFrameworkCore.Patterns.Windsor.Installers.Caching
{
	/// <summary>
	/// Installer, která konfiguraci služeb pro cachování. Cachovat se budu všechny objekty s definovanou sliding expirací.
	/// </summary>
	public class CacheAllEntitiesWithDefaultSlidingExpirationCachingInstaller : DefaultCachingInstaller
	{
		private readonly TimeSpan slidingExpiration;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public CacheAllEntitiesWithDefaultSlidingExpirationCachingInstaller(TimeSpan slidingExpiration)
		{
			this.slidingExpiration = slidingExpiration;
		}

		/// <inheritdoc />
		protected override void RegisterEntityCacheOptionsGenerator(IWindsorContainer container)
		{
			// no base call!
			container.Register(Component.For<IEntityCacheOptionsGenerator>().ImplementedBy<AnnotationsWithDefaultsEntityCacheOptionsGenerator>()
				.LifestyleSingleton()
				.DependsOn(Dependency.OnValue("slidingExpiration", slidingExpiration))
				.DependsOn(Dependency.OnValue("absoluteExpiration", TimeSpan.Zero)));
		}

		/// <inheritdoc />
		protected override void RegisterEntityCacheSupportDecision(IWindsorContainer container)
		{
			// no base call!
			container.Register(Component.For<IEntityCacheSupportDecision>().ImplementedBy<CacheAllEntitiesEntityCacheSupportDecision>().LifestyleSingleton());
		}
	}
}
