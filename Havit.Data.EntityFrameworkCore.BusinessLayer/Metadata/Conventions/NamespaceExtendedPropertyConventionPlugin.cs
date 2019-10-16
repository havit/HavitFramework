using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions
{
	/// <summary>
	/// Registruje NamespaceExtendedPropertyConvention do ConventionSetu.
	/// </summary>
	internal class NamespaceExtendedPropertyConventionPlugin : IConventionSetPlugin
	{
		public ConventionSet ModifyConventions(ConventionSet conventionSet)
		{
			conventionSet.EntityTypeAddedConventions.Add(new NamespaceExtendedPropertyConvention());
			return conventionSet;
		}
	}
}
