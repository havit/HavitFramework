using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Entity.Patterns.DataLoaders
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
		public static TEntity Unwrap<TEntity>(this IEnumerable<TEntity> source)
		{
			throw new NotSupportedException($"Method \"{nameof(Unwrap)}\" should never be executed. It is a markup method for DataLoader only.");
		}
	}
}
