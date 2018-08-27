using System.Collections.Generic;
using System.Linq;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Conventions
{
	/// <summary>
	/// Helper trieda pre prístup ku Collection_* extended atribútom z <see cref="INavigation"/>.
	/// </summary>
	public class CollectionAttributeAccessor
	{
		private readonly IMutableNavigation navigation;

		/// <summary>
		/// Konštruktor.
		/// </summary>
		public CollectionAttributeAccessor(IMutableNavigation navigation)
		{
			this.navigation = navigation;
		}

		private IEnumerable<(string, string)> GetExtendedProperties() =>
			navigation.DeclaringEntityType.GetExtendedProperties()
				.Where(p => p.Key.StartsWith(PropertyPrefix))
				.Select(p => (p.Key.Substring(PropertyPrefix.Length), p.Value));

		private string PropertyPrefix => $"Collection_{navigation.PropertyInfo.Name}_";

		/// <summary>
		/// Accessor property pre <see cref="CollectionAttribute.Sorting"/> property.
		/// </summary>
		public string Sorting => GetExtendedProperties().FirstOrDefault(p => p.Item1 == nameof(CollectionAttribute.Sorting)).Item2;
	}
}