using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions;

/// <summary>
/// Registruje CollectionExtendedPropertiesConvention do ConventionSetu.
/// </summary>
internal class CollectionExtendedPropertiesConventionPlugin : IConventionSetPlugin
{
	public ConventionSet ModifyConventions(ConventionSet conventionSet)
	{
		CollectionExtendedPropertiesConvention convention = new CollectionExtendedPropertiesConvention();
		conventionSet.NavigationAddedConventions.Add(convention);
		conventionSet.ForeignKeyPropertiesChangedConventions.Add(convention);
		return conventionSet;
	}
}
