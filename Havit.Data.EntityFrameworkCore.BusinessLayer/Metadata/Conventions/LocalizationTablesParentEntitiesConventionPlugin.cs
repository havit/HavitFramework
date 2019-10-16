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
			conventionSet.ForeignKeyAddedConventions.Add(new LocalizationTablesParentEntitiesConvention());
			return conventionSet;
		}
	}
}
