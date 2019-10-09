using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Metadata.Conventions
{
	/// <summary>
	/// Registruje CascadeDeleteToRestrictConvention do ConventionSetu.
	/// </summary>
	public class CascadeDeleteToRestrictConventionPlugin : IConventionSetPlugin
	{
		private readonly ProviderConventionSetBuilderDependencies dependencies;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public CascadeDeleteToRestrictConventionPlugin(ProviderConventionSetBuilderDependencies dependencies)
		{
			this.dependencies = dependencies;
		}

		/// <inheritdoc />
		public ConventionSet ModifyConventions(ConventionSet conventionSet)
		{
			CascadeDeleteToRestrictConvention convention = new CascadeDeleteToRestrictConvention(dependencies);

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

			return conventionSet;
		}
	}
}
