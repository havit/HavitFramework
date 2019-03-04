using Havit.Data.EntityFrameworkCore.Metadata;
using Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;
using Havit.Data.Patterns.Infrastructure;
using Havit.Diagnostics.Contracts;
using Havit.Services;
using Havit.Services.Caching;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
		private readonly IEntityCacheKeyGenerator entityCacheKeyGenerator;
		private readonly IEntityCacheOptionsGenerator entityCacheOptionsGenerator;
		private readonly IEntityCacheDependencyManager entityCacheDependencyManager;
		private readonly IDbContext dbContext;
        private readonly IReferencingCollectionsStore referencingCollectionsStore;
        private readonly IEntityKeyAccessor entityKeyAccessor;
		private readonly IPropertyLambdaExpressionManager propertyLambdaExpressionManager;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public EntityCacheManager(ICacheService cacheService, IEntityCacheSupportDecision entityCacheSupportDecision, IEntityCacheKeyGenerator entityCacheKeyGenerator, IEntityCacheOptionsGenerator entityCacheOptionsGenerator, IEntityCacheDependencyManager entityCacheDependencyManager, IEntityKeyAccessor entityKeyAccessor, IPropertyLambdaExpressionManager propertyLambdaExpressionManager, IDbContext dbContext, IReferencingCollectionsStore referencingCollectionsStore)
		{
			this.cacheService = cacheService;
			this.entityCacheSupportDecision = entityCacheSupportDecision;
			this.entityCacheKeyGenerator = entityCacheKeyGenerator;
			this.entityCacheOptionsGenerator = entityCacheOptionsGenerator;
			this.entityCacheDependencyManager = entityCacheDependencyManager;
			this.entityKeyAccessor = entityKeyAccessor;
			this.propertyLambdaExpressionManager = propertyLambdaExpressionManager;
			this.dbContext = dbContext;
            this.referencingCollectionsStore = referencingCollectionsStore;
        }

		/// <inheritdoc />
		public bool TryGetEntity<TEntity>(object key, out TEntity entity)
			where TEntity : class
		{
			// pokud vůbec kdy může být entita cachována, budeme se ptát do cache
			if (entityCacheSupportDecision.ShouldCacheEntity<TEntity>())
			{
				string cacheKey = entityCacheKeyGenerator.GetEntityCacheKey(typeof(TEntity), key);
				if (cacheService.TryGet(cacheKey, out object cacheValues))
				{
					// pokud je entita v cache, materializujeme ji a vrátíme ji
					TEntity result = Activator.CreateInstance<TEntity>();
					//dbContextFactory.ExecuteAction(dbContext =>
					//{
					var entry = dbContext.GetEntry(result, suppressDetectChanges: true);
					entry.OriginalValues.SetValues(cacheValues); // aby při případném update byly známy změněné vlastnosti
					entry.CurrentValues.SetValues(cacheValues); // aby byly naplněny vlastnosti entity
					dbContext.Set<TEntity>().AttachRange(new TEntity[] { result }); // nutno volat až po materializaci, jinak registruje entitu s nenastavenou hodnotou primárního klíče
					//});
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
				string cacheKey = entityCacheKeyGenerator.GetEntityCacheKey(typeof(TEntity), entityKeyAccessor.GetEntityKeyValues(entity).Single());
				EntityEntry entry = null;
				//dbContextFactory.ExecuteAction(dbContext =>
				//{
				entry = dbContext.GetEntry(entity, suppressDetectChanges: true);
				//});
				Contract.Assert<InvalidOperationException>(entry.State != Microsoft.EntityFrameworkCore.EntityState.Detached, "Entity must be attached to DbContext."); // abychom mohli získat smysluplné entry.OriginalValues, musí být entita trackovaná (podmínka nutná, nikoliv postačující - neříká, zda má OriginalValues dobře nastaveny).

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
				string cacheKey = entityCacheKeyGenerator.GetAllKeysCacheKey(typeof(TEntity));
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
				string cacheKey = entityCacheKeyGenerator.GetAllKeysCacheKey(typeof(TEntity));
				cacheService.Add(cacheKey, (object)keys, entityCacheOptionsGenerator.GetAllKeysCacheOptions<TEntity>());
			}
		}

		/// <inheritdoc />
		public bool TryGetCollection<TEntity, TPropertyItem>(TEntity entity, string propertyName)
			where TEntity : class
			where TPropertyItem : class
		{
			if (entityCacheSupportDecision.ShouldCacheCollection<TEntity, TPropertyItem>(entity, propertyName))
			{
				string cacheKey = entityCacheKeyGenerator.GetCollectionCacheKey(typeof(TEntity), entityKeyAccessor.GetEntityKeyValues(entity).Single(), propertyName);

				if (cacheService.TryGet(cacheKey, out object cacheEntityPropertyMembersKeys))
				{
					object[][] entityPropertyMembersKeys = (object[][])cacheEntityPropertyMembersKeys;

					bool isManyToManyEntity = dbContext.Model.FindEntityType(typeof(TPropertyItem)).IsManyToManyEntity();

					var dbSet = dbContext.Set<TPropertyItem>();
					if (isManyToManyEntity)
					{
						var propertyNames = entityKeyAccessor.GetEntityKeyPropertyNames(typeof(TPropertyItem));

						foreach (object[] entityPropertyMemberKey in entityPropertyMembersKeys)
						{
							if (dbSet.FindTracked(entityPropertyMemberKey) != null) // už je načtený, nemůžeme volat TryGetEntity
							{
								TPropertyItem instance = Activator.CreateInstance<TPropertyItem>();
								for (int i = 0; i < propertyNames.Length; i++)
								{
									typeof(TPropertyItem).GetProperty(propertyNames[i]).SetValue(instance, entityPropertyMemberKey[i]);
								}
								dbSet.AttachRange(new TPropertyItem[] { instance });
							}
						}
						return true;
					}
					else
					{
						return entityPropertyMembersKeys.All(entityPropertyMemberKey =>
							(dbSet.FindTracked(entityPropertyMemberKey) != null) // už je načtený, nemůžeme volat TryGetEntity
							|| TryGetEntity<TPropertyItem>(entityPropertyMemberKey, out _));
					}
				}
			}
			return false;
		}

		/// <inheritdoc />
		public void StoreCollection<TEntity, TPropertyItem>(TEntity entity, string propertyName)
			where TEntity : class
			where TPropertyItem : class
		{
			if (entityCacheSupportDecision.ShouldCacheCollection<TEntity, TPropertyItem>(entity, propertyName))
			{
				string cacheKey = entityCacheKeyGenerator.GetCollectionCacheKey(typeof(TEntity), entityKeyAccessor.GetEntityKeyValues(entity).Single(), propertyName);

				var propertyLambda = propertyLambdaExpressionManager.GetPropertyLambdaExpression<TEntity, IEnumerable<TPropertyItem>>(propertyName).LambdaCompiled;
				var entityPropertyMembers = propertyLambda(entity) ?? Enumerable.Empty<TPropertyItem>();

				object[][] entityPropertyMembersKeys = entityPropertyMembers.Select(entityPropertyMember => entityKeyAccessor.GetEntityKeyValues(entityPropertyMember)).ToArray();
				cacheService.Add(cacheKey, entityPropertyMembersKeys);
			}
		}

		/// <inheritdoc />
		public void InvalidateEntity(ChangeType changeType, object entity)
		{
			Contract.Requires(entity != null);

			// invalidate entity cache
			Type entityType = entity.GetType();
            
			object[] entityKeyValues = entityKeyAccessor.GetEntityKeyValues(entity);

            // entity se složeným klíčem (ManyToMany)
            // TODO: Ověřit si, že jde o ManyToMany, nejen o složený klíč
            if (entityKeyValues.Length > 1)
            {
                // odebereme všechny prvky, které mohou mít objekt v kolekci
                InvalidateCollectionsInternal(entityType, entity);
                
                // GetAll a GetEntity není nutné řešit, objekty reprezentující vztah ManyToMany se do cache nedostávají
            }
            else
            {
                object entityKeyValue = entityKeyValues.Single();

                if (changeType != ChangeType.Insert)
                {
                    // nové záznamy nemohou být v cache, neinvalidujeme
                    InvalidateEntityInternal(entityType, entityKeyValue);
                }

                InvalidateCollectionsInternal(entityType, entity);
                InvalidateGetAllInternal(entityType);

                if (changeType != ChangeType.Insert)
                {
                    // na nových záznamech nemohou být závislosti, neinvalidujeme
                    InvalidateSaveCacheDependencyKeyInternal(entityType, entityKeyValue);
                }
                InvalidateAnySaveCacheDependencyKeyInternal(entityType);
            }
		}

		private void InvalidateEntityInternal(Type entityType, object entityKey)
		{
			cacheService.Remove(entityCacheKeyGenerator.GetEntityCacheKey(entityType, entityKey));
		}

        private void InvalidateCollectionsInternal(Type entityType, object entity)
        {
            // získáme všechny kolekce odkazující na tuto entitu (obvykle maximálně jeden)
            var referencingCollections = referencingCollectionsStore.GetReferencingCollections(entityType);            
            foreach (var referencingCollection in referencingCollections)
            {
                // získáme hodnotu cizího klíče
                // z ní klíč pro cachování property objektu s daným klíčem
                // a odebereme z cache
                cacheService.Remove(entityCacheKeyGenerator.GetCollectionCacheKey(referencingCollection.EntityType, referencingCollection.GetForeignKeyValue(dbContext, entity), referencingCollection.CollectionPropertyName));
            }            
        }

        private void InvalidateGetAllInternal(Type type)
		{
			cacheService.Remove(entityCacheKeyGenerator.GetAllKeysCacheKey(type));
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