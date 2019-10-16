using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions
{
	public class IndexForLanguageUiCulturePropertyConvention : IEntityTypeAddedConvention
	{
		public void ProcessEntityTypeAdded(IConventionEntityTypeBuilder entityTypeBuilder, IConventionContext<IConventionEntityTypeBuilder> context)
		{
			// systémové tabulky neřešíme, nebudou IsBusinessLayerLocalizationEntity
			// suppress nemusíme řešit, vyřeší se odstraněním konvence

			if (entityTypeBuilder.Metadata.IsBusinessLayerLanguageEntity())
			{
				IConventionProperty uiCultureProperty = (IConventionProperty)entityTypeBuilder.Metadata.GetBusinessLayerUICultureProperty();
				if (uiCultureProperty != null)
				{
					entityTypeBuilder.HasIndex(new List<IConventionProperty> { uiCultureProperty }.AsReadOnly(), fromDataAnnotation: false /* Convention */);					
				}
			}
		}
	}
}
