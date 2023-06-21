using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Helpers;

public static class SmoDataTypeExtensions
    {
    private static readonly string[] precisionTypes = new[] { "decimal", "numeric", "real" };

        public static bool IsSameAsTypeMapping(this DataType dataType, RelationalTypeMapping typeMapping)
        {
            if (dataType.IsStringType)
            {
                var storeType = GetBaseStoreType(typeMapping.StoreType);
                return storeType == dataType.Name && (dataType.MaximumLength == (typeMapping.Size ?? -1));
            }

            if (precisionTypes.Contains(dataType.Name))
            {
                string storeType = GetBaseStoreType(typeMapping.StoreType);
            (int precision, int? scale) = GetStoreTypePrecisionAndScale(typeMapping.StoreType);
                return storeType == dataType.Name && dataType.NumericPrecision == precision && dataType.NumericScale == (scale ?? 0);
            }

        return typeMapping.StoreType == dataType.Name;
        }

    private static (int, int?) GetStoreTypePrecisionAndScale(string storeType)
	{
		int index = storeType.IndexOf("(", StringComparison.Ordinal);
		if (index == -1)
		{
			throw new InvalidOperationException($"Store type '{storeType}' does not have precision");
		}

		string[] split = storeType.Substring(index + 1, storeType.Length - index - 2).Split(',');
		int precision = int.Parse(split[0]);
		int? scale = split.Length == 2 ? int.Parse(split[1]) : (int?)null;
		return (precision, scale);
	}

    private static string GetBaseStoreType(string storeType)
        {
            int index = storeType.IndexOf("(", StringComparison.Ordinal);
            if (index >= 0)
            {
                storeType = storeType.Substring(0, index);
            }

            return storeType;
        }

    public static string GetStringRepresentation(this DataType dataType)
    {
	    if (dataType.SqlDataType == SqlDataType.Xml)
	    {
		    return "xml";
	    }
	    if (dataType.IsStringType)
	    {
		    return $"{dataType.Name}({(dataType.MaximumLength == -1 ? "max" : dataType.MaximumLength.ToString())})";
	    }

	    if (dataType.IsNumericType)
	    {
		    var str = dataType.Name;
		    if (dataType.NumericPrecision != 0)
		    {
			    str += $"({dataType.NumericPrecision}";
		    }

		    if (dataType.NumericScale != 0)
		    {
			    str += $", {dataType.NumericScale})";
			}
		    else if (dataType.NumericPrecision != 0)
		    {
			    str += ")";
		    }

		    return str;
	    }

	    return dataType.Name;
    }
}