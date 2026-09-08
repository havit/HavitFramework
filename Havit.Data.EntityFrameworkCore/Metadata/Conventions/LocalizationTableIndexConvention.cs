using Havit.Data.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Havit.Model.Localizations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Havit.Data.EntityFrameworkCore.Metadata.Conventions;

/// <summary>
/// V lokalizačních tabulkách vytváří unikátní index s cizími klíči vedoucími do lokalizované tabulky a do tabulky jazyků.
/// </summary>
/// <remarks>
/// Konvence je implementována jako <see cref="IModelFinalizingConvention"/> (a nikoliv jako reakce na změny cizích klíčů),
/// aby nemusela držet stav (dříve vytvořené indexy) - instance konvence může být sdílena mezi více sestaveními modelu.
/// </remarks>
public class LocalizationTableIndexConvention : IModelFinalizingConvention
{
	/// <inheritdoc />
	public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
	{
		foreach (IConventionEntityType entityType in modelBuilder.Metadata.GetEntityTypes())
		{
			// Systémové tabulky nechceme změnit (byť se tato konvence nemůže na systémových tabulkách uplatnit).
			if (entityType.IsSystemType())
			{
				continue;
			}

			if (entityType.IsConventionSuppressed(ConventionIdentifiers.LocalizationTableIndexConvention))
			{
				continue;
			}

			if (!entityType.ClrType.GetInterfaces().Any(item => item.IsGenericType && (item.GetGenericTypeDefinition() == typeof(ILocalization<,>)))) // jsme v lokalizační tabulce?
			{
				continue;
			}

			// najdeme sloupec s odkazem na parent tabulku a sloupec s odkazem na tabulku jazyků
			// GetDeclaredNavigations (nikoliv GetNavigations) - index vytvoříme na typu, který Parent/Language deklaruje. U dědičnosti tak
			// zděděné navigace na potomkovi index neopakují (založí se u předka, kde jsou deklarovány) a zároveň se neztratí u lokalizace
			// dědící z ne-lokalizační báze (navigace si deklaruje sama).
			var parentForeignKey = entityType.GetDeclaredNavigations().FirstOrDefault(p => p.Name == "Parent")?.ForeignKey;
			IConventionProperty parentForeignKeyProperty = (parentForeignKey?.Properties.Count == 1) ? parentForeignKey.Properties[0] : null;

			var languageForeignKey = entityType.GetDeclaredNavigations().FirstOrDefault(p => p.Name == "Language")?.ForeignKey;
			IConventionProperty languageForeignKeyProperty = (languageForeignKey?.Properties.Count == 1) ? languageForeignKey.Properties[0] : null;

			// pokud máme sloupec s odkazem na jazyk i na parent tabulku
			if ((parentForeignKeyProperty != null) && (languageForeignKeyProperty != null) && !parentForeignKeyProperty.IsShadowProperty() && !languageForeignKeyProperty.IsShadowProperty())
			{
				// vytvoříme unikátní index
				entityType.Builder
					.HasIndex(new List<IConventionProperty> { parentForeignKeyProperty, languageForeignKeyProperty }.AsReadOnly(), fromDataAnnotation: false /* Convention */)
					?.IsUnique(unique: true, fromDataAnnotation: false /* Convention */);
			}
		}
	}
}
