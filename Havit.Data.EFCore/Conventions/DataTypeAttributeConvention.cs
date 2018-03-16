using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Havit.Data.Entity.Conventions
{
	/// <summary>	
	/// Pokud je vlastnost třídy modelu označena atributem <see cref="DataTypeAttribute"/> s hodnotou <see cref="DataType.Date"/>, pak se použije datový typ Date.	
	/// </summary>
    public class DataTypeAttributeConvention : PropertyAttributeConvention<DataTypeAttribute>
    {
		/// <inheritdoc cref="PropertyAttributeConvention{T}.Apply(InternalPropertyBuilder, DataTypeAttribute, MemberInfo)"/>
		public override InternalPropertyBuilder Apply(InternalPropertyBuilder propertyBuilder, DataTypeAttribute attribute, MemberInfo clrMember)
	    {
		    if (attribute.DataType == DataType.Date)
		    {
			    propertyBuilder.Relational(ConfigurationSource.DataAnnotation).HasColumnType("Date");
		    }
		    else
		    {
			    throw new NotSupportedException($"DataType.{attribute.DataType} is not supported, the only supported value on DataTypeAttribute is DataType.Date.");
		    }

		    return propertyBuilder;
	    }
    }
}
