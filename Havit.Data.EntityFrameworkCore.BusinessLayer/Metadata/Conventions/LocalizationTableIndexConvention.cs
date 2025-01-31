using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions;

/// <summary>
/// V lokalizačních tabulkách vytváří unikátní index s cizími klíči vedoucími do lokalizované tabulky a do tabulky jazyků.
/// </summary>
public class LocalizationTableIndexConvention : IForeignKeyAddedConvention, IForeignKeyPropertiesChangedConvention
{
	// systémové tabulky neřešíme, nebudou IsBusinessLayerLocalizationEntity
	// suppress nemusíme řešit, vyřeší se odstraněním konvence

	private readonly Dictionary<IConventionEntityType, IConventionIndex> createdIndexes = new Dictionary<IConventionEntityType, IConventionIndex>();

	public void ProcessForeignKeyAdded(IConventionForeignKeyBuilder foreignKeyBuilder, IConventionContext<IConventionForeignKeyBuilder> context)
	{
		EnsureIndex(foreignKeyBuilder);
	}

	public void ProcessForeignKeyPropertiesChanged(IConventionForeignKeyBuilder foreignKeyBuilder, IReadOnlyList<IConventionProperty> oldDependentProperties, IConventionKey oldPrincipalKey, IConventionContext<IReadOnlyList<IConventionProperty>> context)
	{
		EnsureIndex(foreignKeyBuilder);
	}

	private void EnsureIndex(IConventionForeignKeyBuilder foreignKeyBuilder)
	{
		var entityType = foreignKeyBuilder.Metadata.DeclaringEntityType;

		if (entityType.IsBusinessLayerLocalizationEntity()) // jsme v lokalizační tabulce?
		{
			// pokud jsme již index udělali, zrušíme jej
			if (createdIndexes.TryGetValue(entityType, out var index))
			{
				entityType.Builder.HasNoIndex(index);
				createdIndexes.Remove(entityType);
			}

			// najdeme sloupec s odkazem na parent tabulku
			IReadOnlyEntityType parentEntity = entityType.GetBusinessLayerLocalizationParentEntityType();
			IConventionProperty parentLocalizationProperty = entityType.GetForeignKeys().FirstOrDefault(fk => fk.PrincipalEntityType == parentEntity)?.Properties?[0];

			// najdeme sloupec s odkazem jazyk
			IConventionProperty languageProperty = (IConventionProperty)entityType.GetBusinessLayerLanguageProperty();

			// pokud máme sloupec s odkazem na jazyk i na parent tabulku a alespoň jeden z těchto sloupců je v aktuálním relationshipbuilderu
			if ((parentLocalizationProperty != null) && (languageProperty != null) && !parentLocalizationProperty.IsShadowProperty() && !languageProperty.IsShadowProperty())
			{
				// vytvoříme unikátní index
				IConventionIndexBuilder indexBuilder = entityType.Builder.HasIndex(new List<IConventionProperty> { parentLocalizationProperty, languageProperty }.AsReadOnly(), fromDataAnnotation: false);
				indexBuilder.HasDatabaseName(ForeignKeysIndexConvention.GetIndexName(indexBuilder.Metadata));
				indexBuilder.IsUnique(true, fromDataAnnotation: false /* Convention */);
				createdIndexes[entityType] = indexBuilder.Metadata; // zaznamenáme si vytvořený index
			}
		}
	}
}
