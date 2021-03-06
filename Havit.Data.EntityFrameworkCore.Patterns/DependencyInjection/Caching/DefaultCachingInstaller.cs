﻿using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Caching
{
	/// <summary>
	/// Installer, která výchozí konfiguraci služeb pro cachování (EntityCacheManager a závislosti - viz metody).
	/// </summary>
	public class DefaultCachingInstaller<TLifetime> : ICachingInstaller<TLifetime>
	{
		/// <inheritdoc />
		public void Install(IServiceInstaller<TLifetime> installer)
		{
			installer.AddServiceTransient<IEntityCacheManager, EntityCacheManager>(); // kvůli https://github.com/volosoft/castle-windsor-ms-adapter/issues/32 nemůžeme být singleton, protože potřebujeme AKTUÁLNÍ DbContext, ale přes factory dostáváme vždy nový
			RegisterEntityCacheOptionsGenerator(installer);
			RegisterEntityCacheKeyGenerator(installer);
			RegisterEntityCacheSupportDecision(installer);
		}

		/// <summary>
		/// Zaregistruje službu, která vrací CacheOptions, které budou použity pro cachování entit.		
		/// </summary>
		protected virtual void RegisterEntityCacheOptionsGenerator(IServiceInstaller<TLifetime> installer)
		{
			installer.AddServiceTransient<IEntityCacheOptionsGenerator, AnnotationsEntityCacheOptionsGenerator>();
			installer.AddServiceSingleton<IAnnotationsEntityCacheOptionsGeneratorStorage, AnnotationsEntityCacheOptionsGeneratorStorage>();
		}

		/// <summary>
		/// Zaregistruje službu, která vrací klíče, pod kterými budou položky v cache uloženy.
		/// </summary>
		protected virtual void RegisterEntityCacheKeyGenerator(IServiceInstaller<TLifetime> installer)
		{
			installer.AddServiceTransient<IEntityCacheKeyGenerator, EntityCacheKeyGenerator>();
			installer.AddServiceSingleton<IEntityCacheKeyGeneratorStorage, EntityCacheKeyGeneratorStorage>();
		}

		/// <summary>
		/// Zaregistruje službu, která rozhoduje, zda bude daná entita cachována.
		/// </summary>
		protected virtual void RegisterEntityCacheSupportDecision(IServiceInstaller<TLifetime> installer)
		{
			installer.AddServiceTransient<IEntityCacheSupportDecision, AnnotationsEntityCacheSupportDecision>();
			installer.AddServiceSingleton<IAnnotationsEntityCacheSupportDecisionStorage, AnnotationsEntityCacheSupportDecisionStorage>();
		}
	}
}
