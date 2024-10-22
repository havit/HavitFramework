namespace Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;

internal static class EntityActivator
{
	// TODO EF Core 9: Service + storage + FrozenDictionary (13 -> 4 ns) na získání konstruktoru.
	public static TEntity CreateInstance<TEntity>()
	{
		var constructor = typeof(TEntity).GetConstructor(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, Array.Empty<Type>(), null);
		if (constructor == null)
		{
			throw new MissingMemberException($"Type {typeof(TEntity).Name} does not have a parameterless constructor.");
		}
		return (TEntity)constructor.Invoke(null);
	}
}
