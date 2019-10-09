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
	public class ManyToManyEntityKeyDiscoveryConventionPlugin : IConventionSetPlugin
	{
		/// <inheritdoc />
		public ConventionSet ModifyConventions(ConventionSet conventionSet)
		{
			conventionSet.ForeignKeyAddedConventions.Add(new ManyToManyEntityKeyDiscoveryConvention());
			return conventionSet;
		}
	}
}
