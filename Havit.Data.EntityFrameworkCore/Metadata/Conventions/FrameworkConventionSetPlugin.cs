using Havit.Data.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Metadata.Conventions
{
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
				conventionSet.EntityTypeAddedConventions.Add(new CacheAttributeToAnnotationConvention(conventionSetBuilderDependencies));
			}

			if (extension.CascadeDeleteToRestrictConventionEnabled)
			{
				CascadeDeleteToRestrictConvention convention = new CascadeDeleteToRestrictConvention(conventionSetBuilderDependencies);

				if (!ConventionSet.Replace<IForeignKeyAddedConvention, CascadeDeleteConvention>(conventionSet.ForeignKeyAddedConventions, convention))
				{
					// pokud se nepodaří vyměnit konvenci za CascadeDeleteConvention, přidáme ji
					conventionSet.ForeignKeyAddedConventions.Add(convention);
				}

				if (!ConventionSet.Replace<IForeignKeyRequirednessChangedConvention, CascadeDeleteConvention>(conventionSet.ForeignKeyRequirednessChangedConventions, convention))
				{
					// pokud se nepodaří vyměnit konvenci za CascadeDeleteConvention, přidáme ji
					conventionSet.ForeignKeyRequirednessChangedConventions.Add(convention);
				}
			}

			if (extension.DataTypeAttributeConventionEnabled)
			{
				conventionSet.PropertyAddedConventions.Add(new DataTypeAttributeConvention(conventionSetBuilderDependencies));
			}

			if (extension.ManyToManyEntityKeyDiscoveryConventionEnabled)
			{
				conventionSet.ForeignKeyAddedConventions.Add(new ManyToManyEntityKeyDiscoveryConvention());
			}

			if (extension.StringPropertiesDefaultValueConventionEnabled)
			{
				conventionSet.PropertyAddedConventions.Add(new StringPropertiesDefaultValueConvention());
			}

			return conventionSet;
		}
	}
}