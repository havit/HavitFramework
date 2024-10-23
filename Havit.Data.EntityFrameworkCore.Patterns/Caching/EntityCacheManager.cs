using System.Data;
using System.Reflection;
using System.Runtime.ExceptionServices;
using Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.PropertyLambdaExpressions.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;
using Havit.Data.Patterns.Infrastructure;
using Havit.Diagnostics.Contracts;
using Havit.Services.Caching;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching;

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
	private readonly ICacheService _cacheService;
	private readonly IEntityCacheSupportDecision _entityCacheSupportDecision;
	private readonly IEntityCacheKeyGenerator _entityCacheKeyGenerator;
	private readonly IEntityCacheOptionsGenerator _entityCacheOptionsGenerator;
	private readonly IDbContext _dbContext;
	private readonly IReferencingNavigationsService _referencingNavigationsService;
	private readonly INavigationTargetService _navigationTargetService;
	private readonly IEntityKeyAccessor _entityKeyAccessor;
	private readonly IPropertyLambdaExpressionManager _propertyLambdaExpressionManager;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public EntityCacheManager(ICacheService cacheService, IEntityCacheSupportDecision entityCacheSupportDecision, IEntityCacheKeyGenerator entityCacheKeyGenerator, IEntityCacheOptionsGenerator entityCacheOptionsGenerator, IEntityKeyAccessor entityKeyAccessor, IPropertyLambdaExpressionManager propertyLambdaExpressionManager, IDbContext dbContext, IReferencingNavigationsService referencingNavigationsService, INavigationTargetService navigationTargetService)
	{
		_cacheService = cacheService;
		_entityCacheSupportDecision = entityCacheSupportDecision;
		_entityCacheKeyGenerator = entityCacheKeyGenerator;
		_entityCacheOptionsGenerator = entityCacheOptionsGenerator;
		_entityKeyAccessor = entityKeyAccessor;
		_propertyLambdaExpressionManager = propertyLambdaExpressionManager;
		_dbContext = dbContext;
		_referencingNavigationsService = referencingNavigationsService;
		_navigationTargetService = navigationTargetService;
	}

	/// <inheritdoc />
	public bool TryGetEntity<TEntity>(object key, out TEntity entity)
		where TEntity : class
	{
		// pokud vůbec kdy může být entita cachována, budeme se ptát do cache
		if (_entityCacheSupportDecision.ShouldCacheEntityType(typeof(TEntity)))
		{
			string cacheKey = _entityCacheKeyGenerator.GetEntityCacheKey(typeof(TEntity), key);
			if (_cacheService.TryGet(cacheKey, out object cacheValues))
			{
				// pokud je entita v cache, materializujeme ji a vrátíme ji
				TEntity result = EntityActivator.CreateInstance<TEntity>();
				var entry = _dbContext.GetEntry(result, suppressDetectChanges: true);
				entry.OriginalValues.SetValues(cacheValues); // aby při případném update byly známy změněné vlastnosti
				entry.CurrentValues.SetValues(cacheValues); // aby byly naplněny vlastnosti entity
				_dbContext.Set<TEntity>().Attach(result); // nutno volat až po materializaci, jinak registruje entitu s nenastavenou hodnotou primárního klíče

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
		if (_entityCacheSupportDecision.ShouldCacheEntity(entity))
		{
			string cacheKey = _entityCacheKeyGenerator.GetEntityCacheKey(typeof(TEntity), _entityKeyAccessor.GetEntityKeyValues(entity).Single());
			EntityEntry entry = _dbContext.GetEntry(entity, suppressDetectChanges: true);

			Contract.Assert<InvalidOperationException>(entry.State != Microsoft.EntityFrameworkCore.EntityState.Detached, "Entity must be attached to DbContext."); // abychom mohli získat smysluplné entry.OriginalValues, musí být entita trackovaná (podmínka nutná, nikoliv postačující - neříká, zda má OriginalValues dobře nastaveny).

			// entry.OriginalValues vrací abstraktní PropertyValues, ten nese spoustu vlastostí vč. DbContextu.
			// Držením těchto instancí v cache bychom zabránili GC vyčistit je z paměti.
			// Cachovat proto budeme nový objekt, který reprezentuje originální hodnoty.
			// Ten získáme tak, že zavoláme entry.OriginalValues.ToObject(), což vrátí novou instanci entity ve stavu Detached.
			// Tato instance by měla držen ten základní vlastnosti, bez navigačních vlastností (což vzhledem k Detached dává význam).
			object originalValuesObject = entry.OriginalValues.ToObject();
			_cacheService.Add(cacheKey, originalValuesObject, _entityCacheOptionsGenerator.GetEntityCacheOptions(entity));
		}
	}

	/// <inheritdoc />
	public bool TryGetAllKeys<TEntity>(out object keys)
		where TEntity : class
	{
		// pokud mohou být klíče v cache, budeme je hledat
		if (_entityCacheSupportDecision.ShouldCacheAllKeys(typeof(TEntity)))
		{
			string cacheKey = _entityCacheKeyGenerator.GetAllKeysCacheKey(typeof(TEntity));
			return _cacheService.TryGet(cacheKey, out keys);
		}

		keys = null;
		return false;
	}

	/// <inheritdoc />
	public void StoreAllKeys<TEntity>(Func<object> keysFunc)
		where TEntity : class
	{
		// pokud je možné klíče uložit do cache, uložíme je
		if (_entityCacheSupportDecision.ShouldCacheAllKeys(typeof(TEntity)))
		{
			string cacheKey = _entityCacheKeyGenerator.GetAllKeysCacheKey(typeof(TEntity));
			_cacheService.Add(cacheKey, keysFunc.Invoke(), _entityCacheOptionsGenerator.GetAllKeysCacheOptions<TEntity>());
		}
	}

	/// <inheritdoc />
	public bool TryGetNavigation<TEntity, TPropertyItem>(TEntity entity, string propertyName)
		where TEntity : class
		where TPropertyItem : class
	{
		if (_entityCacheSupportDecision.ShouldCacheEntityNavigation(entity, propertyName))
		{
			string cacheKey = _entityCacheKeyGenerator.GetNavigationCacheKey(typeof(TEntity), _entityKeyAccessor.GetEntityKeyValues(entity).Single(), propertyName);

			if (_cacheService.TryGet(cacheKey, out object cacheEntityPropertyMembersKeys))
			{
				var navigationTarget = _navigationTargetService.GetNavigationTarget(typeof(TEntity), propertyName);

				switch (navigationTarget.NavigationType)
				{
					case NavigationType.OneToMany:
						return TryGetNavigation_OneToMany<TPropertyItem>(cacheEntityPropertyMembersKeys);

					case NavigationType.ManyToManyDecomposedToOneToMany:
						return TryGetNavigation_ManyToManyDecomposedToOneToMany<TPropertyItem>(cacheEntityPropertyMembersKeys);

					case NavigationType.ManyToMany:
						return TryGetNavigation_ManyToMany<TPropertyItem>(navigationTarget, entity, cacheEntityPropertyMembersKeys);

					case NavigationType.OneToOne:
						return TryGetNavigation_OneToOne<TPropertyItem>(cacheEntityPropertyMembersKeys);

					default: throw new InvalidOperationException("Not supported navigation type for retrieving navigations.");
				}
			}
		}
		return false;
	}

	private bool TryGetNavigation_OneToMany<TPropertyItem>(object cacheEntityPropertyMembersKeys)
		where TPropertyItem : class
	{
		object[][] entityPropertyMembersKeys = (object[][])cacheEntityPropertyMembersKeys;

		var dbSet = _dbContext.Set<TPropertyItem>();

		// Řekneme, že se nám podařilo odbavit z cache načtení kolekce typy OneToMany,
		// pokud pro každý prvek, který má být v kolekci platí:
		// 1. Buď je již načtený (trackovaný, attached)
		// 2. Nebo se jej podařilo načíst z cache.
		return entityPropertyMembersKeys.All(entityPropertyMemberKey =>
			(dbSet.FindTracked(entityPropertyMemberKey) != null) // už je načtený, nemůžeme volat TryGetEntity
			|| TryGetEntity<TPropertyItem>(entityPropertyMemberKey.Single(), out _));
	}

	private bool TryGetNavigation_ManyToManyDecomposedToOneToMany<TPropertyItem>(object cacheEntityPropertyMembersKeys)
		where TPropertyItem : class
	{
		object[][] entityPropertyMembersKeys = (object[][])cacheEntityPropertyMembersKeys;

		var dbSet = _dbContext.Set<TPropertyItem>();
		var propertyNames = _entityKeyAccessor.GetEntityKeyPropertyNames(typeof(TPropertyItem));

		// Pro všechny objekty, které mají být v kolekci typu many-to-many (dekomponované na dva one-to-many)
		// vytvoříme instanci reprezentující vazební entitu a tu nastavíme jako trackovanou.

		foreach (object[] entityPropertyMemberKey in entityPropertyMembersKeys)
		{
			if (dbSet.FindTracked(entityPropertyMemberKey) == null) // už je načtený, nemůžeme volat TryGetEntity
			{
				TPropertyItem instance = EntityActivator.CreateInstance<TPropertyItem>();
				for (int i = 0; i < propertyNames.Length; i++)
				{
					typeof(TPropertyItem).GetProperty(propertyNames[i]).SetValue(instance, entityPropertyMemberKey[i]);
				}
				dbSet.Attach(instance);
			}
		}
		return true;
	}

	private bool TryGetNavigation_ManyToMany<TPropertyItem>(NavigationTarget navigationTarget, object parentEntity, object cacheEntityPropertyMembersKeys)
		where TPropertyItem : class
	{
		object[][] entityPropertyMembersKeys = (object[][])cacheEntityPropertyMembersKeys;

		var dbSet = _dbContext.Set<TPropertyItem>();

		List<TPropertyItem> cachedEntities = new List<TPropertyItem>(entityPropertyMembersKeys.Length);
		foreach (object[] entityPropertyMembersKey in entityPropertyMembersKeys)
		{
			var entity = dbSet.FindTracked(entityPropertyMembersKey.Single());
			if (entity == null)
			{
				if (!TryGetEntity<TPropertyItem>(entityPropertyMembersKey.Single(), out entity))
				{
					return false;
				}
			}
			cachedEntities.Add(entity);
		}

		bool hasCachedEntities = cachedEntities.Any();

		if (hasCachedEntities)
		{
			_dbContext.GetEntry(parentEntity, suppressDetectChanges: true).DetectChanges();
		}

		object propertyValue = navigationTarget.PropertyInfo.GetValue(parentEntity);
		if (propertyValue == null)
		{
			if (!navigationTarget.PropertyInfo.CanWrite)
			{
				throw new InvalidOperationException($"Only collection properties with public setters can be used when the value of the collection property has null value (we need to set an instance of the collection). Check the {navigationTarget.PropertyInfo.DeclaringType.Name}.{navigationTarget.PropertyInfo.Name} property.");
			}
			try
			{
				navigationTarget.PropertyInfo.SetValue(parentEntity, cachedEntities);
			}
			catch (ArgumentException ae)
			{
				throw new InvalidOperationException($"Collections properties must be a List<>. {navigationTarget.PropertyInfo.DeclaringType.Name}.{navigationTarget.PropertyInfo.Name} property.", ae);
			}
			catch (TargetInvocationException ex)
			{
				ExceptionDispatchInfo.Throw(ex.InnerException);
			}
		}
		else if (hasCachedEntities)
		{
			if (propertyValue is not List<TPropertyItem> propertyList)
			{
				throw new InvalidOperationException($"Collections properties must be a List<>. {navigationTarget.PropertyInfo.DeclaringType.Name}.{navigationTarget.PropertyInfo.Name} property.");
			}

			if (propertyList.Any())
			{
				propertyList.AddRange(cachedEntities.Except(propertyList));
			}
			else
			{
				propertyList.AddRange(cachedEntities);
			}
		}

		if (hasCachedEntities)
		{
			_dbContext.ChangeTracker.Tracked += TryGetNavigation_ManyToMany_ChangeTracker_Tracked;
			_dbContext.GetEntry(parentEntity, suppressDetectChanges: true).DetectChanges();
			_dbContext.ChangeTracker.Tracked -= TryGetNavigation_ManyToMany_ChangeTracker_Tracked;
		}

		return true;
	}

	private void TryGetNavigation_ManyToMany_ChangeTracker_Tracked(object sender, EntityTrackedEventArgs e)
	{
		Contract.Requires<InvalidOperationException>(e.Entry.State == Microsoft.EntityFrameworkCore.EntityState.Added);
		e.Entry.State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
	}

	private bool TryGetNavigation_OneToOne<TPropertyItem>(object cacheEntityPropertyMembersKeys)
		where TPropertyItem : class
	{
		// Pro vazbu OneToOne se prostě pokusíme získat instanci protistrany vazby.

		return TryGetEntity<TPropertyItem>(((object[])cacheEntityPropertyMembersKeys).Single(), out _);
	}

	/// <inheritdoc />
	public void StoreNavigation<TEntity, TPropertyItem>(TEntity entity, string propertyName)
		where TEntity : class
		where TPropertyItem : class
	{
		if (_entityCacheSupportDecision.ShouldCacheEntityNavigation(entity, propertyName))
		{
			string cacheKey = _entityCacheKeyGenerator.GetNavigationCacheKey(typeof(TEntity), _entityKeyAccessor.GetEntityKeyValues(entity).Single(), propertyName);

			var navigationTarget = _navigationTargetService.GetNavigationTarget(typeof(TEntity), propertyName);

			switch (navigationTarget.NavigationType)
			{
				case NavigationType.OneToMany:
				case NavigationType.ManyToMany:
				case NavigationType.ManyToManyDecomposedToOneToMany:
					{
						var propertyLambda = _propertyLambdaExpressionManager.GetPropertyLambdaExpression<TEntity, IEnumerable<TPropertyItem>>(propertyName).LambdaCompiled;
						var entityPropertyMembers = propertyLambda(entity) ?? Enumerable.Empty<TPropertyItem>();

						object[][] entityPropertyMembersKeys = entityPropertyMembers.Select(entityPropertyMember => _entityKeyAccessor.GetEntityKeyValues(entityPropertyMember)).ToArray();
						_cacheService.Add(cacheKey, entityPropertyMembersKeys, _entityCacheOptionsGenerator.GetNavigationCacheOptions(entity, propertyName));

						// V aktuálním použití DbDataLoaderem nechceme cachovat samotnou entitu, cachuje ji DbDataLoader samostatně.
						// Avšak TryGet spoléhá, že cachována je.
					}
					break;

				case NavigationType.OneToOne:
					{
						var propertyLambda = _propertyLambdaExpressionManager.GetPropertyLambdaExpression<TEntity, TPropertyItem>(propertyName).LambdaCompiled;
						var entityPropertyValue = propertyLambda(entity);

						object[] entityPropertyValueKeys = _entityKeyAccessor.GetEntityKeyValues(entityPropertyValue);
						_cacheService.Add(cacheKey, entityPropertyValueKeys, _entityCacheOptionsGenerator.GetNavigationCacheOptions(entity, propertyName));
					}
					break;

				default: throw new InvalidOperationException("Not supported navigation type for storing navigations.");
			}
		}
	}

	/// <inheritdoc />
	public CacheInvalidationOperation PrepareCacheInvalidation(Changes changes)
	{
		HashSet<string> cacheKeysToInvalidate = new HashSet<string>();
		HashSet<object> entitiesToUpdateInCache = new HashSet<object>();

		HashSet<Type> typesToInvalidateGetAll = new HashSet<Type>();

		foreach (var change in changes.Items)
		{
			PrepareCacheInvalidation_EntityWithNavigationsInternal(change, typesToInvalidateGetAll, cacheKeysToInvalidate, entitiesToUpdateInCache);
		}

		// invalidaci GetAll uděláme jen jednou pro každý typ (omezíme tak množství zpráv předávaných při případné distribuované invalidaci)
		// zároveň máme zajištěno, že ji provádíme jen pro podporované typy (tj. neprovádíme pro M:N třídy)
		foreach (Type typeToInvalidateGetAll in typesToInvalidateGetAll)
		{
			PrepareCacheInvalidation_GetAllInternal(typeToInvalidateGetAll, cacheKeysToInvalidate);
		}

		return new CacheInvalidationOperation(() =>
		{
			// odstraníme položky z cache
			_cacheService.RemoveAll(cacheKeysToInvalidate);

			// aktualizujeme v cache změněné entity
			Dictionary<Type, MethodInfo> methodInfosDictionary = new Dictionary<Type, MethodInfo>();
			object[] invokeArguments = new object[1];

			foreach (object entityToUpdateInCache in entitiesToUpdateInCache)
			{
				// protože je metoda StoreEntity generická, musíme přes reflexi
				try
				{
					invokeArguments[0] = entityToUpdateInCache; // eliminace alokace pole pro každý průchod
					Type entityToUpdateInCacheType = entityToUpdateInCache.GetType();
					if (!methodInfosDictionary.TryGetValue(entityToUpdateInCacheType, out MethodInfo methodInfo))
					{
						methodInfo = this.GetType().GetMethod(nameof(StoreEntity)).MakeGenericMethod(entityToUpdateInCacheType);
						methodInfosDictionary.Add(entityToUpdateInCacheType, methodInfo);
					}

					methodInfo.Invoke(this, invokeArguments);
				}
				catch (TargetInvocationException targetInvocationException)
				{
					ExceptionDispatchInfo.Throw(targetInvocationException.InnerException);
				}
			}
		});
	}

	private void PrepareCacheInvalidation_EntityWithNavigationsInternal(Change change, HashSet<Type> typesToInvalidateGetAll, HashSet<string> cacheKeysToInvalidate, HashSet<object> entitiesToUpdateInCache)
	{
		// invalidate entity cache

		object[] entityKeyValues = _entityKeyAccessor.GetEntityKeyValues(change.Entity);

		// Entity se složeným klíčem (ManyToMany jakožto dekomponovaný vztah i skip navigation)
		if (entityKeyValues.Length > 1)
		{
			// odebereme všechny prvky, které mohou mít objekt v kolekci
			PrepareCacheInvalidation_NavigationsInternal(change, cacheKeysToInvalidate);

			// GetAll a GetEntity není nutné řešit, objekty reprezentující vztah asiciační třídu pro dekomponovaný vztah ManyToMany se do cache nedostávají
		}
		else
		{
			object entityKeyValue = entityKeyValues.Single();

			PrepareCacheInvalidation_EntityInternal(change, entityKeyValue, cacheKeysToInvalidate, entitiesToUpdateInCache);
			PrepareCacheInvalidation_NavigationsInternal(change, cacheKeysToInvalidate);

			if (change.ClrType != null) // Podmínka je snad zbytečná. V běžném kodu není, jak by entita s nesloženým PK neměla ClrType.
			{
				typesToInvalidateGetAll.Add(change.ClrType);
			}
		}
	}

	private void PrepareCacheInvalidation_EntityInternal(Change change, object entityKey, HashSet<string> cacheKeysToInvalidate, HashSet<object> entitiesToUpdateInCache)
	{
		// Pro omezení zasílání informace o Remove při distribuované cache bychom se měli omezit jen na ty objekty, které mohou být cachované.

		// TODO: Co se situací, kdy sami necachujeme, ale chceme invalidovat cache jiné části systému?
		if (_entityCacheSupportDecision.ShouldCacheEntity(change.Entity))
		{
			if (change.ChangeType != ChangeType.Insert)
			{
				// nové entity nemohou být v cache, neinvalidujeme
				cacheKeysToInvalidate.Add(_entityCacheKeyGenerator.GetEntityCacheKey(change.ClrType, entityKey));
			}

			if (change.ChangeType != ChangeType.Delete)
			{
				// když už objekt máme, můžeme jej uložit do cache / aktualizovat v cache
				entitiesToUpdateInCache.Add(change.Entity);
			}
		}
	}

	private void PrepareCacheInvalidation_NavigationsInternal(Change change, HashSet<string> cacheKeysToInvalidate)
	{
		var referencingNavigations = _referencingNavigationsService.GetReferencingNavigations(change.EntityType);
		foreach (var referencingNavigation in referencingNavigations)
		{
			// Pro omezení zasílání informace o Remove při distribuované cache bychom se měli omezit jen na ty objekty, které mohou být cachované.
			// Zde nejsme schopni vždy ověřit instanci, doptáme se tedy na typ.
			if (_entityCacheSupportDecision.ShouldCacheEntityTypeNavigation(referencingNavigation.EntityType, referencingNavigation.NavigationPropertyName))
			{
				// získáme hodnotu cizího klíče				
				object foreignKeyCurrentValue = change.GetCurrentValue(referencingNavigation.SourceEntityForeignKeyProperty);
				object foreignKeyOriginalValue = change.GetOriginalValue(referencingNavigation.SourceEntityForeignKeyProperty);

				bool wasChanged = !EqualityComparer<object>.Default.Equals(foreignKeyCurrentValue, foreignKeyOriginalValue);

				// invalidujeme kolekci předchozího "vlastníka" pokud
				// a) mažeme objekt
				// b) aktualizujeme objekt a došlo k přepojení vztahu
				// (a zároveň máme předchozí hodnotu)
				if (((change.ChangeType == ChangeType.Delete) || ((change.ChangeType == ChangeType.Update) && wasChanged)) && (foreignKeyOriginalValue != null))
				{
					cacheKeysToInvalidate.Add(_entityCacheKeyGenerator.GetNavigationCacheKey(referencingNavigation.EntityType, foreignKeyOriginalValue, referencingNavigation.NavigationPropertyName));
				}

				// invalidujeme kolekci aktuálního "vlastníka" pokud:
				// a) zakládáme nový objekt
				// b) aktualizujeme objekt a došlo k přepojení vztahu
				// (a zároveň máme aktuální hodnotu)
				if (((change.ChangeType == ChangeType.Insert) || ((change.ChangeType == ChangeType.Update) && wasChanged)) && (foreignKeyCurrentValue != null))
				{
					cacheKeysToInvalidate.Add(_entityCacheKeyGenerator.GetNavigationCacheKey(referencingNavigation.EntityType, foreignKeyCurrentValue, referencingNavigation.NavigationPropertyName));
				}
			}
		}
	}

	private void PrepareCacheInvalidation_GetAllInternal(Type type, HashSet<string> cacheKeysToInvalidate)
	{
		// Pro omezení zasílání informace o Remove při distribuované cache bychom se měli omezit jen na ty objekty, které mohou být cachované.
		if (_entityCacheSupportDecision.ShouldCacheAllKeys(type))
		{
			cacheKeysToInvalidate.Add(_entityCacheKeyGenerator.GetAllKeysCacheKey(type));
		}
	}
}
