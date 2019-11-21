using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions
{
    /// <summary>
    /// Registruje <see cref="CollectionOrderIndexConvention"/> do <see cref="ConventionSet"/>u.
    /// </summary>
	internal class CollectionOrderIndexConventionPlugin : IConventionSetPlugin
	{
        /// <inheritdoc />
        public ConventionSet ModifyConventions(ConventionSet conventionSet)
		{
			var convention = new CollectionOrderIndexConvention();

            //conventionSet.NavigationAddedConventions.Add(convention);
            //conventionSet.PropertyAnnotationChangedConventions.Add(convention);
            // insert before ValidatingConvention, which marks model as readonly (and thus it won't be possible to modify model anymore)
            ConventionSet.AddBefore(conventionSet.ModelFinalizedConventions, convention, typeof(ValidatingConvention));
            //conventionSet.ForeignKeyPropertiesChangedConventions.Add(convention);
            return conventionSet;
		}
	}
}
