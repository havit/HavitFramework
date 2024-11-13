namespace Havit.Data.EntityFrameworkCore.Patterns.Caching;

/// <summary>
/// Konfigurace pro AnnotationsWithDefaultsEntityCacheOptionsGeneratorStorageBuilder.
/// </summary>
public class AnnotationsWithDefaultsEntityCacheOptionsGeneratorStorageBuilderOptions
{
	/// <summary>
	/// Čas absolutní expirace.
	/// </summary>
	public TimeSpan? AbsoluteExpiration { get; set; }

	/// <summary>
	/// Čas sliding expirace.
	/// </summary>
	public TimeSpan? SlidingExpiration { get; set; }
}
