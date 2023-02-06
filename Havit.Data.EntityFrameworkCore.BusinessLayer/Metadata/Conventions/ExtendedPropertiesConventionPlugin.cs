using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions
{
	/// <summary>
	/// Registruje <see cref="ExtendedPropertiesConvention"/> do <see cref="ConventionSet"/>u.
	/// </summary>
	internal class ExtendedPropertiesConventionPlugin : IConventionSetPlugin
	{
		public ConventionSet ModifyConventions(ConventionSet conventionSet)
		{
			var convention = new ExtendedPropertiesConvention();

			conventionSet.NavigationAddedConventions.Add(convention);
			conventionSet.EntityTypeAddedConventions.Add(convention);
			conventionSet.PropertyAddedConventions.Add(convention);
			return conventionSet;
		}
	}
}