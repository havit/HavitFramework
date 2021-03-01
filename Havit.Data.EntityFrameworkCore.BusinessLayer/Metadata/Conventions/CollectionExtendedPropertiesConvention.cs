using System.Collections.Generic;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.Metadata;
using Havit.Data.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions
{
    /// <summary>
    /// Konvencia pre definovanie Collection_{propertyName} extended properties na entitách. Vynecháva kolekcie s názvom "Localizations", nakoľko by BusinessLayerGenerator vygeneroval kolekciu dvakrát.
    /// </summary>
    public class CollectionExtendedPropertiesConvention : INavigationAddedConvention
	{
		/// <inheritdoc />
		public void ProcessNavigationAdded(IConventionNavigationBuilder navigationBuilder, IConventionContext<IConventionNavigationBuilder> context)
		{
			// Systémové tabulky nechceme změnit.
			if (navigationBuilder.Metadata.DeclaringEntityType.IsSystemType())
			{
				return;
			}

			if (navigationBuilder.Metadata.DeclaringEntityType.IsConventionSuppressed(ConventionIdentifiers.CollectionExtendedPropertiesConvention))
			{
				return;
			}

			if (navigationBuilder.Metadata.IsCollection)
			{
				if ((navigationBuilder.Metadata.Name == "Localizations") && navigationBuilder.Metadata.ForeignKey.DeclaringEntityType.IsBusinessLayerLocalizationEntity())
				{
					// Localizations property cannot have Collection extended property defined
					return;
				}

				var extendedProperties = new Dictionary<string, string>
					{
						{ $"Collection_{navigationBuilder.Metadata.PropertyInfo.Name}", navigationBuilder.Metadata.ForeignKey.DeclaringEntityType.GetTableName() + "." + navigationBuilder.Metadata.ForeignKey.Properties[0].GetColumnName() }
					};

				navigationBuilder.Metadata.TargetEntityType.AddExtendedProperties(extendedProperties, fromDataAnnotation: false /* Convention */);
			}
		}
	}
}