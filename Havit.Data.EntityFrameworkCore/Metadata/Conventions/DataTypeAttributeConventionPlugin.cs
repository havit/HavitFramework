using Havit.Data.EntityFrameworkCore.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Metadata.Conventions
{
	/// <summary>
	/// Registruje DataTypeAttributeConvention do ConventionSetu.
	/// </summary>
	public class DataTypeAttributeConventionPlugin : IConventionSetPlugin
	{
		private readonly ProviderConventionSetBuilderDependencies dependencies;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public DataTypeAttributeConventionPlugin(ProviderConventionSetBuilderDependencies dependencies)
		{
			this.dependencies = dependencies;
		}

		/// <inheritdoc />
		public ConventionSet ModifyConventions(ConventionSet conventionSet)
		{
			conventionSet.PropertyAddedConventions.Add(new DataTypeAttributeConvention(dependencies));
			return conventionSet;
		}
	}
}
