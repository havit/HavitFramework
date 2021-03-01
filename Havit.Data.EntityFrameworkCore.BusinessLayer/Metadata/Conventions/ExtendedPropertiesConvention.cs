using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions
{
    /// <summary>
    /// Konvencia pre nastavenie extended properties na vlastnostiach.
    /// </summary>
    public class ExtendedPropertiesConvention : IEntityTypeAddedConvention, IPropertyAddedConvention, INavigationAddedConvention 
    {
        public void ProcessEntityTypeAdded(IConventionEntityTypeBuilder entityTypeBuilder, IConventionContext<IConventionEntityTypeBuilder> context)
        {
            // Systémové tabulky - nemá cenu řešit, nebudou mít attribut.
            // Podpora pro suppress - nemá význam, stačí nepoužít attribut.

            ExtendedPropertiesAnnotationsHelper.AddExtendedPropertyAnnotations(entityTypeBuilder.Metadata, entityTypeBuilder.Metadata.ClrType, false);
        }

        public void ProcessPropertyAdded(IConventionPropertyBuilder propertyBuilder, IConventionContext<IConventionPropertyBuilder> context)
        {
            // Systémové tabulky - nemá cenu řešit, jejich vlastnosti nebudou mít attribut.
            // Podpora pro suppress - nemá význam, stačí nepoužít attribut.

            if (propertyBuilder.Metadata.IsShadowProperty())
            {
                return;
            }

            ExtendedPropertiesAnnotationsHelper.AddExtendedPropertyAnnotations(propertyBuilder.Metadata, propertyBuilder.Metadata.PropertyInfo, fromDataAnnotation: false);
        }

        public void ProcessNavigationAdded(IConventionNavigationBuilder navigationBuilder, IConventionContext<IConventionNavigationBuilder> context)
        {
            // Systémové tabulky - nemá cenu řešit, jejich vlastnosti nebudou mít attribut.
            // Podpora pro suppress - nemá význam, stačí nepoužít attribut.

            ExtendedPropertiesAnnotationsHelper.AddExtendedPropertyAnnotations(navigationBuilder.Metadata.DeclaringEntityType, navigationBuilder.Metadata.PropertyInfo, false);
        }
    }
}
