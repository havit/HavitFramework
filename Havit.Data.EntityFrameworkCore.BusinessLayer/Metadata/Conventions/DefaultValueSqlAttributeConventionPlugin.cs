using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions;

/// <summary>
/// Registruje DefaultValueSqlAttributeConvention do ConventionSetu.
/// </summary>
internal class DefaultValueSqlAttributeConventionPlugin : IConventionSetPlugin
{
	private readonly ProviderConventionSetBuilderDependencies dependencies;

	public DefaultValueSqlAttributeConventionPlugin(ProviderConventionSetBuilderDependencies dependencies)
	{
		this.dependencies = dependencies;
	}

	public ConventionSet ModifyConventions(ConventionSet conventionSet)
	{
		conventionSet.PropertyAddedConventions.Add(new DefaultValueSqlAttributeConvention(dependencies));
		return conventionSet;
	}
}
