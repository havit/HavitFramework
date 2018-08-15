using System.Collections.Generic;
using System.Linq;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata;
using Havit.Data.EntityFrameworkCore.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Conventions
{
	public class CollectionExtendedPropertiesConvention : IModelConvention
	{
		public void Apply(ModelBuilder modelBuilder)
		{
			foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
			{
				foreach (IMutableNavigation navigation in entity.GetNavigations().Where(n => n.IsCollection()))
				{
					if (navigation.Name == "Localizations" && navigation.ForeignKey.DeclaringEntityType.IsLocalizationEntity())
					{
						// Localizations property cannot have Collection extended property defined
						continue;
					}

					var extendedProperties = new Dictionary<string, string>
					{
						{ $"Collection_{navigation.PropertyInfo.Name}", navigation.ForeignKey.DeclaringEntityType.Relational().TableName + "." + navigation.ForeignKey.Properties[0].Relational().ColumnName }
					};

					entity.AddExtendedProperties(extendedProperties);
				}
			}
		}
	}
}