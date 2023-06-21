using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Metadata.Conventions;

/// <summary>
/// "Installer" konvencí podle nastavení v FrameworkConventionSetOptionsExtension.
/// </summary>
public class FrameworkConventionSetPlugin : IConventionSetPlugin
{
	private readonly IDbContextOptions options;
	private readonly ProviderConventionSetBuilderDependencies conventionSetBuilderDependencies;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public FrameworkConventionSetPlugin(IDbContextOptions options, ProviderConventionSetBuilderDependencies conventionSetBuilderDependencies)
	{
		this.options = options;
		this.conventionSetBuilderDependencies = conventionSetBuilderDependencies;
	}

	/// <inheritdoc />
	public ConventionSet ModifyConventions(ConventionSet conventionSet)
	{
		var extension = options.FindExtension<FrameworkConventionSetOptionsExtension>();

		if (extension.CacheAttributeToAnnotationConventionEnabled)
		{
			conventionSet.Add(new CacheAttributeToAnnotationConvention(conventionSetBuilderDependencies));
		}

		if (extension.CascadeDeleteToRestrictConventionEnabled)
		{
			// ponecháme vestavěnou CascadeDeleteConvention a přebijeme ji pomocí CascadeDeleteToRestrictConvention
			CascadeDeleteToRestrictConvention convention = new CascadeDeleteToRestrictConvention(conventionSetBuilderDependencies);
			conventionSet.Add(convention);
		}

		if (extension.DataTypeAttributeConventionEnabled)
		{
			conventionSet.Add(new DataTypeAttributeConvention(conventionSetBuilderDependencies));
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