using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions
{
	/// <summary>
	/// Registruje IndexForLocalizationTableConvention do ConventionSetu.
	/// </summary>
	internal class LocalizationTableIndexConventionPlugin : IConventionSetPlugin
	{
		public ConventionSet ModifyConventions(ConventionSet conventionSet)
		{
			var convention = new LocalizationTableIndexConvention(); // musíme zajistit existenci jen jediné instance!

			conventionSet.ForeignKeyAddedConventions.Add(convention);
			conventionSet.ForeignKeyPropertiesChangedConventions.Add(convention);
			return conventionSet;
		}
	}
}
