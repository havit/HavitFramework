using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.Infrastructure
{
	internal static class EntityActivator
	{
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
}
