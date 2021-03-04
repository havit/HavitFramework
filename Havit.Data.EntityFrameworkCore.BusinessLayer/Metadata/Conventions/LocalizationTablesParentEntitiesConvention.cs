using System.Linq;
using Havit.Data.EntityFrameworkCore.Metadata;
using Havit.Data.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions
{
	/// <summary>
	/// Konvencia pre názov stĺpca v tabuľke XyLocalization pre lokalizovanú entitu - použije sa názov primárneho kľúča z tabuľky pre lokalizovanú entitu.
	/// </summary>
	public class LocalizationTablesParentEntitiesConvention : IForeignKeyAddedConvention
	{
		/// <inheritdoc />
		public void ProcessForeignKeyAdded(IConventionForeignKeyBuilder foreignKeyBuilder, IConventionContext<IConventionForeignKeyBuilder> context)
		{
			var entityType = foreignKeyBuilder.Metadata.DeclaringEntityType;

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

			if ((foreignKeyBuilder.Metadata.Properties.Count == 1) && (foreignKeyBuilder.Metadata.Properties.Single().Name == "ParentId"))
			{
				// cizí klíč s názvem vlastnosti ParentId
				var parentIdProperty = foreignKeyBuilder.Metadata.Properties.Single();

				IConventionEntityType principalEntityType = foreignKeyBuilder.Metadata.PrincipalEntityType;
				IConventionProperty property = principalEntityType.FindPrimaryKey().Properties.First();
				string pkColumnName = property.GetColumnName(StoreObjectIdentifier.Create(property.DeclaringEntityType, StoreObjectType.Table)!.Value);
				parentIdProperty.SetColumnName(pkColumnName, fromDataAnnotation: false /* Convention */);
			}
		}
	}
}