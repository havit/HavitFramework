using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;

/// <summary>
/// Položka v (I)DbEntityKeyAccessorStorage.
/// </summary>
public class DbEntityKeyAccessorItem
{
	/// <summary>
	/// PropertyInfo reprezentující klíčové vlastnosti.
	/// </summary>
	public required PropertyInfo[] PropertyInfos { get; init; }

	/// <summary>
	/// Názvy vlastností reprezentující klíčové vlastnosti.
	/// </summary>
	public required string[] PropertyNames { get; init; }
}
