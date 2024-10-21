using System.Runtime.CompilerServices;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;
using Havit.Data.Patterns.Infrastructure;
using Havit.Diagnostics.Contracts;
using Havit.Services.Caching;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching;

/// <summary>
/// Třída zajišťující invalidaci závislostí v cache.
/// </summary>
public class EntityCacheDependencyManager : IEntityCacheDependencyManager
{
	private readonly ICacheService _cacheService;
	private readonly IEntityCacheDependencyKeyGenerator _entityCacheDependencyKeyGenerator;
	private readonly IEntityKeyAccessor _entityKeyAccessor;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public EntityCacheDependencyManager(ICacheService cacheService, IEntityCacheDependencyKeyGenerator entityCacheDependencyKeyGenerator, IEntityKeyAccessor entityKeyAccessor)
	{
		_cacheService = cacheService;
		_entityCacheDependencyKeyGenerator = entityCacheDependencyKeyGenerator;
		_entityKeyAccessor = entityKeyAccessor;
	}

	/// <inheritdoc />
	public CacheInvalidationOperation PrepareCacheInvalidation(Changes changes)
	{
		if (!_cacheService.SupportsCacheDependencies)
		{
			// závislosti nemohou být použity
			return null;
		}

		HashSet<string> cacheKeysToInvalidate = new HashSet<string>();
		HashSet<Type> typesToInvalidateAnySaveCacheDependencyKey = new HashSet<Type>();

		foreach (var change in changes)
		{
			if (change.ClrType != null)
			{
				InvalidateEntityDependencies(change.ChangeType, change.Entity, typesToInvalidateAnySaveCacheDependencyKey, cacheKeysToInvalidate);
			}
		}

		foreach (Type typeToInvalidateAnySaveCacheDependencyKey in typesToInvalidateAnySaveCacheDependencyKey)
		{
			// Pro omezení zasílání informace o Remove při distribuované cache invalidujeme AnySave jen jednou pro každý typ.
			InvalidateAnySaveCacheDependencyKeyInternal(typeToInvalidateAnySaveCacheDependencyKey, cacheKeysToInvalidate);
		}

		return new CacheInvalidationOperation(() =>
		{
			_cacheService.RemoveAll(cacheKeysToInvalidate);
		});
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void InvalidateEntityDependencies(ChangeType changeType, object entity, HashSet<Type> typesToInvalidateAnySaveCacheDependencyKey, HashSet<string> cacheKeysToInvalidate)
	{
		Contract.Requires(entity != null);

		// invalidate entity cache
		Type entityType = entity.GetType();

		object[] entityKeyValues = _entityKeyAccessor.GetEntityKeyValues(entity);

		// entity se složeným klíčem nepodporujeme (předpokládáme, že jediné takové jsou reprezentace vztahu ManyToMany)
		if (entityKeyValues.Length == 1)
		{
			object entityKeyValue = entityKeyValues.Single();

			if (changeType != ChangeType.Insert)
			{
				// na nových záznamech nemohou být závislosti, neinvalidujeme
				InvalidateSaveCacheDependencyKeyInternal(entityType, entityKeyValue, cacheKeysToInvalidate);
			}

			// invalidaci AnySave uděláme jen jednou pro každý typ (omezíme tak množství zpráv předávaných při případné distribuované invalidaci)
			typesToInvalidateAnySaveCacheDependencyKey.Add(entityType);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void InvalidateSaveCacheDependencyKeyInternal(Type entityType, object entityKey, HashSet<string> cacheKeysToInvalidate)
	{
		cacheKeysToInvalidate.Add(_entityCacheDependencyKeyGenerator.GetSaveCacheDependencyKey(entityType, entityKey, ensureInCache: false));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void InvalidateAnySaveCacheDependencyKeyInternal(Type entityType, HashSet<string> cacheKeysToInvalidate)
	{
		cacheKeysToInvalidate.Add(_entityCacheDependencyKeyGenerator.GetAnySaveCacheDependencyKey(entityType, ensureInCache: false));
	}

}
