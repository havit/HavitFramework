using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Havit.Data.EntityFrameworkCore.Metadata;
using Havit.Data.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Havit.Data.EntityFrameworkCore.Metadata.Conventions
{
	/// <summary>
	/// Konvence vyhledá kandidáty na entity reprezentující vztah ManyToMany bez nastaveného primárního klíče a nastaví složený primární klíč.
	/// </summary>
	public class ManyToManyEntityKeyDiscoveryConvention : IForeignKeyAddedConvention, IForeignKeyPropertiesChangedConvention
	{
		/// <inheritdoc />
		public void ProcessForeignKeyAdded(IConventionForeignKeyBuilder relationshipBuilder, IConventionContext<IConventionForeignKeyBuilder> context)
		{
			TryDiscoverPrimaryKey(relationshipBuilder, context);
		}

		/// <inheritdoc />
		public void ProcessForeignKeyPropertiesChanged(IConventionForeignKeyBuilder relationshipBuilder, IReadOnlyList<IConventionProperty> oldDependentProperties, IConventionKey oldPrincipalKey, IConventionContext<IReadOnlyList<IConventionProperty>> context)
		{
			TryDiscoverPrimaryKey(relationshipBuilder, context);
		}

		private void TryDiscoverPrimaryKey(IConventionForeignKeyBuilder relationshipBuilder, IConventionContext context)
		{
			// Systémové tabulky nechceme změnit.
			if (relationshipBuilder.Metadata.DeclaringEntityType.IsSystemType())
			{
				return;
			}

			if (relationshipBuilder.Metadata.DeclaringEntityType.IsConventionSuppressed(ConventionIdentifiers.ManyToManyEntityKeyDiscoveryConvention))
			{
				return;
			}

			var entityType = relationshipBuilder.Metadata.DeclaringEntityType;
			if ((entityType.FindPrimaryKey() == null)
				&& entityType.HasExactlyTwoNotNullablePropertiesWhichAreAlsoForeignKeys())
			{
				// OrderBy - chceme properties v pořadí, v jakém jsou definovány v kódu (nikoliv výchozí = abecední pořadí)
				// fromDataAnnotation: false 
				//  - není definováno v modelu
				//	- jinak dostaneme výjimku System.InvalidOperationException: 'Entity type 'PersonToPerson' has composite primary key defined with data annotations. To set composite primary key, use fluent API.'
				IConventionKey newConventionKey = entityType.SetPrimaryKey(entityType.GetProperties().OrderBy(property => property.DeclaringEntityType.ClrType.GetProperties().ToList().IndexOf(property.PropertyInfo)).ToList().AsReadOnly(), fromDataAnnotation: false /* Convention */);
			}
		}
	}
}