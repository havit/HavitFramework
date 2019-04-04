using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.Windsor.Installers.Caching
{
	/// <summary>
	/// Installer, která výchozí konfiguraci služeb pro cachování (EntityCacheManager a závislosti - viz metody).
	/// </summary>
	public class DefaultCachingInstaller : IWindsorInstaller
	{
		/// <inheritdoc />
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(Component.For<IEntityCacheManager>().ImplementedBy<EntityCacheManager>().LifestyleTransient()); // kvůli https://github.com/volosoft/castle-windsor-ms-adapter/issues/32 nemůžeme být singleton, protože potřebujeme AKTUÁLNÍ DbContext, ale přes factory dostáváme vždy nový
			RegisterEntityCacheOptionsGenerator(container);
			RegisterEntityCacheKeyGenerator(container);
			RegisterEntityCacheSupportDecision(container);
		}

		/// <summary>
		/// Zaregistruje službu, která vrací CacheOptions, které budou použity pro cachování entit.		
		/// </summary>
		protected virtual void RegisterEntityCacheOptionsGenerator(IWindsorContainer container)
		{
			container.Register(Component.For<IEntityCacheOptionsGenerator>().ImplementedBy<AnnotationsEntityCacheOptionsGenerator>().LifestyleSingleton());
		}

		/// <summary>
		/// Zaregistruje službu, která vrací klíče, pod kterými budou položky v cache uloženy.
		/// </summary>
		protected virtual void RegisterEntityCacheKeyGenerator(IWindsorContainer container)
		{
			container.Register(Component.For<IEntityCacheKeyGenerator>().ImplementedBy<EntityCacheKeyGenerator>().LifestyleSingleton());
		}

		/// <summary>
		/// Zaregistruje službu, která rozhoduje, zda bude daná entita cachována.
		/// </summary>
		protected virtual void RegisterEntityCacheSupportDecision(IWindsorContainer container)
		{
			container.Register(Component.For<IEntityCacheSupportDecision>().ImplementedBy<AnnotationsEntityCacheSupportDecision>().LifestyleSingleton());
		}
	}
}
