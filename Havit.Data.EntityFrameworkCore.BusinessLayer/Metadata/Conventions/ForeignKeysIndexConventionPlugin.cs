﻿using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions;

/// <summary>
/// Registruje IndexForForeignKeysConvention do ConventionSetu.
/// </summary>
internal class ForeignKeysIndexConventionPlugin : IConventionSetPlugin
{
	public ConventionSet ModifyConventions(ConventionSet conventionSet)
	{
		var convention = new ForeignKeysIndexConvention();
		// přidáme se před výchozí ForeignKeyIndexConvention, abychom případně indexy udělali první
		// ForeignKeyIndexConvention potom indexy, které již máme pokryté, neřeší
		// a nepublikuje se informace o ignorování indexu z důvodu pokrytí sloupců jinými indexy
		if (!ConventionSet.AddBefore<IForeignKeyAddedConvention>(conventionSet.ForeignKeyAddedConventions, convention, typeof(ForeignKeyIndexConvention)))
		{
			// pokud by náhodou vestavěná konvence nebyla, přidáme se na konec (za ostatní naše konvence)
			conventionSet.ForeignKeyAddedConventions.Add(convention);
		}

		if (!ConventionSet.AddBefore<IForeignKeyPropertiesChangedConvention>(conventionSet.ForeignKeyPropertiesChangedConventions, convention, typeof(ForeignKeyIndexConvention)))
		{
			conventionSet.ForeignKeyPropertiesChangedConventions.Add(convention);
		}

		if (!ConventionSet.AddBefore<IForeignKeyRemovedConvention>(conventionSet.ForeignKeyRemovedConventions, convention, typeof(ForeignKeyIndexConvention)))
		{
			conventionSet.ForeignKeyRemovedConventions.Add(convention);
		}

		conventionSet.PropertyAnnotationChangedConventions.Add(convention);
		conventionSet.EntityTypeAnnotationChangedConventions.Add(convention);

		return conventionSet;
	}
}
