﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Havit.Data.Entity.Conventions
{
	/// <summary>	
	/// Pokud je vlastnost třídy modelu označena atributem <see cref="DataTypeAttribute"/> s hodnotou <see cref="DataType.Date"/>, pak se použije datový typ Date.	
	/// </summary>
    public class DataTypeAttributeConvention : IModelConvention
	{
		/// <summary>
		/// Aplikuje konvenci.
		/// </summary>
		public void Apply(ModelBuilder modelBuilder)
		{
			var propertiesWithDataTypeAttribute = 
				(from property in modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetDeclaredProperties())
				 where property.PropertyInfo != null // shadow properties
				 from attribute in property.PropertyInfo.GetCustomAttributes(typeof(DataTypeAttribute), false).Cast<DataTypeAttribute>()
				 select new { Property = property, DataTypeAttribute = attribute })
				.ToList();

			foreach (var item in propertiesWithDataTypeAttribute)
			{
				if (item.DataTypeAttribute.DataType == DataType.Date)
				{
					item.Property.Relational().ColumnType = "Date";
				}
				else
				{
					throw new NotSupportedException($"DataType.{item.DataTypeAttribute.DataType} is not supported, the only supported value on DataTypeAttribute is DataType.Date.");
				}
			}
		}
    }
}