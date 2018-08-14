using System.Collections.Generic;
using System.Linq;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Conventions
{
	public static class CollectionExtendedPropertiesConvention
	{
		public static void ApplyCollectionExtendedProperties(this ModelBuilder modelBuilder)
		{
			foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
			{
				foreach (IMutableNavigation navigation in entity.GetNavigations().Where(n => n.IsCollection()))
				{
					var extendedProperties = new Dictionary<string, string>
					{
						{ $"Collection_{navigation.PropertyInfo.Name}", navigation.ForeignKey.DeclaringEntityType.Relational().TableName + "." + navigation.ForeignKey.PrincipalKey.Properties[0].Relational().ColumnName }
					};

					entity.AddExtendedProperties(extendedProperties);
				}
			}
		}
	}
}