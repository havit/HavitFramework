namespace Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;

/// <summary>
/// Extension metody k Change.
/// </summary>
public static class ChangeExtensions
{
	/// <summary>
	/// Vrací true, pokud je změna k entitě typu TEntity (nebo jejímu potomkovi).
	/// </summary>
	internal static bool IsOfType<TEntity>(this Change change)
	{
		return (change.ClrType != null) && typeof(TEntity).IsAssignableFrom(change.ClrType);
	}
}
