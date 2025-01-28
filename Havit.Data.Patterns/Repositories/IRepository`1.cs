namespace Havit.Data.Patterns.Repositories;

/// <summary>
/// Repository objektů typu TEntity s primárním klíčem typu System.Int32.
/// </summary>
/// <remarks>
/// Základní interface pro repository v Entity Framework 6.
/// </remarks>
public interface IRepository<TEntity> : IRepository<TEntity, int>
	where TEntity : class
{
}
