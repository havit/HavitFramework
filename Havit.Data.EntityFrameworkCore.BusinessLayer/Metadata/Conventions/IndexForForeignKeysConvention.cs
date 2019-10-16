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
	public class IndexForForeignKeysConvention : IForeignKeyAddedConvention
	{
		public void ProcessForeignKeyAdded(IConventionRelationshipBuilder relationshipBuilder, IConventionContext<IConventionRelationshipBuilder> context)
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

				if (deletedProperty != null)
				{
					relationshipBuilder.Metadata.DeclaringEntityType.Builder.HasIndex(
						(deletedProperty != null)
							? new List<IConventionProperty> { fkProperty, deletedProperty }.AsReadOnly()
							: new List<IConventionProperty> { fkProperty }.AsReadOnly(),						
						fromDataAnnotation: false /* Convention */);
				}
			}

		}
	}
}
