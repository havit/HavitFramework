using Havit.Data.EntityFrameworkCore.Metadata;
using Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.PropertyLambdaExpressions.Internal;
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
	/// </list>
	/// </summary>
	public class EntityCacheManager : IEntityCacheManager
	{
		private readonly ICacheService cacheService;
		private readonly IEntityCacheSupportDecision entityCacheSupportDecision;
		private readonly IEntityCacheKeyGenerator entityCacheKeyGenerator;
		private readonly IEntityCacheOptionsGenerator entityCacheOptionsGenerator;
		private readonly IDbContext dbContext;
        private readonly IReferencingCollectionsStore referencingCollectionsStore;
        private readonly IEntityKeyAccessor entityKeyAccessor;
		private readonly IPropertyLambdaExpressionManager propertyLambdaExpressionManager;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public EntityCacheManager(ICacheService cacheService, IEntityCacheSupportDecision entityCacheSupportDecision, IEntityCacheKeyGenerator entityCacheKeyGenerator, IEntityCacheOptionsGenerator entityCacheOptionsGenerator, IEntityKeyAccessor entityKeyAccessor, IPropertyLambdaExpressionManager propertyLambdaExpressionManager, IDbContext dbContext, IReferencingCollectionsStore referencingCollectionsStore)
		{
			this.cacheService = cacheService;
			this.entityCacheSupportDecision = entityCacheSupportDecision;
			this.entityCacheKeyGenerator = entityCacheKeyGenerator;
			this.entityCacheOptionsGenerator = entityCacheOptionsGenerator;
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
			if (entityCacheSupportDecision.ShouldCacheEntityType(typeof(TEntity)))
			{
				string cacheKey = entityCacheKeyGenerator.GetEntityCacheKey(typeof(TEntity), key);
				if (cacheService.TryGet(cacheKey, out object cacheValues))
				{
					// pokud je entita v cache, materializujeme ji a vrátíme ji
					TEntity result = Activator.CreateInstance<TEntity>();

					var entry = dbContext.GetEntry(result, suppressDetectChanges: true);
					entry.OriginalValues.SetValues(cacheValues); // aby při případném update byly známy změněné vlastnosti
					entry.CurrentValues.SetValues(cacheValues); // aby byly naplněny vlastnosti entity
					dbContext.Set<TEntity>().AttachRange(new TEntity[] { result }); // nutno volat až po materializaci, jinak registruje entitu s nenastavenou hodnotou primárního klíče

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
				EntityEntry entry = dbContext.GetEntry(entity, suppressDetectChanges: true);
				
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
			if (entityCacheSupportDecision.ShouldCacheAllKeys(typeof(TEntity)))
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
			if (entityCacheSupportDecision.ShouldCacheAllKeys(typeof(TEntity)))
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
			if (entityCacheSupportDecision.ShouldCacheEntityCollection(entity, propertyName))
			{
				string cacheKey = entityCacheKeyGenerator.GetCollectionCacheKey(typeof(TEntity), entityKeyAccessor.GetEntityKeyValues(entity).Single(), propertyName);

				if (cacheService.TryGet(cacheKey, out object cacheEntityPropertyMembersKeys))
				{
					object[][] entityPropertyMembersKeys = (object[][])cacheEntityPropertyMembersKeys;

                    // TODO JK: Performance? Každý pokus o načtení z cache?
					bool isManyToManyEntity = dbContext.Model.FindEntityType(typeof(TPropertyItem)).IsManyToManyEntity();

					var dbSet = dbContext.Set<TPropertyItem>();
					if (isManyToManyEntity)
					{
						var propertyNames = entityKeyAccessor.GetEntityKeyPropertyNames(typeof(TPropertyItem));

						foreach (object[] entityPropertyMemberKey in entityPropertyMembersKeys)
						{
							if (dbSet.FindTracked(entityPropertyMemberKey) == null) // už je načtený, nemůžeme volat TryGetEntity
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
							|| TryGetEntity<TPropertyItem>(entityPropertyMemberKey.Single(), out _));
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
			if (entityCacheSupportDecision.ShouldCacheEntityCollection(entity, propertyName))
			{
				string cacheKey = entityCacheKeyGenerator.GetCollectionCacheKey(typeof(TEntity), entityKeyAccessor.GetEntityKeyValues(entity).Single(), propertyName);

				var propertyLambda = propertyLambdaExpressionManager.GetPropertyLambdaExpression<TEntity, IEnumerable<TPropertyItem>>(propertyName).LambdaCompiled;
				var entityPropertyMembers = propertyLambda(entity) ?? Enumerable.Empty<TPropertyItem>();

				object[][] entityPropertyMembersKeys = entityPropertyMembers.Select(entityPropertyMember => entityKeyAccessor.GetEntityKeyValues(entityPropertyMember)).ToArray();
				cacheService.Add(cacheKey, entityPropertyMembersKeys, entityCacheOptionsGenerator.GetCollectionCacheOptions(entity, propertyName));
			}
		}

		/// <inheritdoc />
		public void Invalidate(Changes changes)
		{
			HashSet<Type> typesToInvalidateGetAll = new HashSet<Type>();

			if (changes.Inserts.Length > 0)
			{
				foreach (var insertedEntity in changes.Inserts)
				{
					InvalidateEntityWithCollectionsInternal(ChangeType.Insert, insertedEntity, typesToInvalidateGetAll);
				}
			}

			if (changes.Updates.Length > 0)
			{
				foreach (var updatedEntity in changes.Updates)
				{
					InvalidateEntityWithCollectionsInternal(ChangeType.Update, updatedEntity, typesToInvalidateGetAll);
				}
			}

			if (changes.Deletes.Length > 0)
			{
				foreach (var deletedEntity in changes.Deletes)
				{
					InvalidateEntityWithCollectionsInternal(ChangeType.Delete, deletedEntity, typesToInvalidateGetAll);
				}
			}

			// invalidaci GetAll uděláme jen jednou pro každý typ (omezíme tak množství zpráv předávaných při případné distribuované invalidaci)
			// zároveň máme zajištěno, že ji provádíme jen pro podporované typy (tj. neprovádíme pro M:N třídy)
			foreach (Type typeToInvalidateGetAll in typesToInvalidateGetAll)
			{
				InvalidateGetAllInternal(typeToInvalidateGetAll);
			}
		}

		private void InvalidateEntityWithCollectionsInternal(ChangeType changeType, object entity, HashSet<Type> typesToInvalidateGetAll)
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
                    InvalidateEntityInternal(entityType, entity, entityKeyValue);
                }

                InvalidateCollectionsInternal(entityType, entity);
				typesToInvalidateGetAll.Add(entityType);
            }
		}

		private void InvalidateEntityInternal(Type entityType, object entity, object entityKey)
		{
            // Pro omezení zasílání informace o Remove při distribuované cache bychom se měli omezit jen na ty objekty, které mohou být cachované.
            if (entityCacheSupportDecision.ShouldCacheEntity(entity))
            {
                cacheService.Remove(entityCacheKeyGenerator.GetEntityCacheKey(entityType, entityKey));
            }
		}

        private void InvalidateCollectionsInternal(Type entityType, object entity)
        {            
            var referencingCollections = referencingCollectionsStore.GetReferencingCollections(entityType);            
            foreach (var referencingCollection in referencingCollections)
            {
                // Pro omezení zasílání informace o Remove při distribuované cache bychom se měli omezit jen na ty objekty, které mohou být cachované.
                // Zde nejsme schopni vždy ověřit instanci, doptáme se tedy na typ.
                if (entityCacheSupportDecision.ShouldCacheEntityTypeCollection(referencingCollection.EntityType, referencingCollection.CollectionPropertyName))
                {
                    // získáme hodnotu cizího klíče
                    // z ní klíč pro cachování property objektu s daným klíčem
                    // a odebereme z cache
                    cacheService.Remove(entityCacheKeyGenerator.GetCollectionCacheKey(referencingCollection.EntityType, referencingCollection.GetForeignKeyValue(dbContext, entity), referencingCollection.CollectionPropertyName));
                }
            }            
        }

        private void InvalidateGetAllInternal(Type type)
		{
            // Pro omezení zasílání informace o Remove při distribuované cache bychom se měli omezit jen na ty objekty, které mohou být cachované.
            if (entityCacheSupportDecision.ShouldCacheAllKeys(type))
            {
                cacheService.Remove(entityCacheKeyGenerator.GetAllKeysCacheKey(type));
            }
        }
	}
}