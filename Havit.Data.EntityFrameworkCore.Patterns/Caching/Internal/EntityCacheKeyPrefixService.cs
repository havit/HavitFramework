using System.Runtime.CompilerServices;
using Havit.Data.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <summary>
/// Služba pro poskytnutí prefixů stringových klíčů do cache.
/// Do klíče generuje názvy typu.
/// Pro distribuovanou invalidaci musí být klíče deterministické.
/// </summary>
public class EntityCacheKeyPrefixService : IEntityCacheKeyPrefixService
{
	private readonly IEntityCacheKeyPrefixStorage entityCacheKeyPrefixStorage;
	private readonly IDbContext dbContext;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public EntityCacheKeyPrefixService(IEntityCacheKeyPrefixStorage entityCacheKeyPrefixStorage, IDbContext dbContext)
	{
		this.entityCacheKeyPrefixStorage = entityCacheKeyPrefixStorage;
		this.dbContext = dbContext;
	}

	/// <inheritdoc />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string GetCacheKeyPrefix(Type type)
	{
		if (entityCacheKeyPrefixStorage.Value == null)
		{
			lock (entityCacheKeyPrefixStorage)
			{
				if (entityCacheKeyPrefixStorage.Value == null)
				{
					var typesByName = dbContext
						 .Model
						 .GetApplicationEntityTypes(includeManyToManyEntities: false)
						 .Select(entityType => entityType.ClrType)
						 .GroupBy(type => type.Name)
						 .ToList();

					var singleTypeOccurences = typesByName
						.Where(group => group.Count() == 1) // tam, kde pod jménem máme jen jednu položku (>99%)
						.Select(group => new { Type = group.Single(), CacheKeyCore = group.Key }); // použijeme jen název třídy (bez namespace)

					var multipleTypeOccurences = typesByName
							.Where(group => group.Count() > 1) // tam, kde máme pod jedním názvem více tříd v různých namespaces (<1%)
							.SelectMany(group => group)
							.Select(type => new { Type = type, CacheKeyCore = type.FullName }); // použijeme celý název třídy vč. namespace

					entityCacheKeyPrefixStorage.Value = singleTypeOccurences.Concat(multipleTypeOccurences)
							.ToFrozenDictionary(item => item.Type, item => "EF|" + item.CacheKeyCore + "|");
				}
			}
		}

		if (entityCacheKeyPrefixStorage.Value.TryGetValue(type, out string result))
		{
			return result;
		}
		else
		{
			throw new InvalidOperationException(string.Format("Type {0} is not a supported type.", type.FullName));
		}
	}
}
