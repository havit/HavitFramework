using System.Collections.Concurrent;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;

internal static class EntityActivator
{
	// ConstructorInfo se vyhledá reflexí jen jednou per typ; opakované GetConstructor by bylo zbytečně drahé.
	// Constructor.Invoke (ne kompilovaná Expression.New) zachovává podporu neveřejných (protected/internal) bezparametrických konstruktorů.
	private static readonly ConcurrentDictionary<Type, ConstructorInfo> s_constructors = new ConcurrentDictionary<Type, ConstructorInfo>();

	public static TEntity CreateInstance<TEntity>()
	{
		ConstructorInfo constructor = s_constructors.GetOrAdd(
			typeof(TEntity),
			static type => type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null));
		if (constructor == null)
		{
			throw new InvalidOperationException($"Type {typeof(TEntity).Name} does not have a parameterless constructor.");
		}
		return (TEntity)constructor.Invoke(null);
	}
}
