namespace Havit.Data.Patterns.DataLoaders;

/// <summary>
/// Podpora Fluent API pro DbDataLoader.
/// Použit pro výsledek načtení referencí.
/// 
/// </summary>
internal class FluentDataLoader<TEntity> : FluentDataLoader<TEntity, TEntity>
	where TEntity : class
{
	public FluentDataLoader(IDataLoader loader, IEnumerable<TEntity> data) : base(loader, data)
	{
	}
}