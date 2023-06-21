using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;

public static class ConverterHelper
{
	/// <summary>
	/// Vrátí název converteru k danému sloupci.
	/// </summary>
	public static string GetFieldConvertorName(Column column)
	{
		return "_" + NamingConventions.ConventionsHelper.GetCammelCase(PropertyHelper.GetPropertyName(column)) + "Converter";
	}
}
