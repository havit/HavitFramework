﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Metadata.Conventions;

/// <summary>
/// Všem cizím klíčům s nastaví DeleteBehavior na Restrict, čímž se zamezí kaskádnímu delete.
/// </summary>
public class CascadeDeleteToRestrictConvention : CascadeDeleteConvention
{
	/// <summary>
	/// Konstructor.
	/// </summary>
	public CascadeDeleteToRestrictConvention(ProviderConventionSetBuilderDependencies dependencies) : base(dependencies)
	{
	}

	// Podpora pro suppress - není, kdo nechce restrict, nastaví si vlastní hodnotu.

	/// <inheritdoc />
	public override void ProcessForeignKeyAdded(IConventionForeignKeyBuilder relationshipBuilder, IConventionContext<IConventionForeignKeyBuilder> context)
	{
		// Systémové tabulky nechceme změnit (byť zde ani nemá cenu řešit)
		if (relationshipBuilder.Metadata.DeclaringEntityType.IsSystemType())
		{
			return;
		}

		base.ProcessForeignKeyAdded(relationshipBuilder, context);
	}

	/// <inheritdoc />
	public override void ProcessForeignKeyRequirednessChanged(IConventionForeignKeyBuilder relationshipBuilder, IConventionContext<bool?> context)
	{
		// Systémové tabulky nechceme změnit (byť zde ani nemá cenu řešit)
		if (relationshipBuilder.Metadata.DeclaringEntityType.IsSystemType())
		{
			return;
		}

		base.ProcessForeignKeyRequirednessChanged(relationshipBuilder, context);
	}

	/// <inheritdoc />
	protected override DeleteBehavior GetTargetDeleteBehavior(IConventionForeignKey foreignKey)
	{
		// Předek používá fromDataAnnotation false (tj. Convention), resp. nepoužívá žádnou hodnotu a použije se default, tj. fromDataAnnotation=false, tj. Convention.

		// Pro naše účely je správnější použít DeleteBehavior.NoAction. SQL Server nemá podporu ON DELETE RESTRICT, ale jen ON DELETE NO ACTION,
		// takže při DeleteBehavior.Restrict i DeleteBehavior.NoAction se ON DELETE NO ACTION.
		// Nicméně změna výsledku této metody na DeleteBehavior.NoAction na efektivně neudělá nic,
		// "jen" se při tvorbě migrace v cílové aplikaci vytvoří skript, který smaže a znovuzaloží všechny cizí klíče.
		// Z toho důvodu nechávám hodnotu Restrict do okamžiku, než začne být NoAction potřeba.
		return DeleteBehavior.Restrict;
	}

}