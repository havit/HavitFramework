﻿using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions;

/// <summary>
/// Registruje NamespaceExtendedPropertyConvention do ConventionSetu.
/// </summary>
internal class NamespaceExtendedPropertyConventionPlugin : IConventionSetPlugin
{
	public ConventionSet ModifyConventions(ConventionSet conventionSet)
	{
		conventionSet.KeyAddedConventions.Add(new NamespaceExtendedPropertyConvention());
		return conventionSet;
	}
}
