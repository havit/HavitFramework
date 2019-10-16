using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions
{
	/// <summary>
	/// Registruje CharColumnTypeForCharPropertyConvention do ConventionSetu.
	/// </summary>
	internal class CharColumnTypeForCharPropertyConventionPlugin : IConventionSetPlugin
	{
		public ConventionSet ModifyConventions(ConventionSet conventionSet)
		{
			conventionSet.PropertyAddedConventions.Add(new CharColumnTypeForCharPropertyConvention());
			return conventionSet;
		}
	}
}
