using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions
{
	/// <summary>
	/// Registruje IndexForForeignKeysConvention do ConventionSetu.
	/// </summary>
	internal class IndexForForeignKeysConventionPlugin : IConventionSetPlugin
	{
		public ConventionSet ModifyConventions(ConventionSet conventionSet)
		{
			// přidáme se před výchozí ForeignKeyIndexConvention, abychom případně indexy udělali první
			// ForeignKeyIndexConvention potom indexy, které již máme pokryté, neřeší
			// a nepublikuje se informace o ignorování indexu z důvodu pokrytí sloupců jinými indexy
			if (!ConventionSet.AddBefore<IForeignKeyAddedConvention>(conventionSet.ForeignKeyAddedConventions, new IndexForForeignKeysConvention(), typeof(ForeignKeyIndexConvention)))
			{
				// pokud by náhodou vestavěná konvence nebyla, přidáme se na konec (za ostatní naše konvence)
				conventionSet.ForeignKeyAddedConventions.Add(new IndexForForeignKeysConvention());
			}
			return conventionSet;
		}
	}
}
