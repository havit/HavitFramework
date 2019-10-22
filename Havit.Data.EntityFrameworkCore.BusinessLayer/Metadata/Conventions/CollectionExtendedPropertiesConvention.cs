using System.Collections.Generic;
using System.Linq;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata;
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
		public void ProcessNavigationAdded(IConventionRelationshipBuilder relationshipBuilder, IConventionNavigation navigation, IConventionContext<IConventionNavigation> context)
		{
			// Systémové tabulky nechceme změnit.
			if (relationshipBuilder.Metadata.DeclaringEntityType.IsSystemType())
			{
				return;
			}

			if (relationshipBuilder.Metadata.DeclaringEntityType.IsConventionSuppressed<CollectionExtendedPropertiesConvention>())
			{
				return;
			}

			if (navigation.IsCollection())
			{
				if ((navigation.Name == "Localizations") && navigation.ForeignKey.DeclaringEntityType.IsBusinessLayerLocalizationEntity())
				{
					// Localizations property cannot have Collection extended property defined
					return;
				}

				var extendedProperties = new Dictionary<string, string>
					{
						{ $"Collection_{navigation.PropertyInfo.Name}", navigation.ForeignKey.DeclaringEntityType.GetTableName() + "." + navigation.ForeignKey.Properties[0].GetColumnName() }
					};

				relationshipBuilder.Metadata.PrincipalToDependent.DeclaringEntityType.AddExtendedProperties(extendedProperties, fromDataAnnotation: false /* Convention */);
			}
		}
	}
}