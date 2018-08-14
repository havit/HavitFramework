using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties.Attributes
{
	public class CollectionAttributeAccessor
	{
		private readonly IMutableNavigation navigation;

		public CollectionAttributeAccessor(IMutableNavigation navigation)
		{
			this.navigation = navigation;
		}

		private IEnumerable<(string, string)> GetExtendedProperties() =>
			navigation.DeclaringEntityType.GetExtendedProperties()
				.Where(p => p.Key.StartsWith(PropertyPrefix))
				.Select(p => (p.Key.Substring(PropertyPrefix.Length), p.Value));

		private string PropertyPrefix => $"Collection_{navigation.PropertyInfo.Name}_";

		public string Sorting => GetExtendedProperties().FirstOrDefault(p => p.Item1 == nameof(CollectionAttribute.Sorting)).Item2;
	}
}