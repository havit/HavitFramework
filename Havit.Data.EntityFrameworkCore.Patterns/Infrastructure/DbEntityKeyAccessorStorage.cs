using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;

/// <summary>
/// Mapování entit na property info vlastností reprezentující primární klíče.
/// </summary>
public class DbEntityKeyAccessorStorage : IDbEntityKeyAccessorStorage
{
	/// <summary>
	/// Mapování entit na property info vlastností reprezentující primární klíče.
	/// </summary>
	public Dictionary<Type, PropertyInfo[]> Value { get; set; }
}
