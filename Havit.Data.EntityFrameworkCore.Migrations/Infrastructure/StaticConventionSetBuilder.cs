using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Migrations.Infrastructure;

/// <summary>
/// <see cref="IConventionSetBuilder"/> implementation, that just returns predefined <see cref="ConventionSet"/> instance.
/// </summary>
public class StaticConventionSetBuilder : IConventionSetBuilder
{
	private readonly ConventionSet conventionSet;

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="conventionSet"><see cref="ConventionSet"/> that is returned in <see cref="CreateConventionSet"/></param>
	public StaticConventionSetBuilder(ConventionSet conventionSet)
	{
		this.conventionSet = conventionSet;
	}

	/// <inheritdoc />
	public ConventionSet CreateConventionSet()
	{
		return conventionSet;
	}
}