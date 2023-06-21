using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;

/// <summary>
/// Warning! If annotation structure (name or value) is changed (as part of breaking change), don't forget to update/rewrite tests.
/// </summary>
public static class ExtendedPropertiesForExtraDatabaseObjectsBuilder
{
	public static IEnumerable<IAnnotation> ForExtraDatabaseObject(IDictionary<string, string> extendedProperties, string type1, string name, string schema = null)
	{
		return extendedProperties.Select(kvp => ExtendedPropertiesAnnotationsHelper.ExtraDatabaseObjectAnnotation(kvp.Key, kvp.Value, schema, type1, name));
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
