using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.Metadata;
using Havit.Data.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions;

/// <summary>
/// Konvencia pre definovanie Collection_{propertyName} extended properties na entitách. Vynecháva kolekcie s názvom "Localizations", nakoľko by BusinessLayerGenerator vygeneroval kolekciu dvakrát.
/// </summary>
public class CollectionExtendedPropertiesConvention : INavigationAddedConvention, IForeignKeyPropertiesChangedConvention
{
	public void ProcessForeignKeyPropertiesChanged(IConventionForeignKeyBuilder relationshipBuilder, IReadOnlyList<IConventionProperty> oldDependentProperties, IConventionKey oldPrincipalKey, IConventionContext<IReadOnlyList<IConventionProperty>> context)
	{
		var navigation = relationshipBuilder.Metadata.GetNavigation(false);
		if (navigation != null)
		{
			Try(relationshipBuilder.Metadata.GetNavigation(false));
		}
	}

	/// <inheritdoc />
	public void ProcessNavigationAdded(IConventionNavigationBuilder navigationBuilder, IConventionContext<IConventionNavigationBuilder> context)
	{
		Try(navigationBuilder.Metadata);
	}

	private void Try(IConventionNavigation navigation)
	{
		// Systémové tabulky nechceme změnit.
		if (navigation.DeclaringEntityType.IsSystemType())
		{
			return;
		}

		if (navigation.DeclaringEntityType.IsConventionSuppressed(ConventionIdentifiers.CollectionExtendedPropertiesConvention))
		{
			return;
		}

		if (navigation.IsCollection)
		{
			if ((navigation.Name == "Localizations") && navigation.ForeignKey.DeclaringEntityType.IsBusinessLayerLocalizationEntity())
			{
				// Localizations property cannot have Collection extended property defined
				return;
			}

			IConventionProperty fkProperty = navigation.ForeignKey.Properties[0];
			var fkColumn = fkProperty.GetColumnName();
			var extendedProperties = new Dictionary<string, string>
				{
					{ $"Collection_{navigation.PropertyInfo.Name}", navigation.ForeignKey.DeclaringEntityType.GetTableName() + "." + fkColumn }
				};

			navigation.DeclaringEntityType.AddExtendedProperties(extendedProperties, fromDataAnnotation: false /* Convention */);
		}
	}
}