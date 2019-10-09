using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Metadata.Conventions
{
	/// <summary>
	/// Registruje StringPropertiesDefaultValueConvention do ConventionSetu.
	/// </summary>
	public class StringPropertiesDefaultValueConventionPlugin : IConventionSetPlugin
	{
		/// <inheritdoc />
		public ConventionSet ModifyConventions(ConventionSet conventionSet)
		{
			conventionSet.PropertyAddedConventions.Add(new StringPropertiesDefaultValueConvention());
			return conventionSet;
		}
	}
}
