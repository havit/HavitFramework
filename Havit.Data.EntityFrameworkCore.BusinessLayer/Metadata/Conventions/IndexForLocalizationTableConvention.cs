using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions
{
	public class IndexForLocalizationTableConvention : IEntityTypeAddedConvention
	{
		public void ProcessEntityTypeAdded(IConventionEntityTypeBuilder entityTypeBuilder, IConventionContext<IConventionEntityTypeBuilder> context)
		{
			// systémové tabulky neřešíme, nebudou IsBusinessLayerLocalizationEntity
			// suppress nemusíme řešit, vyřeší se odstraněním konvence

			if (entityTypeBuilder.Metadata.IsBusinessLayerLocalizationEntity())
			{
				IEntityType parentEntity = entityTypeBuilder.Metadata.GetBusinessLayerLocalizationParentEntityType();
				IConventionProperty parentLocalizationProperty = entityTypeBuilder.Metadata.GetForeignKeys().FirstOrDefault(fk => fk.PrincipalEntityType == parentEntity)?.Properties?[0];

				IConventionProperty languageProperty = (IConventionProperty)entityTypeBuilder.Metadata.GetBusinessLayerLanguageProperty();

				if ((parentLocalizationProperty != null) && (languageProperty != null))
				{
					IConventionIndexBuilder index = entityTypeBuilder.HasIndex(new List<IConventionProperty> { parentLocalizationProperty, languageProperty }.AsReadOnly(), fromDataAnnotation: false);
					index.IsUnique(true, fromDataAnnotation: false /* Convention */);
				}
			}
		}
	}
}
