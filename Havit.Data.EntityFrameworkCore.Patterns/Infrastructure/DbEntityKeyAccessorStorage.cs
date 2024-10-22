namespace Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;

/// <summary>
/// Mapování entit na property info vlastností reprezentující primární klíče.
/// </summary>
public class DbEntityKeyAccessorStorage : IDbEntityKeyAccessorStorage
{
	/// <summary>
	/// Mapování entit na property info vlastností reprezentující primární klíče.
	/// </summary>
	public required FrozenDictionary<Type, DbEntityKeyAccessorItem> Value { get; init; }
}
