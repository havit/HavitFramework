using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal
{
	/// <summary>
	/// Referencovaná kolekce.
	/// </summary>
	public class ReferencingCollection
	{
		/// <summary>
		/// Typ, který obsahuje vlastnost typu kolekce.
		/// </summary>
		public Type EntityType { get; set; }

		/// <summary>
		/// Název vlastnosti, která je kolekcí.
		/// </summary>
		public string CollectionPropertyName { get; set; }

		/// <summary>
		/// Funkce, která vrátí hodnotu cizího klíče směřujícího k dané kolekci.
		/// </summary>
		public Func<IDbContext, object, object> GetForeignKeyValue { get; set; }
	}
}
