using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions;

/// <summary>
/// Registruje IndexForLanguageUiCulturePropertyConvention do ConventionSetu.
/// </summary>
internal class LanguageUiCultureIndexConventionPlugin : IConventionSetPlugin
{
	public ConventionSet ModifyConventions(ConventionSet conventionSet)
	{
		conventionSet.EntityTypeAddedConventions.Add(new LanguageUiCultureIndexConvention());
		return conventionSet;
	}
}
