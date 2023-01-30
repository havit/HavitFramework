using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions
{
	/// <summary>
	/// Registruje LocalizationTablesParentEntitiesConvention do ConventionSetu.
	/// </summary>
	internal class LocalizationTablesParentEntitiesConventionPlugin : IConventionSetPlugin
	{
		public ConventionSet ModifyConventions(ConventionSet conventionSet)
		{
			LocalizationTablesParentEntitiesConvention convention = new LocalizationTablesParentEntitiesConvention();

			// potřebujeme se dostat před tvorbu indexů
			// a za pojmenování primárních klíčů
			if (!ConventionSet.AddAfter(conventionSet.ForeignKeyAddedConventions, convention, typeof(PrefixedTablePrimaryKeysConvention)))
			{
				if (!ConventionSet.AddBefore(conventionSet.ForeignKeyAddedConventions, convention, typeof(ForeignKeyIndexConvention)))
				{
					conventionSet.ForeignKeyAddedConventions.Add(convention);
				}
			}

			conventionSet.ForeignKeyPropertiesChangedConventions.Add(convention);

			return conventionSet;

		}
	}
}
