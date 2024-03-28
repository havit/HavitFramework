using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Metadata.Conventions;

/// <summary>
/// "Installer" konvencí podle nastavení v FrameworkConventionSetOptionsExtension.
/// </summary>
public class FrameworkConventionSetPlugin : IConventionSetPlugin
{
	private readonly IDbContextOptions _options;
	private readonly ProviderConventionSetBuilderDependencies _conventionSetBuilderDependencies;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public FrameworkConventionSetPlugin(IDbContextOptions options, ProviderConventionSetBuilderDependencies conventionSetBuilderDependencies)
	{
		this._options = options;
		this._conventionSetBuilderDependencies = conventionSetBuilderDependencies;
	}

	/// <inheritdoc />
	public ConventionSet ModifyConventions(ConventionSet conventionSet)
	{
		var extension = _options.FindExtension<FrameworkConventionSetOptionsExtension>();

		if (extension.CacheAttributeToAnnotationConventionEnabled)
		{
			conventionSet.Add(new CacheAttributeToAnnotationConvention(_conventionSetBuilderDependencies));
		}

		if (extension.CascadeDeleteToRestrictConventionEnabled)
		{
			conventionSet.Remove(typeof(SqlServerOnDeleteConvention));

			CascadeDeleteToRestrictConvention convention = new CascadeDeleteToRestrictConvention(_conventionSetBuilderDependencies);
			conventionSet.Add(convention);
		}

		if (extension.DataTypeAttributeConventionEnabled)
		{
			conventionSet.Add(new DataTypeAttributeConvention(_conventionSetBuilderDependencies));
		}

		if (extension.ManyToManyEntityKeyDiscoveryConventionEnabled)
		{
			conventionSet.Add(new ManyToManyEntityKeyDiscoveryConvention());
		}

		if (extension.StringPropertiesDefaultValueConventionEnabled)
		{
			conventionSet.Add(new StringPropertiesDefaultValueConvention());
		}

		if (extension.LocalizationTableIndexConventionEnabled)
		{
			conventionSet.Add(new LocalizationTableIndexConvention());
		}

		return conventionSet;
	}
}