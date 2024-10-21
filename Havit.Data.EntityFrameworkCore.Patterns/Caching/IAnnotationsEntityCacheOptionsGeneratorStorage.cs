using System.Collections.Frozen;
using Havit.Services.Caching;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching;

/// <summary>
/// Úložiště CacheOptions k jednotlivým entitám
/// </summary>
public interface IAnnotationsEntityCacheOptionsGeneratorStorage
{
	/// <summary>
	/// Úložiště CacheOptions k jednotlivým entitám
	/// </summary>
	FrozenDictionary<Type, CacheOptions> Value { get; set; }
}
