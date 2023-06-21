using Havit.Data.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Havit.Model.Localizations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Collections.Generic;
using System.Linq;

namespace Havit.Data.EntityFrameworkCore.Metadata.Conventions;

/// <summary>
/// V lokalizačních tabulkách vytváří unikátní index s cizími klíči vedoucími do lokalizované tabulky a do tabulky jazyků.
/// </summary>
public class LocalizationTableIndexConvention : IForeignKeyAddedConvention, IForeignKeyPropertiesChangedConvention
{
	private readonly Dictionary<IConventionEntityType, IConventionIndex> createdIndexes = new Dictionary<IConventionEntityType, IConventionIndex>();

	/// <inheritdoc />
	public void ProcessForeignKeyAdded(IConventionForeignKeyBuilder foreignKeyBuilder, IConventionContext<IConventionForeignKeyBuilder> context)
	{
		EnsureIndex(foreignKeyBuilder);
	}

	/// <inheritdoc />
	public void ProcessForeignKeyPropertiesChanged(IConventionForeignKeyBuilder foreignKeyBuilder, IReadOnlyList<IConventionProperty> oldDependentProperties, IConventionKey oldPrincipalKey, IConventionContext<IReadOnlyList<IConventionProperty>> context)
	{
		EnsureIndex(foreignKeyBuilder);
	}

	private void EnsureIndex(IConventionForeignKeyBuilder foreignKeyBuilder)
	{
		// Systémové tabulky nechceme změnit (byť se tato konvence nemůže na systémových tabulkách uplatnit).
		if (foreignKeyBuilder.Metadata.DeclaringEntityType.IsSystemType())
		{
			return;
		}

		if (foreignKeyBuilder.Metadata.DeclaringEntityType.IsConventionSuppressed(ConventionIdentifiers.LocalizationTableIndexConvention))
		{
			return;
		}

		var entityType = foreignKeyBuilder.Metadata.DeclaringEntityType;

		if (entityType.ClrType.GetInterfaces().Any(item => item.IsGenericType && (item.GetGenericTypeDefinition() == typeof(ILocalization<,>)))) // jsme v lokalizační tabulce?
		{
			// pokud jsme již index udělali, zrušíme jej
			if (createdIndexes.TryGetValue(entityType, out var index))
			{
				entityType.Builder.HasNoIndex(index);
				createdIndexes.Remove(entityType);
			}

			// najdeme sloupec s odkazem na parent tabulku
			IConventionProperty parentForeignKeyProperty = entityType.GetNavigations().FirstOrDefault(p => p.Name == "Parent")?.ForeignKey?.Properties.SingleOrDefault();
			IConventionProperty languageForeignKeyProperty = entityType.GetNavigations().FirstOrDefault(p => p.Name == "Language")?.ForeignKey?.Properties.SingleOrDefault();

			// pokud máme sloupec s odkazem na jazyk i na parent tabulku a alespoň jeden z těchto sloupců je v aktuálním relationshipbuilderu
			if ((parentForeignKeyProperty != null) && (languageForeignKeyProperty != null) && !parentForeignKeyProperty.IsShadowProperty() && !languageForeignKeyProperty.IsShadowProperty())
			{
				// vytvoříme unikátní index
				IConventionIndexBuilder indexBuilder = entityType.Builder.HasIndex(new List<IConventionProperty> { parentForeignKeyProperty, languageForeignKeyProperty }.AsReadOnly(), fromDataAnnotation: false);
				indexBuilder.IsUnique(true, fromDataAnnotation: false /* Convention */);
				createdIndexes[entityType] = indexBuilder.Metadata; // zaznamenáme si vytvořený index
			}
		}
	}
}
