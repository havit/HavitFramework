using Havit.Data.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching;

/// <summary>
/// Služba pro poskytnutí stringových klíčů do cache.
/// Do klíče generuje názvy typu.
/// Pro distribuovanou invalidaci musí být klíče deterministické.
/// </summary>
public class EntityCacheKeyGenerator : IEntityCacheKeyGenerator
{
	private readonly IEntityCacheKeyGeneratorStorage entityCacheKeyGeneratorStorage;
	private readonly IDbContext dbContext;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public EntityCacheKeyGenerator(IEntityCacheKeyGeneratorStorage entityCacheKeyGeneratorStorage, IDbContext dbContext)
	{
		this.entityCacheKeyGeneratorStorage = entityCacheKeyGeneratorStorage;
		this.dbContext = dbContext;
	}

	/// <inheritdoc />
	public string GetEntityCacheKey(Type entityType, object key)
	{
		return GetValueForEntity(entityType) + key.ToString();
	}

	/// <inheritdoc />
	public string GetCollectionCacheKey(Type entityType, object key, string propertyName)
	{
		return GetValueForEntity(entityType) + key.ToString() + "|" + propertyName;
	}

	/// <inheritdoc />
	public string GetAllKeysCacheKey(Type entityType)
	{
		return GetValueForEntity(entityType) + "AllKeys";
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private string GetValueForEntity(Type type)
	{
		if (entityCacheKeyGeneratorStorage.Value == null)
		{
			lock (entityCacheKeyGeneratorStorage)
			{
				if (entityCacheKeyGeneratorStorage.Value == null)
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

					entityCacheKeyGeneratorStorage.Value = singleTypeOccurences.Concat(multipleTypeOccurences)
							.ToDictionary(item => item.Type, item => "EF|" + item.CacheKeyCore + "|");
				}
			}
		}

		if (entityCacheKeyGeneratorStorage.Value.TryGetValue(type, out string result))
		{
			return result;
		}
		else
		{
			throw new InvalidOperationException(String.Format("Type {0} is not a supported type.", type.FullName));
		}
	}

}
