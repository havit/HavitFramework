using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions
{
	/// <summary>
	/// Registruje DefaultValueAttributeConvention do ConventionSetu.
	/// </summary>
	internal class DefaultValueAttributeConventionPlugin : IConventionSetPlugin
	{
		private readonly ProviderConventionSetBuilderDependencies dependencies;

		public DefaultValueAttributeConventionPlugin(ProviderConventionSetBuilderDependencies dependencies)
		{
			this.dependencies = dependencies;
		}

		public ConventionSet ModifyConventions(ConventionSet conventionSet)
		{
			conventionSet.PropertyAddedConventions.Add(new DefaultValueAttributeConvention(dependencies));
			return conventionSet;
		}
	}
}
