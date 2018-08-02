using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using static Havit.Business.CodeMigrations.ExtendedProperties.ExtendedPropertiesAnnotationsHelper;

namespace Havit.Business.CodeMigrations.ExtendedProperties
{
	public static class ExtendedPropertiesForExtraDatabaseObjectsBuilder
	{
		public static IEnumerable<IAnnotation> ForExtraDatabaseObject(IDictionary<string, string> extendedProperties, string type1, string name, string schema = null)
		{
			return extendedProperties.Select(kvp => ExtraDatabaseObjectAnnotation(kvp.Key, kvp.Value, schema, type1, name));
		}

		public static IEnumerable<IAnnotation> ForProcedure(IDictionary<string, string> extendedProperties, string procedure, string schema = null)
		{
			return ForExtraDatabaseObject(extendedProperties, "PROCEDURE", procedure, schema);
		}

		public static IEnumerable<IAnnotation> ForView(IDictionary<string, string> extendedProperties, string view, string schema = null)
		{
			return ForExtraDatabaseObject(extendedProperties, "VIEW", view, schema);
		}

		public static IEnumerable<IAnnotation> ForFunction(IDictionary<string, string> extendedProperties, string function, string schema = null)
		{
			return ForExtraDatabaseObject(extendedProperties, "FUNCTION", function, schema);
		}
	}
}
