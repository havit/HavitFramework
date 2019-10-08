using Havit.Data.EntityFrameworkCore.Conventions;
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

			if (!ConventionSet.AddAfter<IForeignKeyAddedConvention>(conventionSet.ForeignKeyAddedConventions, convention, typeof(CascadeDeleteConvention)))
			{
				// pokud se nepodaří přidat konvenci za CascadeDeleteConvention, přidáme ji na konec (což vlastně můžeme tak jako tak, ale explicitní vyjádření závislosti se hodí).
				conventionSet.ForeignKeyAddedConventions.Add(convention);
			}

			if (!ConventionSet.AddAfter<IForeignKeyRequirednessChangedConvention>(conventionSet.ForeignKeyRequirednessChangedConventions, convention, typeof(CascadeDeleteConvention)))
			{
				// pokud se nepodaří přidat konvenci za CascadeDeleteConvention, přidáme ji na konec (což vlastně můžeme tak jako tak, ale explicitní vyjádření závislosti se hodí).
				conventionSet.ForeignKeyRequirednessChangedConventions.Add(convention);
			}

			return conventionSet;
		}
	}
}
