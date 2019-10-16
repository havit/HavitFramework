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
		public void ProcessForeignKeyAdded(IConventionRelationshipBuilder relationshipBuilder, IConventionContext<IConventionRelationshipBuilder> context)
		{
			var entityType = relationshipBuilder.Metadata.DeclaringEntityType;

			// Systémové tabulky nechceme změnit.
			if (entityType.IsSystemType())
			{
				return;
			}

			if (entityType.IsConventionSuppressed<LocalizationTablesParentEntitiesConvention>())
			{
				return;
			}

			// pokud nejde o lokalizační tabulku, končíme
			if (entityType.ClrType.GetInterfaces().Any(itype => itype.IsGenericType && itype.GetGenericTypeDefinition() == typeof(Havit.Model.Localizations.ILocalization<,>)))
			{
				return;
			}

			if ((relationshipBuilder.Metadata.Properties.Count == 1) && (relationshipBuilder.Metadata.Properties.Single().Name == "ParentId"))
			{
				// cizí klíč s názvem vlastnosti ParentId
				var parentIdProperty = relationshipBuilder.Metadata.Properties.Single();

				IConventionEntityType principalEntityType = relationshipBuilder.Metadata.PrincipalEntityType;
				string pkColumnName = principalEntityType.FindPrimaryKey().Properties.First().GetColumnName();
				parentIdProperty.SetColumnName(pkColumnName, fromDataAnnotation: false /* Convention */);
			}
		}
	}
}