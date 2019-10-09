using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Havit.Data.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Havit.Data.EntityFrameworkCore.Metadata.Conventions
{
	/// <summary>
	/// Konvence vyhledá kandidáty na entity reprezentující vztah ManyToMany bez nastaveného primárního klíče a nastaví složený primární klíč.
	/// </summary>
	public class ManyToManyEntityKeyDiscoveryConvention : IForeignKeyAddedConvention, IForeignKeyRequirednessChangedConvention
	{
		/// <summary>
		/// Pokud není definován klíč, pokusí se identifikovat klíč vazební tabulky vztahu M:N.
		/// </summary>
		protected void TryAddPrimaryKey(IConventionEntityType entityType)
		{
			if ((entityType.FindPrimaryKey() == null)
			 && entityType.HasExactlyTwoNotNullablePropertiesWhichAreAlsoForeignKeys())
			{
				// OrderBy - chceme properties v pořadí, v jakém jsou definovány v kódu (nikoliv výchozí = abecední pořadí)
				// fromDataAnnotation: false 
				//  - není definováno v modelu
				//	- jinak dostaneme výjimku System.InvalidOperationException: 'Entity type 'PersonToPerson' has composite primary key defined with data annotations. To set composite primary key, use fluent API.'
				entityType.SetPrimaryKey(entityType.GetProperties().OrderBy(property => property.DeclaringEntityType.ClrType.GetProperties().ToList().IndexOf(property.PropertyInfo)).ToList().AsReadOnly(), fromDataAnnotation: false);
			}
		}

		/// <inheritdoc />
		public void ProcessForeignKeyAdded(IConventionRelationshipBuilder relationshipBuilder, IConventionContext<IConventionRelationshipBuilder> context)
		{
			TryAddPrimaryKey(relationshipBuilder.Metadata.DeclaringEntityType);
		}

		/// <inheritdoc />
		public void ProcessForeignKeyRequirednessChanged(IConventionRelationshipBuilder relationshipBuilder, IConventionContext<IConventionRelationshipBuilder> context)
		{
			TryAddPrimaryKey(relationshipBuilder.Metadata.DeclaringEntityType);
		}
	}
}
