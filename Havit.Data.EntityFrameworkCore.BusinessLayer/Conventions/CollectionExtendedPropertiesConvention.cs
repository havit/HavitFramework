using System.Collections.Generic;
using System.Linq;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata;
using Havit.Data.EntityFrameworkCore.Conventions;
using Havit.Data.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Conventions
{
	/// <summary>
	/// Konvencia pre definovanie Collection_{propertyName} extended properties na entitách. Vynecháva kolekcie s názvom "Localizations", nakoľko by BusinessLayerGenerator vygeneroval kolekciu dvakrát.
	/// </summary>
	public class CollectionExtendedPropertiesConvention : IModelConvention
	{
		/// <inheritdoc />
		public void Apply(ModelBuilder modelBuilder)
		{
			foreach (IMutableEntityType entityType in modelBuilder.Model.GetApplicationEntityTypes())
			{
				foreach (IMutableNavigation navigation in entityType.GetNavigations().Where(n => n.IsCollection()))
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

					entityType.AddExtendedProperties(extendedProperties);
				}
			}
		}
	}
}