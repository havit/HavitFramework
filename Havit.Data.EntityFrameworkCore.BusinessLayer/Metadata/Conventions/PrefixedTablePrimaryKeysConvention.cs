using System;
using System.Linq;
using Havit.Data.EntityFrameworkCore.Metadata;
using Havit.Data.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions;

/// <summary>
/// Konvencia pre názvy stĺpcov s primárnym kľúčom. Pre normálne tabuľky nastaví názov PK na {TableName}ID a pre väzobné M:N len zmení suffix z Id na ID.
/// </summary>
public class PrefixedTablePrimaryKeysConvention : IKeyAddedConvention
{
	public void ProcessKeyAdded(IConventionKeyBuilder keyBuilder, IConventionContext<IConventionKeyBuilder> context)
	{
		// Systémové tabulky nechceme změnit.
		if (keyBuilder.Metadata.DeclaringEntityType.IsSystemType())
		{
			return;
		}

		if (keyBuilder.Metadata.DeclaringEntityType.IsConventionSuppressed(ConventionIdentifiers.NamespaceExtendedPropertyConvention))
		{
			return;
		}

		// pro entitní tabulky očekáváme jednu hodnotu
		// pro vazební tabulky neočekáváme žádnou hodnotu
		var primaryKeysEndingId = keyBuilder.Metadata.Properties
			.Where(property => property
				.GetColumnName(StoreObjectIdentifier.Create(
					property.DeclaringType, StoreObjectType.Table)!.Value) == "Id")
			.ToArray();
		if (primaryKeysEndingId.Length == 1)
		{
			primaryKeysEndingId[0].SetColumnName(keyBuilder.Metadata.DeclaringEntityType.ShortName() + "ID", fromDataAnnotation: false /* Convention */);
		}
	}
}