using System.Runtime.CompilerServices;
using Havit.Data.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <inheritdoc />
public class EntityCacheKeyPrefixStorageBuilder : IEntityCacheKeyPrefixStorageBuilder
{
	private readonly IDbContext _dbContext;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public EntityCacheKeyPrefixStorageBuilder(IDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	/// <inheritdoc />
	public IEntityCacheKeyPrefixStorage Build()
	{
		var typesByName = _dbContext
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

		return new EntityCacheKeyPrefixStorage
		{
			Value = singleTypeOccurences.Concat(multipleTypeOccurences).ToFrozenDictionary(item => item.Type, item => "EF|" + item.CacheKeyCore + "|")
		};
	}
}
