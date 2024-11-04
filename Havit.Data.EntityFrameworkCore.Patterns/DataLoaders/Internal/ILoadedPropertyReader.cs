namespace Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;

/// <summary>
/// Získává informace o tom, zda je vlastnost objektu načtena.
/// </summary>
public interface ILoadedPropertyReader
{
	/// <summary>
	/// Získává informace o tom, zda je vlastnost objektu načtena.
	/// </summary>
	bool IsEntityPropertyLoaded<TEntity>(TEntity entity, string propertyName)
		where TEntity : class;
}
