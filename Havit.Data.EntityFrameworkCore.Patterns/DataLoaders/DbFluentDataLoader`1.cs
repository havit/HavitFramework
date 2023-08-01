using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;

namespace Havit.Data.Patterns.DataLoaders;

/// <summary>
/// Podpora Fluent API pro DbDataLoader.
/// Použit pro výsledek načtení referencí.
/// 
/// </summary>
internal class DbFluentDataLoader<TEntity> : DbFluentDataLoader<TEntity, TEntity>
	where TEntity : class
{
	public DbFluentDataLoader(DbDataLoader loader, TEntity[] data) : base(loader, data)
	{
	}
}