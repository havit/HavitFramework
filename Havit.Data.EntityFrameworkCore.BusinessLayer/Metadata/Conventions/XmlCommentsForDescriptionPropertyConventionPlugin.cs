using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions;

/// <summary>
/// Registruje XmlCommentsForDescriptionPropertyConvention do ConventionSetu.
/// </summary>
internal class XmlCommentsForDescriptionPropertyConventionPlugin : IConventionSetPlugin
{
	public ConventionSet ModifyConventions(ConventionSet conventionSet)
	{
		conventionSet.ModelFinalizingConventions.Add(new XmlCommentsForDescriptionPropertyConvention());
		return conventionSet;
	}
}
