using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions
{
	/// <summary>
	/// Zajišťuje tvorbu indexů se sloupcem UiCulture v tabulce jazyků.
	/// </summary>
	public class IndexForLanguageUiCulturePropertyConvention : IEntityTypeAddedConvention
	{
		public void ProcessEntityTypeAdded(IConventionEntityTypeBuilder entityTypeBuilder, IConventionContext<IConventionEntityTypeBuilder> context)
		{
			// systémové tabulky neřešíme, nebudou IsBusinessLayerLocalizationEntity
			// suppress nemusíme řešit, vyřeší se odstraněním konvence

			// Má vůbec význam tvořit tento index nad tabulkou, která má jednotky záznamů?

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
