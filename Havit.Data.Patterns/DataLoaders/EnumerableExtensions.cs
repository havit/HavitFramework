using System;
using System.Collections.Generic;

namespace Havit.Data.Patterns.DataLoaders
{
	/// <summary>
	/// Rozšiřující metody pro podporu načítání prvků kolekcí pomocí DbDataLoaderu.
	/// </summary>
	public static class EnumerableExtensions
	{
		/// <summary>
		/// Metoda pro umožnění načtení prvků kolekcí v DbDataLoaderu.
		/// <example>
		/// <code>DbDataLoader.Load(entity, e => e.Children.Unwrap().Category);</code>
		/// </example> 
		/// </summary>
		/// <exception cref="System.NotSupportedException">Jakékoliv volání metody.</exception>
		[Obsolete("Use syntax of DbExtensions.Include method (see https://msdn.microsoft.com/en-us/library/gg671236(v=vs.103).aspx).", true)]
		public static TEntity Unwrap<TEntity>(this IEnumerable<TEntity> source)
		{
			throw new NotSupportedException();
		}
	}
}
