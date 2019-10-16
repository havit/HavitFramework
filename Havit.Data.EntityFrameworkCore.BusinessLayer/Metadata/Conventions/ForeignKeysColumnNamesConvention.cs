using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Data.EntityFrameworkCore.Metadata;
using Havit.Data.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions
{
	/// <summary>
	/// Konvencia pre názvy stĺpcov s cudzím kľúčom - mení suffix z Id na ID (konvencia BusinessLayeru).
	/// </summary>
    public class ForeignKeysColumnNamesConvention : IForeignKeyAddedConvention
    {
		public void ProcessForeignKeyAdded(IConventionRelationshipBuilder relationshipBuilder, IConventionContext<IConventionRelationshipBuilder> context)
		{
			// Systémové tabulky nechceme změnit.
			if (relationshipBuilder.Metadata.DeclaringEntityType.IsSystemType())
			{
				return;
			}

			if (relationshipBuilder.Metadata.DeclaringEntityType.IsConventionSuppressed<ForeignKeysColumnNamesConvention>())
			{
				return;
			}

			foreach (var property in relationshipBuilder.Metadata.Properties)
			{
				if (property.IsConventionSuppressed<ForeignKeysColumnNamesConvention>())
				{
					continue;
				}

				var columnName = property.GetColumnName();
				if (columnName.EndsWith("Id"))
				{
					string newColumnName = columnName.Left(columnName.Length - 2 /* "Id".Length */) + "ID";
					property.SetColumnName(newColumnName, fromDataAnnotation: false /* Convention */);
				}
			}
		}
	}
}