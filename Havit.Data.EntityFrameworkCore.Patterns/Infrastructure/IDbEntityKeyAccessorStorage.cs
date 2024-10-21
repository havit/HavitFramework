using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;

/// <summary>
/// Mapování entit na property info vlastností reprezentující primární klíče.
/// </summary>
/// <remarks>
/// Vyhrazeno pro DbEntityKeyAccessor, tedy "Db" v názvu interface je korektní.
/// </remarks>
public interface IDbEntityKeyAccessorStorage
{
	/// <summary>
	/// Mapování entit na property info vlastností reprezentující primární klíče.
	/// </summary>
	FrozenDictionary<Type, PropertyInfo[]> Value { get; set; }
}
