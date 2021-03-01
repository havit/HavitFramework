using System.Collections.Generic;
using Havit.Data.EntityFrameworkCore.Metadata;
using Havit.Data.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions
{
    /// <summary>
    /// Konvencia pre názvy stĺpcov s cudzím kľúčom - mení suffix z Id na ID (konvencia BusinessLayeru).
    /// </summary>
    public class ForeignKeysColumnNamesConvention : IForeignKeyAddedConvention, IForeignKeyPropertiesChangedConvention
    {
		public void ProcessForeignKeyAdded(IConventionForeignKeyBuilder foreignKeyBuilder, IConventionContext<IConventionForeignKeyBuilder> context)
		{
			SetColumnName(foreignKeyBuilder);
		}

		public void ProcessForeignKeyPropertiesChanged(IConventionForeignKeyBuilder foreignKeyBuilder, IReadOnlyList<IConventionProperty> oldDependentProperties, IConventionKey oldPrincipalKey, IConventionContext<IReadOnlyList<IConventionProperty>> context)
		{

			SetColumnName(foreignKeyBuilder);
		}

		private static void SetColumnName(IConventionForeignKeyBuilder foreignKeyBuilder)
		{
			// Systémové tabulky nechceme změnit.
			if (foreignKeyBuilder.Metadata.DeclaringEntityType.IsSystemType())
			{
				return;
			}

			if (foreignKeyBuilder.Metadata.DeclaringEntityType.IsConventionSuppressed(ConventionIdentifiers.ForeignKeysColumnNamesConvention))
			{
				return;
			}

			foreach (var property in foreignKeyBuilder.Metadata.Properties)
			{
				if (property.IsConventionSuppressed(ConventionIdentifiers.ForeignKeysColumnNamesConvention))
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