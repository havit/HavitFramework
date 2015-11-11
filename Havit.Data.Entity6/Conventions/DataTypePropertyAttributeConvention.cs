using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Havit.Data.Entity.Conventions
{
	/// <summary>
	/// Zajišťuje podporu atributu DataTypeAttribute v modelu.
	/// Podporován je jen DataType.Date.
	/// </summary>
	public class DataTypePropertyAttributeConvention : PrimitivePropertyAttributeConfigurationConvention<DataTypeAttribute>
	{
		/// <summary>
		/// Aplikuje konvenci na model.
		/// </summary>
		public override void Apply(ConventionPrimitivePropertyConfiguration configuration, DataTypeAttribute attribute)
		{
			if (attribute.DataType == DataType.Date)
			{
				configuration.HasColumnType("Date");
			}
			else
			{
				throw new NotSupportedException("DataType.{0} is not supported, the only supported value on DataTypeAttribute is DataType.Date.");
			}
		}
	}
}