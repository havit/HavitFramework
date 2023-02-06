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
		public void ProcessForeignKeyAdded(IConventionForeignKeyBuilder foreignKeyBuilder,
			IConventionContext<IConventionForeignKeyBuilder> context)
		{
			CreateIndex(foreignKeyBuilder);
		}

		public void ProcessForeignKeyPropertiesChanged(IConventionForeignKeyBuilder foreignKeyBuilder,
			IReadOnlyList<IConventionProperty> oldDependentProperties,
			IConventionKey oldPrincipalKey,
			IConventionContext<IReadOnlyList<IConventionProperty>> context)
		{
			// řeší podporu pro shadow property
			// JK: Nevím úplně proč, ale funguje to. Implementace vychází z ForeignKeyIndexConvention v EF Core 3.0.
			RemoveIndex(foreignKeyBuilder.Metadata.DeclaringEntityType.Builder, oldDependentProperties);
			CreateIndex(foreignKeyBuilder);
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
			if (annotation?.Name == RelationalAnnotationNames.ColumnName)
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
			if (annotation?.Name == RelationalAnnotationNames.TableName)
			{
				RenameIndexes(entityTypeBuilder.Metadata.GetDeclaredIndexes());
			}
		}

		private void CreateIndex(IConventionForeignKeyBuilder foreignKeyBuilder)
		{
			// Systémové tabulky nechceme změnit.
			if (foreignKeyBuilder.Metadata.DeclaringEntityType.IsSystemType())
			{
				return;
			}

			if (foreignKeyBuilder.Metadata.DeclaringEntityType.IsConventionSuppressed(ConventionIdentifiers.ForeignKeysIndexConvention))
			{
				return;
			}

			if (foreignKeyBuilder.Metadata.Properties.Count == 1)
			{
				IConventionProperty fkProperty = foreignKeyBuilder.Metadata.Properties.Single();
				IConventionProperty deletedProperty = (IConventionProperty)foreignKeyBuilder.Metadata.DeclaringEntityType.GetBusinessLayerDeletedProperty();

				// pro shadow property se udělá index vesměs chybně
				// nicméně dále je zpracován pomocí ProcessForeignKeyPropertiesChanged 
				var index = foreignKeyBuilder.Metadata.DeclaringEntityType.Builder.HasIndex(
					(deletedProperty != null)
						? new List<IConventionProperty> { fkProperty, deletedProperty }.AsReadOnly()
						: new List<IConventionProperty> { fkProperty }.AsReadOnly(),
					fromDataAnnotation: false /* Convention */);

				index.HasDatabaseName(GetIndexName(index.Metadata), fromDataAnnotation: false /* Convention */);
			}
		}

		private void RenameIndexes(IEnumerable<IConventionIndex> indexes)
		{
			foreach (var index in indexes)
			{
				if (index.GetDatabaseName().StartsWith("FKX_")) // jen naše indexy
				{
					string newName = GetIndexName(index);
					if (newName != index.GetDatabaseName())
					{
						index.SetDatabaseName(GetIndexName(index), fromDataAnnotation: false /* Convention */);
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

				if ((index != null) && index.GetDatabaseName().StartsWith("FKX_"))
				{
					entityTypeBuilder.HasNoIndex(index);
				}
			}
		}

		internal static string GetIndexName(IConventionIndex index)
		{
			string indexName = index.GetDefaultDatabaseName();
			if (indexName.StartsWith("IX_")) // vždy
			{
				indexName = "FKX_" + indexName.Substring(3);
			}
			return indexName;
		}
	}
}
