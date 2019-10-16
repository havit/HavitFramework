using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions
{
	/// <summary>
	/// Registruje IndexNamingConvention do ConventionSetu.
	/// </summary>
	internal class IndexNamingConventionPlugin : IConventionSetPlugin
	{
		public ConventionSet ModifyConventions(ConventionSet conventionSet)
		{
			conventionSet.IndexAddedConventions.Add(new IndexNamingConvention());
			return conventionSet;
		}
	}
}
