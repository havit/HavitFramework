using Havit.Services.Caching;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching;

/// <summary>
/// Úložiště CacheOptions k jednotlivým entitám
/// </summary>
public class AnnotationsEntityCacheOptionsGeneratorStorage : IAnnotationsEntityCacheOptionsGeneratorStorage
{
	/// <summary>
	/// Úložiště CacheOptions k jednotlivým entitám
	/// </summary>
	public FrozenDictionary<Type, CacheOptions> Value { get; set; }
}
