using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;

/// <inheritdoc />
public class DbLoadedPropertyReader : ILoadedPropertyReader
{
	private readonly IDbContext _dbContext;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public DbLoadedPropertyReader(IDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	/// <summary>
	/// Vrací true, pokud je vlastnost objektu již načtena.
	/// </summary>
	public virtual bool IsEntityPropertyLoaded<TEntity>(TEntity entity, string propertyName)
		where TEntity : class
	{
		return _dbContext.IsNavigationLoaded(entity, propertyName);
	}
}
