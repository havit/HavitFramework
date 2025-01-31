using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions;

/// <summary>
/// Registruje ForeignKeysColumnNamesConvention do ConventionSetu.
/// </summary>
internal class ForeignKeysColumnNamesConventionPlugin : IConventionSetPlugin
{
	public ConventionSet ModifyConventions(ConventionSet conventionSet)
	{
		ForeignKeysColumnNamesConvention convention = new ForeignKeysColumnNamesConvention();

		// potřebujeme se dostat před tvorbu indexů
		if (!ConventionSet.AddBefore(conventionSet.ForeignKeyAddedConventions, convention, typeof(ForeignKeyIndexConvention)))
		{
			conventionSet.ForeignKeyAddedConventions.Add(convention);
		}

		if (!ConventionSet.AddBefore(conventionSet.ForeignKeyPropertiesChangedConventions, convention, typeof(ForeignKeyIndexConvention)))
		{
			conventionSet.ForeignKeyPropertiesChangedConventions.Add(convention);
		}

		return conventionSet;
	}
}
