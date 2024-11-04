using Havit.Data.Patterns.DataLoaders;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;

/// <summary>
/// Interní použití v DbDataLoaderu.
/// </summary>
internal class LoadPropertyInternalResult
{
	public required IEnumerable Entities { get; init; }
	public required object FluentDataLoader { get; init; }

	internal static LoadPropertyInternalResult CreateEmpty<TEntity>()
		where TEntity : class
	{
		return new LoadPropertyInternalResult
		{
			Entities = Enumerable.Empty<TEntity>(),
			FluentDataLoader = new NullFluentDataLoader<TEntity>()
		};
	}
}