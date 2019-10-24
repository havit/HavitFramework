using Havit.Data.EntityFrameworkCore.Metadata;
using Havit.Data.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions
{
	// TODO EF Core 3.0: Přejmenovat vč. Use... a pluginu

	/// <summary>
	/// Zajišťuje tvorbu indexů pro cizí klíče spolu se sloupcem Deleted.
	/// </summary>
	public class IndexForForeignKeysConvention : IForeignKeyAddedConvention, IForeignKeyPropertiesChangedConvention, IForeignKeyRemovedConvention, IPropertyAnnotationChangedConvention, IEntityTypeAnnotationChangedConvention
	{
		public void ProcessForeignKeyAdded(IConventionRelationshipBuilder relationshipBuilder, IConventionContext<IConventionRelationshipBuilder> context)
		{
			CreateIndex(relationshipBuilder);
		}

		public void ProcessForeignKeyPropertiesChanged(IConventionRelationshipBuilder relationshipBuilder, IReadOnlyList<IConventionProperty> oldDependentProperties, IConventionKey oldPrincipalKey, IConventionContext<IConventionRelationshipBuilder> context)
		{
			CreateIndex(relationshipBuilder);
		}

		public void ProcessForeignKeyRemoved(IConventionEntityTypeBuilder entityTypeBuilder, IConventionForeignKey foreignKey, IConventionContext<IConventionForeignKey> context)
		{
			foreach (var property in foreignKey.Properties)
			{
				foreach (var index in property.GetContainingIndexes().ToList())
				{
					entityTypeBuilder.HasNoIndex(index);
				}
			}
		}

		public void ProcessPropertyAnnotationChanged(IConventionPropertyBuilder propertyBuilder, string name, IConventionAnnotation annotation, IConventionAnnotation oldAnnotation, IConventionContext<IConventionAnnotation> context)
		{
			if (annotation.Name == RelationalAnnotationNames.ColumnName)
			{
				RenameIndexes(propertyBuilder.Metadata.GetContainingIndexes());
			}
		}

		public void ProcessEntityTypeAnnotationChanged(IConventionEntityTypeBuilder entityTypeBuilder, string name, IConventionAnnotation annotation, IConventionAnnotation oldAnnotation, IConventionContext<IConventionAnnotation> context)
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

			if (relationshipBuilder.Metadata.DeclaringEntityType.IsConventionSuppressed<IndexForForeignKeysConvention>())
			{
				return;
			}

			if ((relationshipBuilder.Metadata.Properties.Count == 1) && !relationshipBuilder.Metadata.Properties.Single().IsPrimaryKey())
			{
				IConventionProperty fkProperty = relationshipBuilder.Metadata.Properties.Single();
				IConventionProperty deletedProperty = (IConventionProperty)relationshipBuilder.Metadata.DeclaringEntityType.GetBusinessLayerDeletedProperty();

				if (fkProperty.IsShadowProperty())
				{
					return;
				}

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
					index.SetName(GetIndexName(index), fromDataAnnotation: false /* Convention */);
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
