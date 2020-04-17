using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal
{
	/// <summary>
	/// Seznam kolekcí referencující danou entitu.
	/// </summary>
	public class ReferencingCollectionsStorage : IReferencingCollectionsStorage
	{
		/// <summary>
		/// Seznam kolekcí referencující danou entitu.
		/// </summary>
		public Dictionary<Type, List<ReferencingCollection>> Value { get; set; }
	}
}
