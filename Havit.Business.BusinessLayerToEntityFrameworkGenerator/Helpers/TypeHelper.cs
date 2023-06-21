using System;
using System.Collections.Generic;
using System.Reflection;
using Havit.Business.BusinessLayerToEntityFrameworkGenerator.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Helpers;

    public static class TypeHelper
    {
        private static readonly Dictionary<string, Type> csharpAliasToTypeMapping = new Dictionary<string, Type>
        {
            { "bool", typeof(bool) },
            { "byte", typeof(byte) },
            { "char", typeof(char) },
            { "decimal", typeof(decimal) },
            { "double", typeof(double) },
            { "float", typeof(float) },
            { "int", typeof(int) },
            { "long", typeof(long) },
            { "object", typeof(object) },
            { "sbyte", typeof(sbyte) },
            { "short", typeof(short) },
            { "string", typeof(string) },
            { "uint", typeof(uint) },
            { "ulong", typeof(ulong) },
            { "ushort", typeof(ushort) },
            { "void", typeof(void) },
        };

        public static Type GetPropertyType(EntityProperty property)
        {
            string typeName = property.TypeName;
            bool isNullable = property.TypeName.EndsWith("?");
            if (isNullable)
            {
                typeName = typeName.Trim('?');
            }

            if (csharpAliasToTypeMapping.TryGetValue(typeName, out Type type))
            {
                return isNullable ? typeof(Nullable<>).MakeGenericType(type) : type;
            }

        Type systemType = Type.GetType($"System.{typeName}", false);
        if (systemType != null)
        {
	        return isNullable ? typeof(Nullable<>).MakeGenericType(systemType) : systemType;
		}

            return Type.GetType(property.TypeName, false);
        }

        public static RelationalTypeMapping GetMapping(Type type)
        {
            return EfCoreAccessor.Use<IRelationalTypeMappingSource, RelationalTypeMapping>(s => s.FindMapping(type));
        }

    public static string GetPropertyTypeName(Column column)
    {
		string typeName = !BusinessLayerGenerator.Helpers.TypeHelper.IsNonstandardType(column)
				? BusinessLayerGenerator.Helpers.TypeHelper.GetPropertyTypeName(column)
				: BusinessLayerGenerator.Helpers.TypeHelper.GetFieldSystemTypeName(column);

		if (typeName == "XmlDocument")
	    {
		    return "string";
	    }

	    return typeName;
    }

    public static bool IsNullableType(Type type)
    {
	    TypeInfo typeInfo = type.GetTypeInfo();

	    return !typeInfo.IsValueType
	           || (typeInfo.IsGenericType
	           && (typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>)));
    }
}