namespace Havit.Data.Patterns.Repositories;

/// <summary>
/// Repository objektů typu TEntity.
/// </summary>
public interface IRepository<TEntity> : IRepository<TEntity, int>
	where TEntity : class
{
}
