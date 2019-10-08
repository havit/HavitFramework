using Havit.Data.EntityFrameworkCore.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Metadata.Conventions
{
	/// <summary>
	/// Registruje CacheAttributeToAnnotationConvention do ConventionSetu.
	/// </summary>
	public class CacheAttributeToAnnotationConventionPlugin : IConventionSetPlugin
	{
		private readonly ProviderConventionSetBuilderDependencies dependencies;

		/// <summary>
		/// Konstructor.
		/// </summary>
		public CacheAttributeToAnnotationConventionPlugin(ProviderConventionSetBuilderDependencies dependencies)
		{
			this.dependencies = dependencies;
		}

		/// <inheritdoc />
		public ConventionSet ModifyConventions(ConventionSet conventionSet)
		{
			conventionSet.EntityTypeAddedConventions.Add(new CacheAttributeToAnnotationConvention(dependencies));
			return conventionSet;
		}
	}
}
