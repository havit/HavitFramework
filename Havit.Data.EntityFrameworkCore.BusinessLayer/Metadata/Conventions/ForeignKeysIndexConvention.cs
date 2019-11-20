using System.Collections.Generic;
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
    /// Zajišťuje tvorbu indexů pro cizí klíče spolu se sloupcem Deleted.
    /// </summary>
    public class ForeignKeysIndexConvention :
        IForeignKeyAddedConvention,
        IForeignKeyPropertiesChangedConvention,
        IForeignKeyRemovedConvention,
        IPropertyAnnotationChangedConvention,
        IEntityTypeAnnotationChangedConvention
	{
		public void ProcessForeignKeyAdded(
            IConventionRelationshipBuilder relationshipBuilder,
            IConventionContext<IConventionRelationshipBuilder> context)
		{
			CreateIndex(relationshipBuilder);
		}

        public void ProcessForeignKeyPropertiesChanged(
            IConventionRelationshipBuilder relationshipBuilder,
            IReadOnlyList<IConventionProperty> oldDependentProperties,
            IConventionKey oldPrincipalKey,
            IConventionContext<IConventionRelationshipBuilder> context)
		{
			// řeší podporu pro shadow property
			// JK: Nevím úplně proč, ale funguje to. Implementace vychází z ForeignKeyIndexConvention v EF Core 3.0.
			RemoveIndex(relationshipBuilder.Metadata.DeclaringEntityType.Builder, oldDependentProperties);
			CreateIndex(relationshipBuilder);
		}

        public void ProcessForeignKeyRemoved(
            IConventionEntityTypeBuilder entityTypeBuilder,
            IConventionForeignKey foreignKey,
            IConventionContext<IConventionForeignKey> context)
		{
			RemoveIndex(entityTypeBuilder, foreignKey.Properties);
		}

        public void ProcessPropertyAnnotationChanged(
            IConventionPropertyBuilder propertyBuilder,
            string name,
            IConventionAnnotation annotation,
            IConventionAnnotation oldAnnotation,
            IConventionContext<IConventionAnnotation> context)
		{
			if (annotation.Name == RelationalAnnotationNames.ColumnName)
			{
				RenameIndexes(propertyBuilder.Metadata.GetContainingIndexes());
			}
		}

        public void ProcessEntityTypeAnnotationChanged(
            IConventionEntityTypeBuilder entityTypeBuilder,
            string name,
            IConventionAnnotation annotation,
            IConventionAnnotation oldAnnotation,
            IConventionContext<IConventionAnnotation> context)
		{
			if (annotation.Name == RelationalAnnotationNames.TableName)
			{
				RenameIndexes(entityTypeBuilder.Metadata.GetDeclaredIndexes());
			}
		}

		private void CreateIndex(IConventionRelationshipBuilder relationshipBuilder)
		{
			// Systémové tabulky nechceme změnit.
			if (relationshipBuilder.Metadata.DeclaringEntityType.IsSystemType())
			{
				return;
			}

			if (relationshipBuilder.Metadata.DeclaringEntityType.IsConventionSuppressed(ConventionIdentifiers.ForeignKeysIndexConvention))
			{
				return;
			}

			if (relationshipBuilder.Metadata.Properties.Count == 1)
			{
				IConventionProperty fkProperty = relationshipBuilder.Metadata.Properties.Single();
				IConventionProperty deletedProperty = (IConventionProperty)relationshipBuilder.Metadata.DeclaringEntityType.GetBusinessLayerDeletedProperty();

				// pro shadow property se udělá index vesměs chybně
				// nicméně dále je zpracován pomocí ProcessForeignKeyPropertiesChanged 
				var index = relationshipBuilder.Metadata.DeclaringEntityType.Builder.HasIndex(
					(deletedProperty != null)
						? new List<IConventionProperty> { fkProperty, deletedProperty }.AsReadOnly()
						: new List<IConventionProperty> { fkProperty }.AsReadOnly(),
					fromDataAnnotation: false /* Convention */);

				index.HasName(GetIndexName(index.Metadata), fromDataAnnotation: false /* Convention */);
			}
		}

		private void RenameIndexes(IEnumerable<IConventionIndex> indexes)
		{
			foreach (var index in indexes)
			{
				if (index.GetName().StartsWith("FKX_")) // jen naše indexy
				{
					string newName = GetIndexName(index);
					if (newName != index.GetName())
					{
						index.SetName(GetIndexName(index), fromDataAnnotation: false /* Convention */);
					}
				}
			}
		}

		private void RemoveIndex(IConventionEntityTypeBuilder entityTypeBuilder, IEnumerable<IConventionProperty> properties)
		{
			if (properties.Count() == 1)
			{
				IConventionProperty deletedProperty = (IConventionProperty)entityTypeBuilder.Metadata.GetBusinessLayerDeletedProperty();
				// index je potřeba hledat pomocí FindIndex nad vlastnostmi, nelze použít GetContainingIndexes() nad property
				var index = entityTypeBuilder.Metadata.FindIndex((deletedProperty != null)
					? new List<IConventionProperty> { properties.Single(), deletedProperty }.AsReadOnly()
					: new List<IConventionProperty> { properties.Single() }.AsReadOnly());

				if ((index != null) && index.GetName().StartsWith("FKX_"))
				{
					entityTypeBuilder.HasNoIndex(index);
				}
			}
		}

		internal static string GetIndexName(IConventionIndex index)
		{
			string indexName = index.GetDefaultName();
			if (indexName.StartsWith("IX_")) // vždy
			{
				indexName = "FKX_" + indexName.Substring(3);
			}
			return indexName;
		}
	}
}
