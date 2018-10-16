using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;
using Havit.Data.Patterns.Infrastructure;
using Havit.Diagnostics.Contracts;
using Havit.Services;
using Havit.Services.Caching;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching
{
	/// <summary>
	/// Výchozí EntityCacheManager zajišťující cachování entit s pomocí dalších závislostí:
	/// <list type="bullet">
	/// <item>IEntityCacheSupportDecision rozhoduje, zda je možné danou entitu cachovat.</item>
	/// <item>ICacheService zajišťuje uložení do cache.</item>
	/// <item>IEntityCacheKeyGenerator zajišťuje generování klíče, pod jakým jsou entity registrovány do cache.</item>
	/// <item>IEntityCacheOptionsGenerator zajišťuje generování cache options, se kterými jsou entity registrovány do cache (umožňuje řešit prioritu nebo sliding expiraci, atp.)</item>
	/// <item>IEntityCacheDependencyManager zajišťuje získání klíčů pro invalidaci závislých záznamů.</item>
	/// </list>
	/// </summary>
	public class EntityCacheManager : IEntityCacheManager
	{
		private readonly ICacheService cacheService;
		private readonly IEntityCacheSupportDecision entityCacheSupportDecision;
		private readonly IEntityCacheKeyGenerator entityCacheKeyNamingService;
		private readonly IEntityCacheOptionsGenerator entityCacheOptionsGenerator;
		private readonly IEntityCacheDependencyManager entityCacheDependencyManager;
		private readonly IServiceFactory<IDbContext> dbContextFactory;
		private readonly IEntityKeyAccessor entityKeyAccessor;

		public EntityCacheManager(ICacheService cacheService, IEntityCacheSupportDecision entityCacheSupportDecision, IEntityCacheKeyGenerator entityCacheKeyNamingService, IEntityCacheOptionsGenerator entityCacheOptionsGenerator, IEntityCacheDependencyManager entityCacheDependencyManager, IEntityKeyAccessor entityKeyAccessor, IServiceFactory<IDbContext> dbContextFactory)
		{
			this.cacheService = cacheService;
			this.entityCacheSupportDecision = entityCacheSupportDecision;
			this.entityCacheKeyNamingService = entityCacheKeyNamingService;
			this.entityCacheOptionsGenerator = entityCacheOptionsGenerator;
			this.entityCacheDependencyManager = entityCacheDependencyManager;
			this.entityKeyAccessor = entityKeyAccessor;
			this.dbContextFactory = dbContextFactory;
		}
		/// <inheritdoc />
		public bool TryGetEntity<TEntity>(object key, out TEntity entity)
			where TEntity : class
		{
			// pokud vůbec kdy může být entita cachována, budeme se ptát do cache
			if (entityCacheSupportDecision.ShouldCacheEntity<TEntity>())
			{
				string cacheKey = entityCacheKeyNamingService.GetEntityCacheKey(typeof(TEntity), key);
				if (cacheService.TryGet(cacheKey, out object cacheValues))
				{
					// pokud je entita v cache, materializujeme ji a vrátíme ji
					TEntity result = Activator.CreateInstance<TEntity>();
					dbContextFactory.ExecuteAction(dbContext =>
					{
						var entry = dbContext.GetEntry(result, suppressDetectChanges: true);
						entry.OriginalValues.SetValues(cacheValues); // aby při případném update byly známy změněné vlastnosti
						entry.CurrentValues.SetValues(cacheValues); // aby byly naplněny vlastnosti entity
						dbContext.Set<TEntity>().AttachRange(new TEntity[] { result }); // nutno volat až po materializaci, jinak registruje entitu s nenastavenou hodnotou primárního klíče
					});
					entity = result;
					return true;

				}
			}
			entity = null;
			return false;
		}

		/// <inheritdoc />
		public void StoreEntity<TEntity>(TEntity entity)
			where TEntity : class
		{
			// pokud je entitu možné uložit do cache, uložíme ji
			if (entityCacheSupportDecision.ShouldCacheEntity(entity))
			{
				string cacheKey = entityCacheKeyNamingService.GetEntityCacheKey(typeof(TEntity), entityKeyAccessor.GetEntityKey(entity));
				EntityEntry entry = null;
				dbContextFactory.ExecuteAction(dbContext =>
				{
					entry = dbContext.GetEntry(entity, suppressDetectChanges: true);
				});
				Contract.Assert<InvalidOperationException>(entry.State != Microsoft.EntityFrameworkCore.EntityState.Detached, "Entity must be attached do DbContext."); // abychom mohli získat smysluplné entry.OriginalValues, musí být entita trackovaná (podmínka nutná, nikoliv postačující - neříká, zda má OriginalValues dobře nastaveny).

				// entry.OriginalValues vrací abstraktní PropertyValues, ten nese spoustu vlastostí vč. DbContextu.
				// Držením těchto instancí v cache bychom zabránili GC vyčistit je z paměti.
				// Cachovat proto budeme nový objekt, který reprezentuje originální hodnoty.
				// Ten získáme tak, že zavoláme entry.OriginalValues.ToObject(), což vrátí novou instanci entity ve stavu Detached.
				// Tato instance by měla držen ten základní vlastnosti, bez navigačních vlastností (což vzhledem k Detached dává význam).
				object originalValuesObject = entry.OriginalValues.ToObject(); 
				cacheService.Add(cacheKey, originalValuesObject, entityCacheOptionsGenerator.GetEntityCacheOptions(entity));
			}
		}

		/// <inheritdoc />
		public bool TryGetAllKeys<TEntity>(out object keys)
			where TEntity : class
		{
			// pokud mohou být klíče v cache, budeme je hledat
			if (entityCacheSupportDecision.ShouldCacheAllKeys<TEntity>())
			{
				string cacheKey = entityCacheKeyNamingService.GetAllKeysCacheKey(typeof(TEntity));
				return cacheService.TryGet(cacheKey, out keys);
			}

			keys = null;
			return false;
		}

		/// <inheritdoc />
		public void StoreAllKeys<TEntity>(object keys)
			where TEntity : class
		{
			// pokud je možné klíče uložit do cache, uložíme je
			if (entityCacheSupportDecision.ShouldCacheAllKeys<TEntity>())
			{
				string cacheKey = entityCacheKeyNamingService.GetAllKeysCacheKey(typeof(TEntity));
				cacheService.Add(cacheKey, (object)keys, entityCacheOptionsGenerator.GetAllKeysCacheOptions<TEntity>());
			}
		}
			   
		/// <inheritdoc />
		public void InvalidateEntity(ChangeType changeType, object entity)
		{
			Contract.Requires(entity != null);

			// invalidate entity cache
			Type entityType = entity.GetType();
			object entityKey = entityKeyAccessor.GetEntityKey(entity);

			if (changeType != ChangeType.Insert)
			{
				// nové záznamy nemohou být v cache, neinvalidujeme
				InvalidateEntityInternal(entityType, entityKey);
			}
			InvalidateGetAllInternal(entityType);

			// až budeme řešit dataloader...
			//InvalidateOneToMany();
			//InvalidateManyToMany();

			if (changeType != ChangeType.Insert)
			{
				// na nových záznamech nemohou být závislosti, neinvalidujeme
				InvalidateSaveCacheDependencyKeyInternal(entityType, entityKey);
			}
			InvalidateAnySaveCacheDependencyKeyInternal(entityType);		
		}

		private void InvalidateEntityInternal(Type entityType, object entityKey)
		{
			cacheService.Remove(entityCacheKeyNamingService.GetEntityCacheKey(entityType, entityKey));
		}

		private void InvalidateGetAllInternal(Type type)
		{
			cacheService.Remove(entityCacheKeyNamingService.GetAllKeysCacheKey(type));
		}

		private void InvalidateSaveCacheDependencyKeyInternal(Type entityType, object entityKey)
		{
			if (cacheService.SupportsCacheDependencies)
			{
				cacheService.Remove(entityCacheDependencyManager.GetSaveCacheDependencyKey(entityType, entityKey));
			}
		}

		private void InvalidateAnySaveCacheDependencyKeyInternal(Type entityType)
		{
			if (cacheService.SupportsCacheDependencies)
			{
				cacheService.Remove(entityCacheDependencyManager.GetAnySaveCacheDependencyKey(entityType, false));
			}
		}
	}
}