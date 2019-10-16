using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions
{
	/// <summary>
	/// Registruje IndexForLanguageUiCulturePropertyConvention do ConventionSetu.
	/// </summary>
	internal class IndexForLanguageUiCulturePropertyConventionPlugin : IConventionSetPlugin
	{
		public ConventionSet ModifyConventions(ConventionSet conventionSet)
		{
			conventionSet.EntityTypeAddedConventions.Add(new IndexForLanguageUiCulturePropertyConvention());
			return conventionSet;
		}
	}
}
