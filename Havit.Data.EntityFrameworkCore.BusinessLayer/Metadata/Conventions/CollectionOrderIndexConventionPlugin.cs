using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions;

/// <summary>
/// Registruje <see cref="CollectionOrderIndexConvention"/> do <see cref="ConventionSet"/>u.
/// </summary>
internal class CollectionOrderIndexConventionPlugin : IConventionSetPlugin
{
	/// <inheritdoc />
	public ConventionSet ModifyConventions(ConventionSet conventionSet)
	{
		conventionSet.ModelFinalizingConventions.Add(new CollectionOrderIndexConvention());
		return conventionSet;
	}
}
