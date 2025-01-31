using Havit.Data.EntityFrameworkCore.Metadata;
using Havit.Data.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions;

/// <summary>
/// Konvencia pre názov stĺpca v tabuľke XyLocalization pre lokalizovanú entitu - použije sa názov primárneho kľúča z tabuľky pre lokalizovanú entitu.
/// </summary>
public class LocalizationTablesParentEntitiesConvention : IForeignKeyAddedConvention, IForeignKeyPropertiesChangedConvention
{
	/// <inheritdoc />
	public void ProcessForeignKeyAdded(IConventionForeignKeyBuilder foreignKeyBuilder, IConventionContext<IConventionForeignKeyBuilder> context)
	{
		TrySetColumnName(foreignKeyBuilder.Metadata);

	}

	/// <inheritdoc />
	public void ProcessForeignKeyPropertiesChanged(IConventionForeignKeyBuilder relationshipBuilder, IReadOnlyList<IConventionProperty> oldDependentProperties, IConventionKey oldPrincipalKey, IConventionContext<IReadOnlyList<IConventionProperty>> context)
	{
		TrySetColumnName(relationshipBuilder.Metadata);
	}

	private void TrySetColumnName(IConventionForeignKey foreignKey)
	{
		var entityType = foreignKey.DeclaringEntityType;

		// Systémové tabulky nechceme změnit.
		if (entityType.IsSystemType())
		{
			return;
		}

		if (entityType.IsConventionSuppressed(ConventionIdentifiers.LocalizationTablesParentEntitiesConvention))
		{
			return;
		}

		// pokud nejde o lokalizační tabulku, končíme
		if (!entityType.ClrType.GetInterfaces().Any(itype => itype.IsGenericType && itype.GetGenericTypeDefinition() == typeof(Havit.Model.Localizations.ILocalization<,>)))
		{
			return;
		}

		if ((foreignKey.Properties.Count == 1) && (foreignKey.Properties.Single().Name == "ParentId"))
		{
			// cizí klíč s názvem vlastnosti ParentId
			var parentIdProperty = foreignKey.Properties.Single();

			IConventionEntityType principalEntityType = foreignKey.PrincipalEntityType;
			IConventionProperty property = principalEntityType.FindPrimaryKey().Properties.First();
			string pkColumnName = property.GetColumnName(StoreObjectIdentifier.Create(property.DeclaringType, StoreObjectType.Table)!.Value);
			parentIdProperty.SetColumnName(pkColumnName, fromDataAnnotation: false /* Convention */);
		}
	}
}